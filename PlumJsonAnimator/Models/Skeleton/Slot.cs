using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;
using Common.Constants;
using Newtonsoft.Json;

namespace PlumJsonAnimator.Models.Skeleton
{
    public class Slot : Bone, INotifyPropertyChanged, IRenamable
    {
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

        private string _name = "";
        public override string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private Attachment _currentAttachment;

        public Attachment CurrentAttachment
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
            CurrentAttachment = ConstantsClass.currentProject.CurrentSkin.GetAttachment(this);
        }

        public int DrawOrder { get; set; }
        public double parentA = 0;
        public double lengthX = 100;
        public double lengthY = 100;
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

        public Slot(int id, string path)
        {
            this.id = id;
            this.a = 0; // Теперь это свойство, которое использует _parentA
            this.x = 0;
            this.y = 0;

            this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
            this.isBone = false;
            UpdateAttachment();
        }

        public Slot(string name, Bone b)
        {
            this.Name = name;
            this.BoundedBone = b;
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
        }

        public override void scale(double x, double y)
        {
            lengthX += 50;
        }

        public override void rotate(double a)
        {
            this.a = a;
        }

        public void drawSlot(Canvas canvas)
        {
            if (ConstantsClass.currentProject.CurrentSkin.isSlotDrawable(this))
            {
                var image = new Image
                {
                    Source = new Avalonia.Media.Imaging.Bitmap(
                        ConstantsClass.currentProject.CurrentSkin.GetImagePath(this)
                    ),
                    Width = lengthX,
                    Height = lengthY,
                    RenderTransform = new RotateTransform(this.a + this.parentA),
                };

                Canvas.SetLeft(image, canvas.Width / 2 + this.x - image.Width / 2);
                Canvas.SetTop(image, canvas.Height / 2 + this.y - image.Height / 2);

                canvas.Children.Add(image);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SlotData generateJSONData()
        {
            return new SlotData
            {
                Name = this.Name,
                Bone = this.BoundedBone?.Name,
                Attachment = this.Name,
            };
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), ConstantsClass.jsonSettings);
        }

        public static Slot regenerate(string json, string imagesFolder = "")
        {
            try
            {
                var data = JsonConvert.DeserializeObject<SlotData>(
                    json,
                    ConstantsClass.jsonSettings
                );

                if (data == null)
                    return new Slot(0, "default.png");

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

                var slot = new Slot(0, imagePath) { Name = data.Name };

                return slot;
            }
            catch
            {
                return new Slot(0, "default.png");
            }
        }

        public void SetName(string? name)
        {
            if (name != null)
            {
                this.Name = name;
            }
        }

        public string GetName
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

        [JsonProperty("attachment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public required string Attachment { get; set; }
    }
}
