using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;

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
        public Bone? BoundedBone { get; set; } = null;

        public Slot(string title, int id, string path)
        {
            Title = title;
            Path = path;
            Id = id;
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
                };

                Canvas.SetLeft(image, canvas.Height / 2 + this.BoundedBone.x);
                Canvas.SetTop(image, canvas.Width / 2 + this.BoundedBone.y);

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
