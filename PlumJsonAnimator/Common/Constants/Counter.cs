using System;

namespace PlumJsonAnimator.Common.Constants
{
    public class Counter()
    {
        private static Random _random = new Random();

        public static string GenerateName()
        {
            return $"Slot_{DateTime.Now.Ticks}_{_random.Next(1000, 9999)}";
        }
    }
}
