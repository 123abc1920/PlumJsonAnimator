using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AnimModels
{
    public class Slot : INotifyPropertyChanged
    {
        private string _title;
        private string _path;

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

        public int Id { get; set; }
        public int DrawOrder { get; set; }
        public double x = 0;
        public double y = 0;
        public double selfA = 0;
        public double a = 0;
        public double lengthX = 100;
        public double lengthY = 100;
        public Bone? BoundedBone { get; set; } = null;

        public Slot(string title, int id, string path)
        {
            Title = title;
            Path = path;
            Id = id;
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

        public void move(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void scale(double x, double y)
        {
            Console.WriteLine("scale");
        }

        public void rotate(double a)
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
                    Width = 100,
                    Height = 100,
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
    }
}
