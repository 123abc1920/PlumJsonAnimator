using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AnimConverters
{
    public class AnimationConverter : IValueConverter
    {
        public string PlayIcon
        {
            get => "/Assets/bootstrap_icons/play-fill.svg";
        }
        public string PauseIcon
        {
            get => "/Assets/bootstrap_icons/pause-fill.svg";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRunning)
            {
                return isRunning ? PauseIcon : PlayIcon;
            }
            return PlayIcon;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
