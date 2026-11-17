using System;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common.TransformModes;

class RotateMode : Mode
{
    private class Point
    {
        public double _x;
        public double _y;

        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }
    }

    public RotateMode(GlobalState globalState)
        : base(globalState)
    {
        Type = TransformModeTypes.ROTATE;
        Name = "rotate";
    }

    public override void ClearMode() { }

    public override void Transform(Bone bone, double x, double y, IEasing easing)
    {
        double xx = x - bone.X;
        Point av = new Point(xx, y - bone.Y);
        Point bv = new Point(10, 0);

        double dot = av._x * bv._x + av._y * bv._y;
        double det = av._x * bv._y - av._y * bv._x;
        double angleRad = Math.Atan2(det, dot);
        double angleDeg = angleRad * 180 / Math.PI;

        bone.Rotate(-angleDeg);
        if (_globalState.SetBasePos == false)
        {
            var animation = _globalState.CurrentProject?.GetCurrentAnimation();
            if (animation != null && !animation.IsRun)
            {
                animation.RotateBone(bone, bone.A, easing);
            }
        }
    }
}
