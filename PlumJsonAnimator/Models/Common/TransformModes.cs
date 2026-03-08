using System;

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

    public class TransformModeFactory
    {
        private static Mode[] modes =
        {
            new NoMode(),
            new TransformMode(),
            new RotateMode(),
            new ScaleMode(),
        };

        public static Mode createMode(Mode old, TransformModesTypes type)
        {
            if (old.type == type)
            {
                return new NoMode();
            }
            else
            {
                return modes[(int)type];
            }
        }
    }

    public abstract class Mode
    {
        public TransformModesTypes type;
        public string name = "";

        public abstract void transform(Bone bone, double a, double b);
    }

    class NoMode : Mode
    {
        public NoMode()
        {
            type = TransformModesTypes.NO;
            name = "";
        }

        public override void transform(Bone bone, double a, double b)
        {
            return;
        }
    }

    class TransformMode : Mode
    {
        public TransformMode()
        {
            type = TransformModesTypes.TRANSLATE;
            name = "transform";
        }

        public override void transform(Bone bone, double x, double y)
        {
            bone.move(x, y);
            var animation = ConstantsClass.currentProject?.GetAnimation();
            if (animation != null && !animation.IsRun)
            {
                animation.TranslateBone(bone, bone.x, bone.y);
            }
        }
    }

    class ScaleMode : Mode
    {
        public ScaleMode()
        {
            type = TransformModesTypes.SCALE;
            name = "scale";
        }

        public override void transform(Bone bone, double x, double y)
        {
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

        public RotateMode()
        {
            type = TransformModesTypes.ROTATE;
            name = "rotate";
        }

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

            var animation = ConstantsClass.currentProject?.GetAnimation();
            if (animation != null && !animation.IsRun)
            {
                animation.RotateBone(bone, bone.a);
            }
        }
    }
}
