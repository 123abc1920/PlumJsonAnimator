using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Platform;
using PlumJsonAnimator.ViewModels;
using PlumJsonAnimator.Views;

namespace Constants.CommonItemsUI
{
    public enum ViewType
    {
        SETTINGS = 0,
        NEWPROJECT,
        RENAME,
        EXPORT_JPG,
        EXPORT_PNG,
        EXPORT_GIF,
        EXPORT_MP4,
    }

    public class Dialogs
    {
        private class DialogSize
        {
            public int width;
            public int height;

            public DialogSize(int _x, int _y)
            {
                this.width = _x;
                this.height = _y;
            }
        }

        private static List<DialogSize> sizes = new List<DialogSize>
        {
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(250, 100),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
        };

        private static UserControl userControlFactory(
            ViewType viewType,
            MainWindowViewModel viewModel
        )
        {
            if (viewType == ViewType.SETTINGS)
            {
                return new SettingsView(viewModel);
            }
            if (viewType == ViewType.NEWPROJECT)
            {
                return new NewProjectDialog(viewModel);
            }
            if (viewType == ViewType.RENAME)
            {
                return new RenameDialog(viewModel);
            }
            if (viewType == ViewType.EXPORT_JPG)
            {
                return new ExportPanelJPG(viewModel);
            }
            if (viewType == ViewType.EXPORT_PNG)
            {
                return new ExportPanelPNG(viewModel);
            }
            if (viewType == ViewType.EXPORT_GIF)
            {
                return new ExportPanelGIF(viewModel);
            }
            if (viewType == ViewType.EXPORT_MP4)
            {
                return new ExportPanelMP4(viewModel);
            }
            return new SettingsView(viewModel);
        }

        public static async void ShowDialog(
            string title,
            MainWindowViewModel viewModel,
            Window owner,
            ViewType viewType
        )
        {
            DialogSize size = sizes[(int)viewType];

            var window = new Window
            {
                Title = title,
                Width = size.width,
                Icon = new WindowIcon(
                    AssetLoader.Open(new Uri("avares://PlumJsonAnimator/Assets/logo.ico"))
                ),
                Height = size.height,
                Content = userControlFactory(viewType, viewModel),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            await window.ShowDialog(owner);
        }
    }
}
