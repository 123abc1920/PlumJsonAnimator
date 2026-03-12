using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace PlumJsonAnimator.Common.Dialogs
{
    public class Popups
    {
        public static Window? win = null;

        public static void ShowPopup(string message, Control target)
        {
            var topLevel = TopLevel.GetTopLevel(target);
            if (topLevel == null)
                return;

            var overlayLayer = OverlayLayer.GetOverlayLayer(topLevel);
            if (overlayLayer == null)
                return;

            // 1. ОПРЕДЕЛЯЕМ ЦВЕТА (Раз TryGetResource не срабатывает)
            bool isDark = topLevel.ActualThemeVariant == ThemeVariant.Dark;

            // В светлой теме Suki фон обычно белый или очень светло-серый
            var bgColor = isDark ? "#1A1A1A" : "#FFFFFF";
            var textColor = isDark ? Brushes.White : Brushes.Black;

            var container = new Panel
            {
                Width = topLevel.Bounds.Width,
                Height = topLevel.Bounds.Height,
                Background = Brushes.Transparent,
                IsHitTestVisible = false,
            };

            var notification = new Border
            {
                MinWidth = 200,
                Background = SolidColorBrush.Parse(bgColor),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15, 10), // Чуть больше отступ слева для красоты
                BorderBrush = SolidColorBrush.Parse("#ff003b"),
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 20, 20),

                Child = new TextBlock
                {
                    Text = message,
                    Foreground = textColor,
                    FontSize = 13,
                    TextAlignment = TextAlignment.Left, // ТЕКСТ ТЕПЕРЬ СЛЕВА
                    TextWrapping = TextWrapping.Wrap,
                },
            };

            container.Children.Add(notification);
            overlayLayer.Children.Add(container);

            Task.Delay(1500)
                .ContinueWith(_ =>
                    Dispatcher.UIThread.InvokeAsync(() => overlayLayer.Children.Remove(container))
                );
        }

        public static void ShowPopup(string message)
        {
            var topLevel = TopLevel.GetTopLevel(win);
            if (topLevel == null)
                return;

            var overlayLayer = OverlayLayer.GetOverlayLayer(topLevel);
            if (overlayLayer == null)
                return;

            // 1. ОПРЕДЕЛЯЕМ ЦВЕТА (Раз TryGetResource не срабатывает)
            bool isDark = topLevel.ActualThemeVariant == ThemeVariant.Dark;

            // В светлой теме Suki фон обычно белый или очень светло-серый
            var bgColor = isDark ? "#1A1A1A" : "#FFFFFF";
            var textColor = isDark ? Brushes.White : Brushes.Black;

            var container = new Panel
            {
                Width = topLevel.Bounds.Width,
                Height = topLevel.Bounds.Height,
                Background = Brushes.Transparent,
                IsHitTestVisible = false,
            };

            var notification = new Border
            {
                MinWidth = 200,
                Background = SolidColorBrush.Parse(bgColor),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15, 10), // Чуть больше отступ слева для красоты
                BorderBrush = SolidColorBrush.Parse("#ff003b"),
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 20, 20),

                Child = new TextBlock
                {
                    Text = message,
                    Foreground = textColor,
                    FontSize = 13,
                    TextAlignment = TextAlignment.Left, // ТЕКСТ ТЕПЕРЬ СЛЕВА
                    TextWrapping = TextWrapping.Wrap,
                },
            };

            container.Children.Add(notification);
            overlayLayer.Children.Add(container);

            Task.Delay(1500)
                .ContinueWith(_ =>
                    Dispatcher.UIThread.InvokeAsync(() => overlayLayer.Children.Remove(container))
                );
        }
    }
}
