using System.Collections.Generic;

namespace PlumJsonAnimator.Models.Interfaces;

public interface IEasing
{
    List<double> KeysMap { get; }

    double Ease(double t);
}
