namespace Interpolations
{
    public class Interpolation
    {
        public static double linearInterpolation(double start, double end, double t)
        {
            return start + t * (end - start);
        }

        public static double angleInterpolation(double start, double end, double t)
        {
            return start + ((end - start + 540) % 360 - 180) * t;
        }
    }
}
