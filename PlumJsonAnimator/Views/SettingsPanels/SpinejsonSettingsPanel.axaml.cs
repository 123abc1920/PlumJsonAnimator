using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;
using SukiUI;

// TODO: исправить привязки
namespace PlumJsonAnimator.Views
{
    public partial class AppSettingsPanel : UserControl
    {
        public AppSettingsPanel()
        {
            InitializeComponent();
        }

        public AppSettingsPanel(AppSettingsViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (DataContext is AppSettingsViewModel viewModel)
            {
                var rect = viewModel.GetCaptureArea().GetRect();

                AppSettingsData appSettingsData = new AppSettingsData()
                {
                    LastDir = "",
                    Workspace = Path.Combine(pathTextBox.Text, viewModel.CurrentProject.Name),
                    Lang = viewModel.CurrentLang,
                    Theme = viewModel.CurrentTheme,
                    Ffmpeg = viewModel.FfmpegPath,
                    CaptureX = ((int)rect.X == null) ? 0 : (int)rect.X,
                    CaptureY = ((int)rect.Y == null) ? 0 : (int)rect.Y,
                    CaptureWidth = (int)rect.Width,
                    CaptureHeight = (int)rect.Height,
                };

                viewModel.SaveSettings(appSettingsData);

                var sukiTheme = SukiTheme.GetInstance();

                if (viewModel.CurrentTheme == "dark")
                {
                    sukiTheme.ChangeBaseTheme(ThemeVariant.Dark);
                }
                else
                {
                    sukiTheme.ChangeBaseTheme(ThemeVariant.Light);
                }

                sukiTheme.ChangeColorTheme(
                    new SukiUI.Models.SukiColorTheme(
                        "PlumTheme",
                        AppColors.AppColor,
                        AppColors.AppColor
                    )
                );
            }
        }

        private async void SelectFolder(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var storageProvider = topLevel.StorageProvider;

            var folders = await storageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions { Title = "Выберите папку", AllowMultiple = false }
            );

            if (folders.Count > 0)
            {
                string folderPath = folders[0].Path.LocalPath;
                pathTextBox.Text = folderPath;
            }
        }

        private async void SelectFfmpeg(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var storageProvider = topLevel.StorageProvider;

            var fileTypeFilter = new FilePickerFileType[]
            {
                new($"*exe") { Patterns = new[] { $"*exe" } },
            };

            var result = await storageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Выберите файл ffmpeg.exe",
                    AllowMultiple = false,
                    FileTypeFilter = fileTypeFilter,
                }
            );

            var path = result?.FirstOrDefault()?.Path.LocalPath;
            if (DataContext is AppSettingsViewModel viewModel)
            {
                viewModel.FfmpegPath = path;
            }
        }
    }
}
