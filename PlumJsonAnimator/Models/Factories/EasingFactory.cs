using System.Collections.Generic;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Easing;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Factories;

public class EasingFactory
{
    public IEasing CreateEasing(EasingTypes easingType)
    {
        switch (easingType)
        {
            case EasingTypes.STEPPED:
                return new SteppedEasing();
            case EasingTypes.LINEAR:
                return new LinearEasing();
            case EasingTypes.BEZIER:
                return new BezierEasing(0.0, 0.0, 0.58, 1.0);
            default:
                return new LinearEasing();
        }
    }
}
