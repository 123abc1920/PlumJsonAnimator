using System;
using Avalonia;
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

        private static readonly IPen BluePen = new Pen(Brushes.Blue, 2);
        private static readonly Typeface DefaultTypeface = new Typeface(FontFamily.Default);
        private static readonly IBrush PointFillBrush = new SolidColorBrush(Colors.Red, 0.6);
        private static readonly IPen PointStrokePen = new Pen(
            new SolidColorBrush(Colors.Black, 0.6),
            1
        );

        public CaptureArea(int x, int y, int width, int height, AppSettings appSettings)
        {
            _a = new PointModel(x, y);
            _b = new PointModel(x + width, y);
            _c = new PointModel(x + width, y + height);
            _d = new PointModel(x, y + height);

            _points = new PointModel[4] { _a, _b, _c, _d };

            _appSettings = appSettings;

            ValidatePoints();
        }

        /// <summary>
        /// Validates all capture area corner points
        /// </summary>
        private void ValidatePoints()
        {
            foreach (PointModel point in _points)
            {
                point.X = Math.Max(0, Math.Min(GlobalState.BaseCanvasSize, point.X));
                point.Y = Math.Max(0, Math.Min(GlobalState.BaseCanvasSize, point.Y));
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
                point.X = Math.Max(0, Math.Min(GlobalState.BaseCanvasSize, point.X));
                point.Y = Math.Max(0, Math.Min(GlobalState.BaseCanvasSize, point.Y));
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
                    _selectedPoint = p;
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
            if (_selectedPoint != null)
            {
                var oldx = _selectedPoint.X;
                var oldy = _selectedPoint.Y;

                _selectedPoint.X = x;
                _selectedPoint.Y = y;

                foreach (PointModel p in _points)
                {
                    if (p.X == oldx)
                    {
                        p.X = _selectedPoint.X;
                    }
                    if (p.Y == oldy)
                    {
                        p.Y = _selectedPoint.Y;
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
            _selectedPoint = null;

            ValidatePoints();

            _appSettings.SetCaptureArea(GetRect());
        }

        /// <summary>
        /// Returns bounds of capture area
        /// </summary>
        /// <returns>Bound rect</returns>
        public Rect GetRect()
        {
            var width = Math.Abs(_b.X - _a.X);
            var height = Math.Abs(_d.Y - _a.Y);

            if (width % 2 != 0)
            {
                width++;
            }
            if (height % 2 != 0)
            {
                height++;
            }

            return new Rect(Math.Floor((double)_a.X), Math.Floor((double)_a.Y), width, height);
        }

        public void DrawCaptureArea(DrawingContext context)
        {
            var rect = GetRect();

            context.DrawRectangle(null, BluePen, new Rect(rect.X, rect.Y, rect.Width, rect.Height));

            var width = _b.X - _a.X;
            var height = _d.Y - _a.Y;
            var formattedText = new FormattedText(
                $"{width:F0} x {height:F0}",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                DefaultTypeface,
                10,
                Brushes.Black
            );

            var textBgRect = new Rect(
                rect.X,
                rect.Y,
                formattedText.Width + 4,
                formattedText.Height + 4
            );
            context.FillRectangle(Brushes.White, textBgRect);
            context.DrawText(formattedText, new Point(rect.X + 2, rect.Y + 2));

            DrawPoint(context, _a.X, _a.Y);
            DrawPoint(context, _b.X, _b.Y);
            DrawPoint(context, _c.X, _c.Y);
            DrawPoint(context, _d.X, _d.Y);
        }

        /// <summary>
        /// Draws corner point and its bounds
        /// </summary>
        /// <param name="context">Canvas for drawing</param>
        /// <param name="x">Point x coordinate</param>
        /// <param name="y">Point y coordinate</param>
        private void DrawPoint(DrawingContext context, double x, double y)
        {
            context.DrawEllipse(PointFillBrush, PointStrokePen, new Point(x, y), 10, 10);
        }
    }
}
