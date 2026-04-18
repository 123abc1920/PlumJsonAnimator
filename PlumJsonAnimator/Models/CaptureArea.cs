using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models
{
    /// <summary>
    /// Provides methods for work with capture area. This area renders into exported images, gifs, videos and other images.
    /// </summary>
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

        private Point _a;
        private Point _b;
        private Point _c;
        private Point _d;

        private Point[] _points;
        private Point? _selectedPoint = null;

        private AppSettings _appSettings;

        private const int NEAR_REGION = 20;

        public CaptureArea(int x, int y, int width, int height, AppSettings appSettings)
        {
            this._a = new Point(x, y);
            this._b = new Point(x + width, y);
            this._c = new Point(x + width, y + height);
            this._d = new Point(x, y + height);

            _points = new Point[4] { this._a, this._b, this._c, this._d };

            this._appSettings = appSettings;

            ValidatePoints();
        }

        /// <summary>
        /// Validates all capture area corner points
        /// </summary>
        private void ValidatePoints()
        {
            foreach (Point point in _points)
            {
                point.x = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.x));
                point.y = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.y));
            }

            int minWidth = NEAR_REGION * 2;
            int minHeight = NEAR_REGION * 2;

            int left = Math.Min(_a.x, Math.Min(_b.x, Math.Min(_c.x, _d.x)));
            int top = Math.Min(_a.y, Math.Min(_b.y, Math.Min(_c.y, _d.y)));
            int right = Math.Max(_a.x, Math.Max(_b.x, Math.Max(_c.x, _d.x)));
            int bottom = Math.Max(_a.y, Math.Max(_b.y, Math.Max(_c.y, _d.y)));

            if (right - left < minWidth)
            {
                right = left + minWidth;
            }
            if (bottom - top < minHeight)
            {
                bottom = top + minHeight;
            }

            _a.x = left;
            _a.y = top;

            _b.x = right;
            _b.y = top;

            _c.x = right;
            _c.y = bottom;

            _d.x = left;
            _d.y = bottom;

            foreach (Point point in _points)
            {
                point.x = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.x));
                point.y = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.y));
            }
        }

        /// <summary>
        /// Selects point near the click location
        /// </summary>
        /// <param name="x">X click coordinate</param>
        /// <param name="y">Y click coordinate</param>
        public void SelectPoint(int x, int y)
        {
            double realX = x;
            double realY = y;

            foreach (Point p in _points)
            {
                double dx = Math.Abs(p.x - realX);
                double dy = Math.Abs(p.y - realY);

                if (dx < NEAR_REGION && dy < NEAR_REGION)
                {
                    this._selectedPoint = p;
                    break;
                }
            }
        }

        /// <summary>
        /// Moves the selected point into new location
        /// </summary>
        /// <param name="x">X click coordinate</param>
        /// <param name="y">Y click coordinate</param>
        public void MoveSelectedPoint(int x, int y)
        {
            if (this._selectedPoint != null)
            {
                var oldx = this._selectedPoint.x;
                var oldy = this._selectedPoint.y;

                this._selectedPoint.x = x;
                this._selectedPoint.y = y;

                foreach (Point p in _points)
                {
                    if (p.x == oldx)
                    {
                        p.x = this._selectedPoint.x;
                    }
                    if (p.y == oldy)
                    {
                        p.y = this._selectedPoint.y;
                    }
                }
            }

            ValidatePoints();
        }

        /// <summary>
        /// Unselects point and saves capture area into app settings file
        /// </summary>
        public void UnSelectPoint()
        {
            this._selectedPoint = null;

            ValidatePoints();

            this._appSettings.SetCaptureArea(GetRect());
        }

        /// <summary>
        /// Returns bounds of capture area
        /// </summary>
        /// <returns>Bound rect</returns>
        public Rect GetRect()
        {
            var width = Math.Abs(this._b.x - this._a.x);
            var height = Math.Abs(this._d.y - this._a.y);

            if (width % 2 != 0)
            {
                width++;
            }
            if (height % 2 != 0)
            {
                height++;
            }

            return new Rect(
                Math.Floor((double)this._a.x),
                Math.Floor((double)this._a.y),
                width,
                height
            );
        }

        public void DrawCaptureArea(Canvas canvas)
        {
            var rect = GetRect();

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

            var width = this._b.x - this._a.x;
            var height = this._d.y - this._a.y;
            var sizeText = new TextBlock
            {
                Text = $"{width:F0} x {height:F0}",
                FontSize = 10,
                Foreground = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Padding = new Thickness(2),
            };

            Canvas.SetLeft(sizeText, rect.X);
            Canvas.SetTop(sizeText, rect.Y);
            canvas.Children.Add(sizeText);

            DrawPoint(canvas, this._a.x, this._a.y);
            DrawPoint(canvas, this._b.x, this._b.y);
            DrawPoint(canvas, this._c.x, this._c.y);
            DrawPoint(canvas, this._d.x, this._d.y);
        }

        /// <summary>
        /// Draws corner point and its bounds
        /// </summary>
        /// <param name="canvas">Canvas for drawing</param>
        /// <param name="x">Point x coordinate</param>
        /// <param name="y">Point y coordinate</param>
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
