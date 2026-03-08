using System;
using System.Globalization;
using Avalonia.Data.Converters;
using IconPacks.Avalonia.BootstrapIcons;

namespace PlumJsonAnimator.Common.Converters
{
    public class AnimationConverter : IValueConverter
    {
        public PackIconBootstrapIconsKind PlayIcon => PackIconBootstrapIconsKind.PlayFill;

        public PackIconBootstrapIconsKind PauseIcon => PackIconBootstrapIconsKind.PauseFill;

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
        ) => throw new NotImplementedException();
    }
}
