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
            public int _width;
            public int _height;

            public DialogSize(int _x, int _y)
            {
                _width = _x;
                _height = _y;
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
                Width = size._width,
                Icon = new WindowIcon(
                    AssetLoader.Open(new Uri("avares://PlumJsonAnimator/Assets/logo.ico"))
                ),
                Height = size._height,
                Content = UserControlFactory(viewType, viewModel),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            await window.ShowDialog(owner);
        }

        public Window MainWin { get; set; }

        public async void ShowDialog(string title, ViewModelBase viewModel, DialogType viewType)
        {
            DialogSize size = _sizes[(int)viewType];

            var window = new SukiWindow
            {
                Title = title,
                Width = size._width,
                Icon = new WindowIcon(
                    AssetLoader.Open(new Uri("avares://PlumJsonAnimator/Assets/logo.ico"))
                ),
                Height = size._height,
                Content = UserControlFactory(viewType, viewModel),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            await window.ShowDialog(MainWin);
        }
    }
}
