using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SpinejsonEditor.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ShowProjectSettings(object sender, RoutedEventArgs e)
        {
            SettingsContentControl.Content = new ProjectSettingsPanel();
        }

        private void ShowSpinejsonSettings(object sender, RoutedEventArgs e)
        {
            SettingsContentControl.Content = new SpinejsonSettingsPanel();
        }
    }
}
