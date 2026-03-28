using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Platform;
using PlumJsonAnimator.ViewModels;
using PlumJsonAnimator.Views;
using SukiUI.Controls;

namespace PlumJsonAnimator.Common.Dialogs
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

        private List<DialogSize> sizes = new List<DialogSize>
        {
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(250, 200),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
        };

        private UserControl userControlFactory(ViewType viewType, ViewModelBase viewModel)
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

        public async void ShowDialog(
            string title,
            ViewModelBase viewModel,
            Window owner,
            ViewType viewType
        )
        {
            DialogSize size = sizes[(int)viewType];

            var window = new SukiWindow
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

        public Window mainWin;

        public async void ShowDialog(string title, ViewModelBase viewModel, ViewType viewType)
        {
            DialogSize size = sizes[(int)viewType];

            var window = new SukiWindow
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

            await window.ShowDialog(mainWin);
        }
    }
}
