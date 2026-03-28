using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using SukiUI.Toasts;

namespace PlumJsonAnimator.Common.Dialogs
{
    public class Popups
    {
        public static ISukiToastManager ToastManager;
        private const double SHOW_TIME = 3;

        public static void ShowPopup(string message, Control target)
        {
            ToastManager
                .CreateToast()
                .WithTitle("Сообщение")
                .WithContent(message)
                .Dismiss()
                .After(TimeSpan.FromSeconds(SHOW_TIME))
                .Queue();
        }

        public static void ShowPopup(string message)
        {
            ToastManager
                .CreateToast()
                .WithTitle("Сообщение")
                .WithContent(message)
                .Dismiss()
                .After(TimeSpan.FromSeconds(SHOW_TIME))
                .Queue();
        }
    }
}
