using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Easing;

class QuadroEasing : IEasing
{
    public double InInterpolation(double t)
    {
        return t * t;
    }

    public double OutInterpolation(double t)
    {
        return t * (2 - t);
    }

    public double InOutInterpolation(double t)
    {
        if (t < 0.5)
        {
            return 2 * t * t;
        }
        else
        {
            return 1 - 2 * (1 - t) * (1 - t);
        }
    }
}
