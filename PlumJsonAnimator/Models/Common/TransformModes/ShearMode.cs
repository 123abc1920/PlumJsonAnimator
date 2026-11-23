using System;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common.TransformModes;

class ShearMode : Mode
{
    private double? startX = null;
    private double? startY = null;
    private double startShearX = 0;
    private double startShearY = 0;
    private bool isHorizontalLocked = false;
    private bool isVerticalLocked = false;

    private const double Sensitivity = 0.5;
    private const double LockThreshold = 5.0;

    public ShearMode(GlobalState globalState)
        : base(globalState)
    {
        Type = TransformModeTypes.SHEAR;
        Name = "shear";
    }

    public override void ClearMode()
    {
        startX = null;
        startY = null;
        isHorizontalLocked = false;
        isVerticalLocked = false;
    }

    public override void Transform(IAnimatable item, double x, double y, IEasing easing)
    {
        if (startX == null || startY == null)
        {
            startX = x;
            startY = y;
            startShearX = item.ShearX;
            startShearY = item.ShearY;
            return;
        }

        double deltaX = x - startX.Value;
        double deltaY = y - startY.Value;

        if (!isHorizontalLocked && !isVerticalLocked)
        {
            if (Math.Abs(deltaX) > LockThreshold)
            {
                isHorizontalLocked = true;
            }
            else if (Math.Abs(deltaY) > LockThreshold)
            {
                isVerticalLocked = true;
            }
        }

        double newShearX = startShearX;
        double newShearY = startShearY;

        if (isHorizontalLocked)
        {
            newShearY = startShearY + deltaX * Sensitivity;
        }
        else if (isVerticalLocked)
        {
            newShearX = startShearX + deltaY * Sensitivity;
        }

        newShearX = Math.Clamp(newShearX, -89.0, 89.0);
        newShearY = Math.Clamp(newShearY, -89.0, 89.0);

        item.Shear(newShearX, newShearY);

        if (item is Bone bone)
        {
            if (_globalState.SetBasePos == false)
            {
                var animation = _globalState.CurrentProject?.GetCurrentAnimation();
                if (animation != null && !animation.IsRun)
                {
                    animation.ShearBone(bone, bone.ShearX, bone.ShearY, easing);
                }
            }
        }
    }
}
