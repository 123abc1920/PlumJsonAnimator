using Avalonia.Controls;
using Avalonia.Interactivity;
using PlumAnimation.ViewModels;

namespace PlumAnimation.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public SettingsView(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void ShowProjectSettings(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                SettingsContentControl.Content = new ProjectSettingsPanel(viewModel);
            }
        }

        private void ShowSpinejsonSettings(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                SettingsContentControl.Content = new SpinejsonSettingsPanel(viewModel);
            }
        }
    }
}
