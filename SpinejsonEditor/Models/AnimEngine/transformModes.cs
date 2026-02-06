using System;
using AnimModels;

namespace TransformModes
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

    class Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public abstract class Mode
    {
        public TransformModesTypes type;

        public abstract void transform(IBone bone, double a, double b);
    }

    class NoMode : Mode
    {
        public NoMode()
        {
            type = TransformModesTypes.NO;
        }

        public override void transform(IBone bone, double a, double b)
        {
            return;
        }
    }

    class TransformMode : Mode
    {
        public TransformMode()
        {
            type = TransformModesTypes.TRANSLATE;
        }

        public override void transform(IBone bone, double x, double y)
        {
            bone.move(x, y);
        }
    }

    class ScaleMode : Mode
    {
        public ScaleMode()
        {
            type = TransformModesTypes.SCALE;
        }

        public override void transform(IBone bone, double x, double y)
        {
            bone.scale(x, y);
        }
    }

    class RotateMode : Mode
    {
        public RotateMode()
        {
            type = TransformModesTypes.ROTATE;
        }

        public override void transform(IBone bone, double x, double y)
        {
            double xx = x - bone.x;
            Point av = new Point(xx, y - bone.y);
            Point bv = new Point(10, 0);

            double dot = av.x * bv.x + av.y * bv.y;
            double det = av.x * bv.y - av.y * bv.x;
            double angleRad = Math.Atan2(det, dot);
            double angleDeg = angleRad * 180 / Math.PI;

            bone.rotate(-angleDeg);
        }
    }
}
