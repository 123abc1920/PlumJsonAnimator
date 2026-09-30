using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Easing;

class LinearEasing : IEasing
{
    public double InInterpolation(double t)
    {
        return t;
    }

    public double InOutInterpolation(double t)
    {
        return t;
    }

    public double OutInterpolation(double t)
    {
        return t;
    }
}
