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
        context.DrawLine(new Pen(brush, 1), new Point(p1.X, p1.Y), new Point(p2.X, p2.Y));
    }
}
