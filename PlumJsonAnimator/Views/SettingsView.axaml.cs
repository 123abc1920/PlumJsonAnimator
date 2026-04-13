using Avalonia.Controls;
using Avalonia.Interactivity;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public SettingsView(ViewModelBase viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void ShowProjectSettings(object sender, RoutedEventArgs e)
        {
            if (DataContext is AppSettingsViewModel viewModel)
            {
                SettingsContentControl.Content = new ProjectSettingsPanel(viewModel);
            }
        }

        private void ShowAppSettings(object sender, RoutedEventArgs e)
        {
            if (DataContext is AppSettingsViewModel viewModel)
            {
                SettingsContentControl.Content = new AppSettingsPanel(viewModel);
            }
        }
    }
}
