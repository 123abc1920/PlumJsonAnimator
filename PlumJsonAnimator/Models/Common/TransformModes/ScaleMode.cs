using System;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common.TransformModes;

class ScaleMode : Mode
{
    private double? startX = null;
    private double? startY = null;

    public ScaleMode(GlobalState globalState)
        : base(globalState)
    {
        Type = TransformModeTypes.SCALE;
        Name = "scale";
    }

    public override void ClearMode()
    {
        startX = null;
        startY = null;
    }

    public override void Transform(Bone bone, double x, double y, IEasing easing)
    {
        if (startX == null || startY == null)
        {
            startX = x;
            startY = y;
            return;
        }
        var dx = 0.0;
        var dy = 0.0;
        if (Math.Abs(x - (double)startX) > Math.Abs(y - (double)startY))
        {
            if (x - (double)startX > 0)
            {
                dx = 0.01;
            }
            else
            {
                dx = -0.01;
            }
        }
        else
        {
            if (y - (double)startY > 0)
            {
                dy = 0.01;
            }
            else
            {
                dy = -0.01;
            }
        }

        bone.Scale(bone.ScaleX + dx, bone.ScaleY + dy);
        startX = x;
        startY = y;

        if (_globalState.SetBasePos == false)
        {
            var animation = _globalState.CurrentProject?.GetCurrentAnimation();
            if (animation != null && !animation.IsRun)
            {
                animation.ScaleBone(bone, bone.ScaleX, bone.ScaleY, easing);
            }
        }
    }
}