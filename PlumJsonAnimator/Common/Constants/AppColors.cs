using Avalonia.Media;

namespace PlumJsonAnimator.Common.Constants
{
    public class AppColors
    {
        private static string _appColor = "#ff003b";
        public static Color AppColor
        {
            get { return Color.Parse(_appColor); }
        }

        public static IImmutableBrush Red = Brushes.Red;
        public static IImmutableBrush Green = Brushes.Green;
        public static IImmutableBrush Blue = Brushes.Blue;
        public static IImmutableBrush Aqua = Brushes.Aqua;
    }
}
