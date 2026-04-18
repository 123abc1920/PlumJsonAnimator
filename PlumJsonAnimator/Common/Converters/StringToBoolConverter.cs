using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PlumJsonAnimator.Common.Converters
{
    /// <summary>
    /// Converter for toggle transform modes buttons
    /// </summary>
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string currentMode && parameter is string targetMode)
            {
                return currentMode == targetMode;
            }
            return false;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            return value;
        }
    }
}
