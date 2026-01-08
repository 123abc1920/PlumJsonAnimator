using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;
using Newtonsoft.Json;

namespace AnimModels
{
    public class Slot : IBone, INotifyPropertyChanged
    {
        private string _title = "";
        private string _path = "";

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }
        public int DrawOrder { get; set; }
        public double selfA = 0;
        public double lengthX = 100;
        public double lengthY = 100;
        public Bone? BoundedBone { get; set; } = null;

        public Slot(int id, string path)
        {
            Path = path;
            this.id = id;
            this.a = 0;
            this.x = 0;
            this.y = 0;

            Title = System.IO.Path.GetFileNameWithoutExtension(path);
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
            this.selfA = a;
        }

        public void drawSlot(Canvas canvas)
        {
            if (this.BoundedBone != null)
            {
                var image = new Image
                {
                    Source = new Avalonia.Media.Imaging.Bitmap(this.Path),
                    Width = lengthX,
                    Height = lengthY,
                    RenderTransform = new RotateTransform(this.a + this.selfA),
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
                Name = this.Title,
                Bone = this.BoundedBone?.name,
                Attachment = this.Title,
            };
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
        }
    }
}

public class SlotData
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("bone", NullValueHandling = NullValueHandling.Ignore)]
    public string Bone { get; set; }

    [JsonProperty("attachment", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public required string Attachment { get; set; }
}
