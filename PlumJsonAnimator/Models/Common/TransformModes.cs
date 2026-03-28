using System;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common
{
    public enum TransformModesTypes
    {
        NO = 0,
        TRANSLATE,
        ROTATE,
        SCALE,
        SHEAR,
    }

    public abstract class Mode
    {
        public TransformModesTypes type;
        public string name = "";

        protected GlobalState globalState;

        public Mode(GlobalState globalState)
        {
            this.globalState = globalState;
        }

        public abstract void clearMode();

        public abstract void transform(Bone bone, double a, double b);
    }

    class NoMode : Mode
    {
        public NoMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.NO;
            name = "";
        }

        public override void clearMode() { }

        public override void transform(Bone bone, double a, double b)
        {
            return;
        }
    }

    class TransformMode : Mode
    {
        public TransformMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.TRANSLATE;
            name = "transform";
        }

        public override void clearMode() { }

        public override void transform(Bone bone, double x, double y)
        {
            bone.move(x, y);
            var animation = this.globalState.CurrentProject?.GetAnimation();
            if (animation != null && !animation.IsRun && bone.IsBone == true)
            {
                animation.TranslateBone(bone, bone.x, bone.y);
            }
        }
    }

    class ScaleMode : Mode
    {
        private double? startX = null;
        private double? startY = null;

        public ScaleMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.SCALE;
            name = "scale";
        }

        public override void clearMode()
        {
            startX = null;
            startY = null;
        }

        public override void transform(Bone bone, double x, double y)
        {
            if (startX == null || startY == null)
            {
                startX = x;
                startY = y;
                return;
            }
            double deltaX = (double)startX - x;
            double deltaY = (double)startY - y;
            startX = x;
            startY = y;
            bone.scale(x, y);
        }
    }

    class RotateMode : Mode
    {
        private class Point
        {
            public double x;
            public double y;

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public RotateMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.ROTATE;
            name = "rotate";
        }

        public override void clearMode() { }

        public override void transform(Bone bone, double x, double y)
        {
            double xx = x - bone.x;
            Point av = new Point(xx, y - bone.y);
            Point bv = new Point(10, 0);

            double dot = av.x * bv.x + av.y * bv.y;
            double det = av.x * bv.y - av.y * bv.x;
            double angleRad = Math.Atan2(det, dot);
            double angleDeg = angleRad * 180 / Math.PI;

            bone.rotate(-angleDeg);

            var animation = this.globalState.CurrentProject?.GetAnimation();
            if (animation != null && !animation.IsRun && bone.IsBone == true)
            {
                animation.RotateBone(bone, bone.a);
            }
        }
    }
}
