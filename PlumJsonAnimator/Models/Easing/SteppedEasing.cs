using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Easing;

class SteppedEasing : IEasing
{
    private List<double> _keysMap = new List<double>();
    public List<double> KeysMap => _keysMap;

    public void DrawLine(DrawingContext context, PointModel p1, PointModel p2, Brush brush)
    {
        context.DrawLine(new Pen(brush, 1), new Point(p1.X, p1.Y), new Point(p2.X, p1.Y));
        context.DrawLine(new Pen(brush, 1), new Point(p2.X, p1.Y), new Point(p2.X, p2.Y));
    }

    public double Ease(double t)
    {
        return t < 1.0 ? 0.0 : 1.0;
    }
}
