namespace Interpolations
{
    public class Interpolation
    {
        public static double linearInterpolation(double start, double end)
        {
            return start + 0.5 * (end -start);
        }
    }
}