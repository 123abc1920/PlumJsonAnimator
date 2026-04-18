using System;

namespace PlumJsonAnimator.Common.Constants
{
    /// <summary>
    /// Generates unique names
    /// </summary>
    public class Counter()
    {
        private static Random _random = new Random();

        /// <summary>
        /// Genearate postfix that can be joined with the common name
        /// </summary>
        /// <returns>Postfix string</returns>
        public static string GenerateNamePostfix()
        {
            return $"_{DateTime.Now.Ticks}_{_random.Next(1000, 9999)}";
        }
    }
}
