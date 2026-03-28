using Avalonia.Controls;
using Avalonia.Interactivity;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class RenameDialog : UserControl
    {
        public RenameDialog()
        {
            InitializeComponent();
        }

        public RenameDialog(ViewModelBase viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void SetName(object sender, RoutedEventArgs e)
        {
            if (DataContext is RenameViewModel viewModel)
            {
                string? newName = this.FindControl<TextBox>("name")?.Text;
                viewModel.RedactObj.SetName(newName);

                var parentWindow = this.VisualRoot as Window;
                parentWindow?.Close();
            }
        }
    }
}
