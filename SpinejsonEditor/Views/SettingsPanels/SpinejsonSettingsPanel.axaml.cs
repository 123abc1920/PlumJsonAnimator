using AnimEngine;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            AppSettings.SaveSettings();
            Popups.ShowPopup("Saved", this);
        }
    }
}
