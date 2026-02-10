using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AnimConverters
{
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value is double doubleValue)
            {
                // Ограничиваем до 2 знаков после запятой
                return doubleValue.ToString("0.##", culture);
            }
            else if (value is decimal decimalValue)
            {
                // Ограничиваем до 2 знаков после запятой
                return decimalValue.ToString("0.##", culture);
            }
            else if (value is float floatValue)
            {
                // Ограничиваем до 2 знаков после запятой
                return floatValue.ToString("0.##", culture);
            }

            return value.ToString() ?? string.Empty;
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
                // При конвертации обратно также ограничиваем до 2 знаков после запятой
                if (targetType == typeof(double))
                {
                    return Math.Round(result, 2);
                }
                else if (targetType == typeof(decimal))
                {
                    return Math.Round((decimal)result, 2);
                }
                else if (targetType == typeof(float))
                {
                    return (float)Math.Round(result, 2);
                }

                return result;
            }

            return value;
        }
    }
}
