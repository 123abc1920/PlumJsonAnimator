using System;
using SukiUI.Toasts;

namespace PlumJsonAnimator.Common.Dialogs
{
    /// <summary>
    /// Procides popups
    /// </summary>
    public class Popups
    {
        public static ISukiToastManager ToastManager;
        private const double SHOW_TIME = 3;

        public static void ShowPopup(string message, string title)
        {
            ToastManager
                .CreateToast()
                .WithTitle(title)
                .WithContent(message)
                .Dismiss()
                .After(TimeSpan.FromSeconds(SHOW_TIME))
                .Queue();
        }
    }
}
