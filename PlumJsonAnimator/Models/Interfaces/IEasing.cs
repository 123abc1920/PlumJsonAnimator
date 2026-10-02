using System.Collections.Generic;
using Avalonia.Media;
using PlumJsonAnimator.Models.Common;

namespace PlumJsonAnimator.Models.Interfaces;

public interface IEasing
{
    List<double> KeysMap { get; }

    double Ease(double t);

    void DrawLine(DrawingContext context, PointModel p1, PointModel p2, Brush brush);
}
