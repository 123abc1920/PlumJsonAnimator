using AnimEngine;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Constants;
using SpinejsonEditor.ViewModels;

namespace SpinejsonEditor.Views
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

        private string[] _themes = new[]
        {
            "avares://SpinejsonEditor/Views/Themes/LightTheme.axaml",
            "avares://SpinejsonEditor/Views/Themes/DarkTheme.axaml",
        };

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveSettings();
            Popups.ShowPopup("Saved", this);

            if (ConstantsClass.theme == "dark")
            {
                Application.Current.RequestedThemeVariant = ThemeVariant.Light;
            }
            else
            {
                Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
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
