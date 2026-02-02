using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AnimConverters
{
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            string stringValue = value as string;

            if (
                string.IsNullOrEmpty(stringValue)
                || stringValue == "-"
                || stringValue == "."
                || stringValue == ","
            )
            {
                return 0;
            }

            string normalized = stringValue.Replace(',', '.');

            if (
                double.TryParse(
                    normalized,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double result
                )
            )
            {
                return result;
            }

            return value;
        }
    }
}
