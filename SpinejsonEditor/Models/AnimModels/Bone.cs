using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Constants;

namespace AnimModels
{
    public class Bone
    {
        public string name = "root";
        public int id = 0;
        public double x = 100;
        public double y = 100;
        public double a = 0;
        public List<Bone> children = new List<Bone>();
        private double endX = 110;
        private double endY = 110;
        public double length = 10;

        public Bone()
        {
            this.a = -100;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);
        }

        public Bone(int _id)
        {
            this.id = _id;
            this.name = "name" + this.id.ToString();
        }

        public void addChildren(Bone bone)
        {
            this.children.Add(bone);
        }

        public void move(double x, double y)
        {
            double deltaX = this.x - x;
            double deltaY = this.y - y;

            this.x = x;
            this.y = y;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);

            foreach (Bone c in this.children)
            {
                c.move(c.x - deltaX, c.y - deltaY);
            }
        }

        public void scale(double x, double y)
        {
            Console.WriteLine("scale");
        }

        public void rotate(double a)
        {
            double oldA = this.a;
            this.a = a;
            double angleRad = this.a * Math.PI / 180;

            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);

            foreach (Bone child in this.children)
            {
                double dx = child.x - this.x;
                double dy = child.y - this.y;

                double angleDiff = (a - oldA) * Math.PI / 180;
                double newDx = dx * Math.Cos(angleDiff) - dy * Math.Sin(angleDiff);
                double newDy = dx * Math.Sin(angleDiff) + dy * Math.Cos(angleDiff);

                child.x = this.x + newDx;
                child.y = this.y + newDy;

                child.rotate(child.a + (a - oldA));
            }
        }

        public void drawBone(Canvas canvas)
        {
            Point start = new Point(this.x, this.y);
            Point end = new Point(this.endX, this.endY);

            var line = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = Constants.Color.getLineBoneColor(this.id),
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Constants.Color.getDotBoneColor(this.id),
            };

            Canvas.SetLeft(joint, start.X - 4);
            Canvas.SetTop(joint, start.Y - 4);

            canvas.Children.Add(line);
            canvas.Children.Add(joint);
        }
    }
}
