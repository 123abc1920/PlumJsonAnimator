using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models
{
    /// <summary>
    /// Provides methods for work with capture area. This area renders into exported images, gifs, videos and other images.
    /// </summary>
    public class CaptureArea
    {
        private PointModel _a;
        private PointModel _b;
        private PointModel _c;
        private PointModel _d;

        private PointModel[] _points;
        private PointModel? _selectedPoint = null;

        private AppSettings _appSettings;

        private const int NearRegion = 20;

        public CaptureArea(int x, int y, int width, int height, AppSettings appSettings)
        {
            this._a = new PointModel(x, y);
            this._b = new PointModel(x + width, y);
            this._c = new PointModel(x + width, y + height);
            this._d = new PointModel(x, y + height);

            _points = new PointModel[4] { this._a, this._b, this._c, this._d };

            this._appSettings = appSettings;

            ValidatePoints();
        }

        /// <summary>
        /// Validates all capture area corner points
        /// </summary>
        private void ValidatePoints()
        {
            foreach (PointModel point in _points)
            {
                point.X = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.X));
                point.Y = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.Y));
            }

            int minWidth = NearRegion * 2;
            int minHeight = NearRegion * 2;

            int left = Math.Min(_a.X, Math.Min(_b.X, Math.Min(_c.X, _d.X)));
            int top = Math.Min(_a.Y, Math.Min(_b.Y, Math.Min(_c.Y, _d.Y)));
            int right = Math.Max(_a.X, Math.Max(_b.X, Math.Max(_c.X, _d.X)));
            int bottom = Math.Max(_a.Y, Math.Max(_b.Y, Math.Max(_c.Y, _d.Y)));

            if (right - left < minWidth)
            {
                right = left + minWidth;
            }
            if (bottom - top < minHeight)
            {
                bottom = top + minHeight;
            }

            _a.X = left;
            _a.Y = top;

            _b.X = right;
            _b.Y = top;

            _c.X = right;
            _c.Y = bottom;

            _d.X = left;
            _d.Y = bottom;

            foreach (PointModel point in _points)
            {
                point.X = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.X));
                point.Y = Math.Max(0, Math.Min(GlobalState.BASE_CANVAS_SIZE, point.Y));
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

            foreach (PointModel p in _points)
            {
                double dx = Math.Abs(p.X - realX);
                double dy = Math.Abs(p.Y - realY);

                if (dx < NearRegion && dy < NearRegion)
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
                var oldx = this._selectedPoint.X;
                var oldy = this._selectedPoint.Y;

                this._selectedPoint.X = x;
                this._selectedPoint.Y = y;

                foreach (PointModel p in _points)
                {
                    if (p.X == oldx)
                    {
                        p.X = this._selectedPoint.X;
                    }
                    if (p.Y == oldy)
                    {
                        p.Y = this._selectedPoint.Y;
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
            var width = Math.Abs(this._b.X - this._a.X);
            var height = Math.Abs(this._d.Y - this._a.Y);

            if (width % 2 != 0)
            {
                width++;
            }
            if (height % 2 != 0)
            {
                height++;
            }

            return new Rect(
                Math.Floor((double)this._a.X),
                Math.Floor((double)this._a.Y),
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

            var width = this._b.X - this._a.X;
            var height = this._d.Y - this._a.Y;
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

            DrawPoint(canvas, this._a.X, this._a.Y);
            DrawPoint(canvas, this._b.X, this._b.Y);
            DrawPoint(canvas, this._c.X, this._c.Y);
            DrawPoint(canvas, this._d.X, this._d.Y);
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
