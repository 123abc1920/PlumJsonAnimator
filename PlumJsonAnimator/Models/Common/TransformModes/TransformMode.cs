using System;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common.TransformModes;

class TransformMode : Mode
{
    public TransformMode(GlobalState globalState)
        : base(globalState)
    {
        Type = TransformModeTypes.TRANSLATE;
        Name = "transform";
    }

    public override void ClearMode() { }

    public override void Transform(Bone bone, double x, double y, IEasing easing)
    {
        bone.Move(x, y);
        if (_globalState.SetBasePos == false)
        {
            var animation = _globalState.CurrentProject?.GetCurrentAnimation();
            if (animation != null && !animation.IsRun)
            {
                animation.TranslateBone(bone, bone.X, bone.Y, easing);
            }
        }
    }
}
