using System;

namespace Interpolations
{
    public class Interpolation
    {
        /// <summary>
        /// Interpolates coords
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns>Interpolated coord</returns>
        public static double linearInterpolation(double start, double end, double t)
        {
            return start + t * (end - start);
        }

        /// <summary>
        /// Intepolates angles
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns>Interpolated angle</returns>
        public static double angleInterpolation(double start, double end, double t)
        {
            return start + ((end - start + 540) % 360 - 180) * t;
        }

        /// <summary>
        /// Finds intepolation parameter t
        /// </summary>
        /// <param name="segmentDuration"></param>
        /// <param name="timeElapsed"></param>
        /// <returns>Parameter t</returns>
        public static double findInterpolateParam(double segmentDuration, double timeElapsed)
        {
            double t;
            if (segmentDuration > 0)
            {
                t = timeElapsed / segmentDuration;
            }
            else
            {
                t = 1.0;
            }

            return Math.Clamp(t, 0.0, 1.0);
        }
    }
}
