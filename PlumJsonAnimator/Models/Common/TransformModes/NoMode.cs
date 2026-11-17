using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common.TransformModes;

class NoMode : Mode
{
    public NoMode(GlobalState globalState)
        : base(globalState)
    {
        Type = TransformModeTypes.NO;
        Name = "";
    }

    public override void ClearMode() { }

    public override void Transform(Bone bone, double a, double b, IEasing easing)
    {
        return;
    }
}
