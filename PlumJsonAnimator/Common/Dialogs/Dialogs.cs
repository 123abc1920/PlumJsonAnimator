using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Platform;
using PlumJsonAnimator.ViewModels;
using PlumJsonAnimator.Views;
using SukiUI.Controls;

namespace PlumJsonAnimator.Common.Dialogs
{
    /// <summary>
    /// Dialogs types
    /// </summary>
    public enum DialogType
    {
        SETTINGS = 0,
        NEWPROJECT,
        RENAME,
        EXPORT_JPG,
        EXPORT_PNG,
        EXPORT_GIF,
        EXPORT_MP4,
    }

    /// <summary>
    /// Provides methods for opening dialogs
    /// </summary>
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

        private List<DialogSize> _sizes = new List<DialogSize>
        {
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(250, 200),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
            new DialogSize(600, 400),
        };

        private UserControl UserControlFactory(DialogType viewType, ViewModelBase viewModel)
        {
            if (viewType == DialogType.SETTINGS)
            {
                return new SettingsView(viewModel);
            }
            if (viewType == DialogType.NEWPROJECT)
            {
                return new NewProjectDialog(viewModel);
            }
            if (viewType == DialogType.RENAME)
            {
                return new RenameDialog(viewModel);
            }
            if (viewType == DialogType.EXPORT_JPG)
            {
                return new ExportPanelJPG(viewModel);
            }
            if (viewType == DialogType.EXPORT_PNG)
            {
                return new ExportPanelPNG(viewModel);
            }
            if (viewType == DialogType.EXPORT_GIF)
            {
                return new ExportPanelGIF(viewModel);
            }
            if (viewType == DialogType.EXPORT_MP4)
            {
                return new ExportPanelMP4(viewModel);
            }
            return new SettingsView(viewModel);
        }

        public async void ShowDialog(
            string title,
            ViewModelBase viewModel,
            Window owner,
            DialogType viewType
        )
        {
            DialogSize size = _sizes[(int)viewType];

            var window = new SukiWindow
            {
                Title = title,
                Width = size.width,
                Icon = new WindowIcon(
                    AssetLoader.Open(new Uri("avares://PlumJsonAnimator/Assets/logo.ico"))
                ),
                Height = size.height,
                Content = UserControlFactory(viewType, viewModel),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            await window.ShowDialog(owner);
        }

        public Window mainWin;

        public async void ShowDialog(string title, ViewModelBase viewModel, DialogType viewType)
        {
            DialogSize size = _sizes[(int)viewType];

            var window = new SukiWindow
            {
                Title = title,
                Width = size.width,
                Icon = new WindowIcon(
                    AssetLoader.Open(new Uri("avares://PlumJsonAnimator/Assets/logo.ico"))
                ),
                Height = size.height,
                Content = UserControlFactory(viewType, viewModel),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            await window.ShowDialog(mainWin);
        }
    }
}
