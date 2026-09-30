using System;

namespace PlumJsonAnimator.Models.Interfaces;

public interface IEasing
{
    double InInterpolation(double t);
    double OutInterpolation(double t);
    double InOutInterpolation(double t);

    double FindParametrT(double t, bool isIn = false, bool isOut = false)
    {
        if (isIn && isOut)
        {
            t = InOutInterpolation(t);
        }
        else if (isIn && !isOut)
        {
            t = InInterpolation(t);
        }
        else if (isOut && !isIn)
        {
            t = OutInterpolation(t);
        }

        t = Math.Clamp(t, 0.0, 1.0);
        return t;
    }
}
