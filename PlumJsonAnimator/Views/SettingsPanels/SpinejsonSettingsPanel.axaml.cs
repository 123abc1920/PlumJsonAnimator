using Avalonia;
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
    public partial class SpinejsonSettingsPanel : UserControl
    {
        public SpinejsonSettingsPanel()
        {
            InitializeComponent();
        }

        public SpinejsonSettingsPanel(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                AppSettingsData appSettingsData = new AppSettingsData()
                {
                    LastDir = "",
                    Workspace = pathTextBox.Text,
                    Lang = "ru",
                    Theme = viewModel.CurrentTheme,
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
                    new SukiUI.Models.SukiColorTheme("PlumTheme", Colors.AppColor, Colors.AppColor)
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
    }
}
