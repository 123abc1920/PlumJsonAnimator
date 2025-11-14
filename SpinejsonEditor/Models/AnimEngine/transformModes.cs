using System;
using AnimModels;

namespace TransformModes
{
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

    public interface Mode
    {
        void transform(IBone bone, double a, double b);
    }

    class NoMode : Mode
    {
        public void transform(IBone bone, double x, double y)
        {
            return;
        }
    }

    class TransformMode : Mode
    {
        public TransformMode() { }

        public void transform(IBone bone, double x, double y)
        {
            bone.move(x, y);
        }
    }

    class ScaleMode : Mode
    {
        public ScaleMode() { }

        public void transform(IBone bone, double x, double y)
        {
            bone.scale(x, y);
        }
    }

    class RotateMode : Mode
    {
        public RotateMode() { }

        public void transform(IBone bone, double x, double y)
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
