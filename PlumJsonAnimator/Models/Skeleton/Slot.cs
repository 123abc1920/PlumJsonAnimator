using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;

// TODO: add draworder time
namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    public class Slot : Bone, IRenamable
    {
        public override bool IsBone
        {
            get { return false; }
        }
        private double _x = 0;
        private double _y = 0;
        private double _a = 0;

        public override double x
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > double.Epsilon)
                {
                    _x = value;
                    move(_x, _y);
                    OnPropertyChanged(nameof(x));
                }
            }
        }

        public override double y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) > double.Epsilon)
                {
                    _y = value;
                    move(_x, _y);
                    OnPropertyChanged(nameof(y));
                }
            }
        }

        public override double a
        {
            get => _a;
            set
            {
                if (Math.Abs(_a - value) > double.Epsilon)
                {
                    _a = value;
                    rotate(_a);
                    OnPropertyChanged(nameof(a));
                }
            }
        }

        private Attachment? _currentAttachment;

        public Attachment? CurrentAttachment
        {
            get => _currentAttachment;
            set
            {
                if (_currentAttachment != value)
                {
                    _currentAttachment = value;
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateAttachment()
        {
            CurrentAttachment = this.globalState.currentProject!.CurrentSkin.GetAttachment(this);

            if (CurrentAttachment != null)
            {
                this._x = CurrentAttachment.x;
                this._y = CurrentAttachment.y;
                this._a = CurrentAttachment.a;
            }
        }

        public SortedDictionary<double, DrawOrderOffset> drawOrders =
            new SortedDictionary<double, DrawOrderOffset>();

        private int _currentDrawOrderOffset;
        public int CurrentDrawOrderOffset
        {
            get => _currentDrawOrderOffset;
            set
            {
                _currentDrawOrderOffset = value;
                double currTime = this.globalState.currentProject.CurrentAnimation.currentTime;
                if (drawOrders.Keys.Contains(currTime))
                {
                    drawOrders[currTime].Offset = value;
                }
                else
                {
                    drawOrders.Add(
                        currTime,
                        new DrawOrderOffset() { Slot = this.Name, Offset = value }
                    );
                }
                OnPropertyChanged();
            }
        }
        public double parentA = 0;
        private double lengthX = 100;
        private double lengthY = 100;
        public override double LengthX
        {
            get => lengthX;
            set
            {
                if (lengthX != value && value > 0)
                {
                    lengthX = value;
                    OnPropertyChanged(nameof(LengthX));
                }
            }
        }

        public override double LengthY
        {
            get => lengthY;
            set
            {
                if (lengthY != value && value > 0)
                {
                    lengthY = value;
                    OnPropertyChanged(nameof(LengthY));
                }
            }
        }
        private Bone? _boundedBone;
        public Bone? BoundedBone
        {
            get => _boundedBone;
            set
            {
                if (_boundedBone != value)
                {
                    _boundedBone = value;
                    OnPropertyChanged();
                }
            }
        }

        public Slot(GlobalState globalState, int id, string path)
        {
            this.id = id;
            this.a = 0;
            this.x = 0;
            this.y = 0;

            this.Name = $"{Path.GetFileNameWithoutExtension(path)}{Counter.GenerateName()}";

            this.globalState = globalState;
            UpdateAttachment();
        }

        public Slot(GlobalState globalState, string name, Bone b)
        {
            this.Name = name;
            this.BoundedBone = b;
            this.globalState = globalState;
            UpdateAttachment();
        }

        public Slot(GlobalState globalState, Bone b)
        {
            this.Name = $"tesr{Counter.GenerateName()}";
            this.BoundedBone = b;
            this.globalState = globalState;
            UpdateAttachment();
        }

        public void setBone(Bone? b)
        {
            this.BoundedBone = b;
            if (b != null)
            {
                this.x = b.x;
                this.y = b.y;
            }
        }

        public override void move(double x, double y)
        {
            this.x = x;
            this.y = y;

            CurrentAttachment?.SetPos(this.x, this.y, this.a);
        }

        public override void scale(double x, double y)
        {
            this.LengthX = Math.Abs(x - this.x) * 5;
            this.LengthY = Math.Abs(y - this.y) * 5;
        }

        public override void rotate(double a)
        {
            this.a = a;

            CurrentAttachment?.SetPos(this.x, this.y, this.a);
        }

        private Bitmap _cachedBitmap;
        private string _cachedPath;

        public void drawSlot(Canvas canvas)
        {
            if (!this.globalState.currentProject!.CurrentSkin.isSlotDrawable(this))
            {
                return;
            }

            try
            {
                string currentPath = this.globalState.currentProject.CurrentSkin.GetImagePath(this);

                if (_cachedBitmap == null || _cachedPath != currentPath)
                {
                    _cachedPath = currentPath;
                    byte[] imageBytes = File.ReadAllBytes(currentPath);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        _cachedBitmap?.Dispose();
                        _cachedBitmap = new Avalonia.Media.Imaging.Bitmap(ms);
                    }
                }

                var image = new Image
                {
                    Source = _cachedBitmap,
                    Width = lengthX,
                    Height = lengthY,
                    RenderTransform = new RotateTransform(this.a + this.parentA),
                };

                double left = canvas.Width / 2 + this.x - image.Width / 2;
                double top = canvas.Height / 2 + this.y - image.Height / 2;

                Canvas.SetLeft(image, left);
                Canvas.SetTop(image, top);

                canvas.Children.Add(image);

                if (this.globalState.IsSlotSelected(this))
                {
                    int SELECTION = 10;

                    var border = new Border
                    {
                        Width = SELECTION,
                        Height = SELECTION,
                        BorderBrush = AppColors.Red,
                        BorderThickness = new Thickness(2),
                        Background = null,
                    };

                    Canvas.SetLeft(border, canvas.Width / 2 + this.x - SELECTION / 2);
                    Canvas.SetTop(border, canvas.Height / 2 + this.y - SELECTION / 2);

                    canvas.Children.Add(border);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _cachedBitmap?.Dispose();
        }

        public new SlotData generateJSONData()
        {
            return new SlotData
            {
                Name = this.Name,
                Bone = this.BoundedBone?.Name,
                Attachment = this.CurrentAttachment?.Name,
            };
        }

        public new string generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), this.globalState.jsonSettings);
        }

        public Slot regenerate(string json, string imagesFolder = "")
        {
            try
            {
                var data = JsonConvert.DeserializeObject<SlotData>(
                    json,
                    this.globalState.jsonSettings
                );

                if (data == null)
                    return new Slot(this.globalState, 0, "default.png");

                string imagePath;
                if (!string.IsNullOrEmpty(imagesFolder) && Directory.Exists(imagesFolder))
                {
                    var possibleExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    foreach (var ext in possibleExtensions)
                    {
                        var fullPath = imagesFolder + data.Attachment + ext;
                        if (File.Exists(fullPath))
                        {
                            imagePath = fullPath;
                            break;
                        }
                    }
                    imagePath = imagesFolder + data.Attachment + ".png";
                }
                else
                {
                    imagePath = data.Attachment + ".png";
                }

                var slot = new Slot(this.globalState, 0, imagePath) { Name = data.Name };

                return slot;
            }
            catch
            {
                return new Slot(this.globalState, 0, "default.png");
            }
        }

        public new void SetName(string? name)
        {
            if (name != null)
            {
                this.Name = name;
            }
        }

        public new string GetName
        {
            get => this.Name;
            set
            {
                if (this.Name != value)
                {
                    this.Name = value;
                }
            }
        }
    }

    public class SlotData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("bone", NullValueHandling = NullValueHandling.Ignore)]
        public string? Bone { get; set; }

        [JsonProperty("attachment", NullValueHandling = NullValueHandling.Ignore)]
        public string? Attachment { get; set; }
    }
}
