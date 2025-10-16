using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace AnimModels
{
    public class Bone
    {
        public string name = "root";
        public int id = 0;
        public int x = 100;
        public int y = 100;

        public Bone() { }

        public Bone(int _id)
        {
            this.id = _id;
            this.name = "name" + this.id.ToString();
        }

        public void drawBone(Canvas canvas)
        {
            Point start = new Point(this.x - 5, this.y - 5);
            Point end = new Point(this.x + 5, this.y + 5);

            var line = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = Brushes.Red,
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Blue,
            };

            Canvas.SetLeft(joint, start.X - 4);
            Canvas.SetTop(joint, start.Y - 4);

            canvas.Children.Add(line);
            canvas.Children.Add(joint);
        }
    }
}
