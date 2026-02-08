using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace Constants
{
    public class Popups
    {
        public static Window win = null;

        public static void ShowPopup(string message, Control target)
        {
            var popup = new Popup
            {
                Child = new Border
                {
                    Width = 200,
                    Height = 40,
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10, 10),
                    Child = new TextBlock { Text = message, Foreground = Brushes.Black },
                },
                PlacementTarget = target,
                Placement = PlacementMode.RightEdgeAlignedBottom,
                VerticalOffset = -50,
                HorizontalOffset = -250,
                IsOpen = true,
            };

            Task.Delay(1000)
                .ContinueWith(_ => Dispatcher.UIThread.InvokeAsync(() => popup.IsOpen = false));
        }

        public static void ShowPopup(string message)
        {
            var popup = new Popup
            {
                Child = new Border
                {
                    Width = 200,
                    Height = 40,
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10, 10),
                    Child = new TextBlock { Text = message, Foreground = Brushes.Black },
                },
                PlacementTarget = win,
                Placement = PlacementMode.RightEdgeAlignedBottom,
                VerticalOffset = -50,
                HorizontalOffset = -250,
                IsOpen = true,
            };

            Task.Delay(1000)
                .ContinueWith(_ => Dispatcher.UIThread.InvokeAsync(() => popup.IsOpen = false));
        }
    }
}
