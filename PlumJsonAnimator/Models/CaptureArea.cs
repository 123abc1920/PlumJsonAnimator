using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace PlumJsonAnimator.Models
{
    public class CaptureArea
    {
        private class Point
        {
            public int x;
            public int y;

            public Point(int _x, int _y)
            {
                this.x = _x;
                this.y = _y;
            }
        }

        private Point a;
        private Point b;
        private Point c;
        private Point d;

        private Point[] points;

        private Point? selectedPoint = null;

        private const int NEAR_REGION = 20;

        public CaptureArea(int x, int y, int width, int height)
        {
            this.a = new Point(x, y);
            this.b = new Point(x + width, y);
            this.c = new Point(x + width, y + height);
            this.d = new Point(x, y + height);

            points = new Point[4] { this.a, this.b, this.c, this.d };
        }

        public void SelectPoint(int x, int y)
        {
            foreach (Point p in points)
            {
                if (Math.Abs(p.x - x) < NEAR_REGION && Math.Abs(p.y - y) < NEAR_REGION)
                {
                    this.selectedPoint = p;
                }
            }
        }

        public void MovePoint(int x, int y)
        {
            if (this.selectedPoint != null)
            {
                var oldx = this.selectedPoint.x;
                var oldy = this.selectedPoint.y;

                this.selectedPoint.x = x;
                this.selectedPoint.y = y;

                foreach (Point p in points)
                {
                    if (p.x == oldx)
                    {
                        p.x = this.selectedPoint.x;
                    }
                    if (p.y == oldy)
                    {
                        p.y = this.selectedPoint.y;
                    }
                }
            }
        }

        public void UnSelectPoint()
        {
            this.selectedPoint = null;
        }

        public Rect GetRect()
        {
            var width = Math.Abs(this.b.x - this.a.x);
            var height = Math.Abs(this.d.y - this.a.y);

            if (width % 2 != 0)
            {
                width++;
            }
            if (height % 2 != 0)
            {
                height++;
            }

            return new Rect(this.a.x, this.a.y, width, height);
        }

        public void Draw(Canvas canvas)
        {
            Rect rect = GetRect();

            var rectangle = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Blue),
                BorderThickness = new Thickness(2),
                Width = rect.Width,
                Height = rect.Height,
            };

            Canvas.SetLeft(rectangle, rect.X);
            Canvas.SetTop(rectangle, rect.Y);
            canvas.Children.Add(rectangle);

            DrawPoint(canvas, a.x, a.y);
            DrawPoint(canvas, b.x, b.y);
            DrawPoint(canvas, c.x, c.y);
            DrawPoint(canvas, d.x, d.y);
        }

        private void DrawPoint(Canvas canvas, double x, double y)
        {
            var point = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Red),
                Stroke = new SolidColorBrush(Colors.Black),
                Width = 20,
                Height = 20,
                StrokeThickness = 1,
                Opacity = 0.6,
            };

            Canvas.SetLeft(point, x - 10);
            Canvas.SetTop(point, y - 10);

            canvas.Children.Add(point);
        }
    }
}
