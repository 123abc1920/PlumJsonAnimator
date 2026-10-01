using System.Collections.Generic;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Easing;

class SteppedEasing : IEasing
{
    private List<double> _keysMap = new List<double>();
    public List<double> KeysMap => _keysMap;

    public double Ease(double t)
    {
        return t < 1.0 ? 0.0 : 1.0;
    }
}
