using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;

// TODO: UI bug with animation and skin name length
namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    /// <summary>
    /// Provides methods for work with slots
    /// </summary>
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
                    Move(_x, _y);
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
                    Move(_x, _y);
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
                    Rotate(_a);
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

        /// <summary>
        /// Sets actual attachment to slot according current skin
        /// </summary>
        public void UpdateAttachment()
        {
            CurrentAttachment = this._globalState.CurrentProject!.CurrentSkin.GetAttachment(this);

            if (CurrentAttachment != null && this.BoundedBone != null)
            {
                this._x = this.BoundedBone.x + CurrentAttachment.x;
                this._y = this.BoundedBone.y + CurrentAttachment.y;
                this._a = CurrentAttachment.a + this.BoundedBone.a;

                this.parentA = this.BoundedBone.a;

                Dictionary<string, int?> size = CurrentAttachment.GetSize();
                this.LengthX = size["width"] ?? this.LengthX;
                this.LengthY = size["height"] ?? this.LengthY;
            }
        }

        public SortedDictionary<double, DrawOrderOffset> drawOrders =
            new SortedDictionary<double, DrawOrderOffset>();

        public bool isUpdatingFromCode;
        private int _currentDrawOrderOffset;
        public int CurrentDrawOrderOffset
        {
            get => _currentDrawOrderOffset;
            set
            {
                if (value == null)
                {
                    _currentDrawOrderOffset = 0;
                    OnPropertyChanged();
                    return;
                }

                _currentDrawOrderOffset = value;
                if (!isUpdatingFromCode)
                {
                    double currTime = this._globalState.CurrentProject.CurrentAnimation.currentTime;
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
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Updates draw order offset according current animation time
        /// </summary>
        public void UpdateDrawOrderOffset()
        {
            double currTime = this._globalState.CurrentProject.CurrentAnimation.currentTime;

            double? foundKey = null;
            foreach (var key in drawOrders.Keys)
            {
                if (key <= currTime)
                    foundKey = key;
                else
                    break;
            }

            var value = foundKey.HasValue ? drawOrders[foundKey.Value] : null;
            if (value != null)
            {
                isUpdatingFromCode = true;
                this.CurrentDrawOrderOffset = value.Offset;
                isUpdatingFromCode = false;
            }
            else
            {
                isUpdatingFromCode = true;
                this.CurrentDrawOrderOffset = 0;
                isUpdatingFromCode = false;
            }
        }

        public double parentA = 0;
        private double _lengthX = 100;

        public override double LengthX
        {
            get => _lengthX;
            set
            {
                if (_lengthX != value && value > 0)
                {
                    _lengthX = value;
                    OnPropertyChanged(nameof(LengthX));
                }
            }
        }
        private double _lengthY = 100;
        public override double LengthY
        {
            get => _lengthY;
            set
            {
                if (_lengthY != value && value > 0)
                {
                    _lengthY = value;
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
                    if (value != null)
                    {
                        Move(value.x, value.y);
                    }
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

            this.Name = $"{Path.GetFileNameWithoutExtension(path)}{Counter.GenerateNamePostfix()}";

            this._globalState = globalState;
            UpdateAttachment();
        }

        public Slot(GlobalState globalState, string name, Bone b)
        {
            this.Name = name;
            this.BoundedBone = b;
            this._globalState = globalState;
            UpdateAttachment();
        }

        public Slot(GlobalState globalState, Bone b)
        {
            this.Name = $"tesr{Counter.GenerateNamePostfix()}";
            this.BoundedBone = b;
            this._globalState = globalState;
            UpdateAttachment();
        }

        /// <summary>
        /// Moves slot to target position
        /// </summary>
        /// <param name="x">Target x coordinate</param>
        /// <param name="y">Target y coordinate</param>
        public override void Move(double x, double y)
        {
            this.x = x;
            this.y = y;

            CurrentAttachment?.SetPos(
                this.x - this.BoundedBone!.x,
                this.y - this.BoundedBone.y,
                this.a - this.BoundedBone.a
            );
        }

        /// <summary>
        /// Changes slot size
        /// </summary>
        /// <param name="x">X click coordinate</param>
        /// <param name="y">Y click coordinate</param>
        public override void Scale(double x, double y)
        {
            if (this.CurrentAttachment != null)
            {
                this.LengthX = Math.Abs(x - this.x) * 5;
                this.LengthY = Math.Abs(y - this.y) * 5;

                this.CurrentAttachment.SetSize(this.LengthX, this.LengthY);
            }
        }

        /// <summary>
        /// Rotates slot ONLY FROM UI!
        /// </summary>
        /// <param name="a">Target angle</param>
        public override void Rotate(double a)
        {
            this.a = a;

            CurrentAttachment?.SetPos(
                this.x - this.BoundedBone!.x,
                this.y - this.BoundedBone.y,
                this.a - this.BoundedBone.a
            );
        }

        private Bitmap _cachedBitmap;
        private string _cachedPath;

        public void DrawSlot(Canvas canvas)
        {
            if (!this._globalState.CurrentProject!.CurrentSkin.IsSlotDrawable(this))
            {
                return;
            }

            try
            {
                string currentPath = this._globalState.CurrentProject.CurrentSkin.GetImagePath(
                    this
                );

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
                    Width = _lengthX,
                    Height = _lengthY,
                    RenderTransform = new RotateTransform(this.a + this.parentA),
                };

                double left = canvas.Width / 2 + this.x - image.Width / 2;
                double top = canvas.Height / 2 + this.y - image.Height / 2;

                Canvas.SetLeft(image, left);
                Canvas.SetTop(image, top);

                canvas.Children.Add(image);

                if (this._globalState.IsSlotSelected(this))
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

        /// <summary>
        /// Disposes cached bitmap
        /// </summary>
        private void Dispose()
        {
            _cachedBitmap?.Dispose();
        }

        public new SlotData GenerateJSONData()
        {
            return new SlotData
            {
                Name = this.Name,
                Bone = this.BoundedBone?.Name,
                Attachment = this.CurrentAttachment?.Name,
            };
        }

        public new string GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }

        /// <summary>
        /// Sets name to IRenamble object
        /// </summary>
        /// <param name="name">New name</param>
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

    /// <summary>
    /// Slot JSON data
    /// </summary>
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
