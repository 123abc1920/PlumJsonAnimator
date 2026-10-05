using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using PlumJsonAnimator.Models.Common;

namespace PlumJsonAnimator.Common.Converters;

public class EasingTypeToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EasingTypes current && parameter is string param)
        {
            return current.ToString().Equals(param, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is true && parameter is string param)
        {
            return param switch
            {
                "linear" => EasingTypes.LINEAR,
                "stepped" => EasingTypes.STEPPED,
                "bezier" => EasingTypes.BEZIER,
                _ => BindingOperations.DoNothing,
            };
        }

        return BindingOperations.DoNothing;
    }
}
