using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;

// TODO: validate points
// TODO: fix freezing when zoom<1
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

        private AppSettings appSettings;
        private GlobalState globalState;

        private const int NEAR_REGION = 20;

        public CaptureArea(
            int x,
            int y,
            int width,
            int height,
            AppSettings appSettings,
            GlobalState globalState
        )
        {
            this.a = new Point(x, y);
            this.b = new Point(x + width, y);
            this.c = new Point(x + width, y + height);
            this.d = new Point(x, y + height);

            points = new Point[4] { this.a, this.b, this.c, this.d };

            this.appSettings = appSettings;
            this.globalState = globalState;
        }

        public void SelectPoint(int x, int y)
        {
            x = (int)(x);
            y = (int)(y);

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
            x = (int)(x);
            y = (int)(y);

            if (x < -this.globalState.canvasWidth / 2)
            {
                x = -this.globalState.canvasWidth / 2;
            }
            if (y < -this.globalState.canvasHeight / 2)
            {
                y = -this.globalState.canvasHeight / 2;
            }

            if (x >= this.globalState.canvasWidth / 2)
            {
                x = this.globalState.canvasWidth / 2 - 1;
            }
            if (y >= this.globalState.canvasHeight / 2)
            {
                y = this.globalState.canvasHeight / 2 - 1;
            }

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

            this.appSettings.SetCaptureArea(GetRect());
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

            return new Rect(
                Math.Floor((double)this.a.x),
                Math.Floor((double)this.a.y),
                width,
                height
            );
        }

        public void Draw(Canvas canvas)
        {
            var centerX = canvas.Width / 2;
            var centerY = canvas.Height / 2;

            var rect = new Rect(
                centerX + this.a.x,
                centerY + this.a.y,
                Math.Abs(this.b.x - this.a.x),
                Math.Abs(this.d.y - this.a.y)
            );

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

            DrawPoint(canvas, centerX + this.a.x, centerY + this.a.y);
            DrawPoint(canvas, centerX + this.b.x, centerY + this.b.y);
            DrawPoint(canvas, centerX + this.c.x, centerY + this.c.y);
            DrawPoint(canvas, centerX + this.d.x, centerY + this.d.y);
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
