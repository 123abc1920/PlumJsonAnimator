using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Easing;

class BezierEasing : IEasing
{
    private List<double> _keysMap = new List<double>();
    public List<double> KeysMap => _keysMap;

    public BezierEasing(params double[] keys)
    {
        foreach (var k in keys)
        {
            _keysMap.Add(k);
        }
    }

    public double Ease(double t)
    {
        double u = t;

        for (int i = 0; i < 5; i++)
        {
            double currentX =
                3.0 * (1.0 - u) * (1.0 - u) * u * KeysMap[0]
                + 3.0 * (1.0 - u) * u * u * KeysMap[2]
                + u * u * u;

            double differencial =
                3.0 * (1.0 - u) * (1.0 - u) * KeysMap[0]
                + 6.0 * (1.0 - u) * u * (KeysMap[2] - KeysMap[0])
                + 3.0 * u * u * (1.0 - KeysMap[2]);

            if (Math.Abs(differencial) < 1e-6)
                break;

            u -= (currentX - t) / differencial;
        }

        double resultedT =
            3.0 * (1.0 - u) * (1.0 - u) * u * KeysMap[1]
            + 3.0 * (1.0 - u) * u * u * KeysMap[3]
            + u * u * u;

        return resultedT;
    }

    public void DrawLine(DrawingContext context, PointModel p1, PointModel p2, Brush brush)
    {
        if (_keysMap != null && _keysMap.Count >= 4)
        {
            double x1 = _keysMap[0];
            double y1 = _keysMap[1];
            double x2 = _keysMap[2];
            double y2 = _keysMap[3];

            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;

            Point controlPoint1 = new Point(p1.X + x1 * deltaX, p1.Y + y1 * deltaY);
            Point controlPoint2 = new Point(p1.X + x2 * deltaX, p1.Y + y2 * deltaY);

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(p1.X, p1.Y), isFilled: false);
                ctx.CubicBezierTo(controlPoint1, controlPoint2, new Point(p2.X, p2.Y));
                ctx.EndFigure(isClosed: false);
            }

            context.DrawGeometry(null, new Pen(brush, 1.5), geometry);
        }
    }
}
