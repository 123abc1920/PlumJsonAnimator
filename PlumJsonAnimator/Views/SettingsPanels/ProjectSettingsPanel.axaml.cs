using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class ProjectSettingsPanel : UserControl
    {
        public ProjectSettingsPanel()
        {
            InitializeComponent();
        }

        public ProjectSettingsPanel(AppSettingsViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (DataContext is AppSettingsViewModel viewModel)
            {
                SettingsData settingsData = new SettingsData()
                {
                    Path = path.Text,
                    Name = pName.Text,
                    Spine = pVersion.Text,
                    Anim = "",
                };
                viewModel.RenameProject(settingsData);
            }
        }

        private async void SelectFolder(object sender, RoutedEventArgs e)
        {
            if (DataContext is AppSettingsViewModel viewModel)
            {
                var topLevel = TopLevel.GetTopLevel(this);
                var storageProvider = topLevel!.StorageProvider;

                var folders = await storageProvider.OpenFolderPickerAsync(
                    new FolderPickerOpenOptions
                    {
                        Title = $"{viewModel.GetMessage(LocalizationConsts.SELECT_FOLDER)}",
                        AllowMultiple = false,
                    }
                );

                if (folders.Count > 0)
                {
                    string folderPath = folders[0].Path.LocalPath;

                    if (this.FindControl<TextBox>("path") is TextBox pathTextBox)
                    {
                        pathTextBox.Text = folderPath;
                    }
                }
            }
        }
    }
}
