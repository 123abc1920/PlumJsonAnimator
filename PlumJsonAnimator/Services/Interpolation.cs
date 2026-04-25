using System;

namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Provides interpolation methods for animations and method for finding interpolation parameter t.
    /// </summary>
    public class Interpolation
    {
        /// <summary>
        /// Linear interpolation between two values
        /// </summary>
        /// <param name="start">Start value at t=0</param>
        /// <param name="end">End value at t=1</param>
        /// <param name="t">Interpolation parameter (0 to 1)</param>
        /// <example>
        /// new Interpolation().linearInterpolation(20, 40, 0.5);
        /// </example>
        public double linearInterpolation(double start, double end, double t)
        {
            return start + t * (end - start);
        }

        /// <summary>
        /// Angle interpolation with shortest path (handles 360° wrap-around)
        /// </summary>
        /// <param name="start">Start angle in degrees</param>
        /// <param name="end">End angle in degrees</param>
        /// <param name="t">Interpolation parameter (0 to 1)</param>
        /// <example>
        /// new Interpolation().angleInterpolation(350, 10, 0.5);
        /// </example>
        public double angleInterpolation(double start, double end, double t)
        {
            return start + ((end - start + 540) % 360 - 180) * t;
        }

        /// <summary>
        /// Calculates interpolation parameter t from elapsed time
        /// </summary>
        /// <param name="segmentDuration">Total duration in ms</param>
        /// <param name="timeElapsed">Time passed in ms</param>
        /// <example>
        /// new Interpolation().findInterpolateParam(10, 6); //Returns 0.6
        /// </example>
        /// <returns>t value clamped between 0 and 1</returns>
        public double findInterpolateParam(double segmentDuration, double timeElapsed)
        {
            double t = segmentDuration > 0 ? timeElapsed / segmentDuration : 1.0;
            return Math.Clamp(t, 0.0, 1.0);
        }
    }
}
