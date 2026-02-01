using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
    }
}
