using System;
using AnimEngine;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Constants;
using SpinejsonEditor.ViewModels;

namespace SpinejsonEditor.Views
{
    public partial class RenameDialog : UserControl
    {
        public RenameDialog()
        {
            InitializeComponent();
        }

        public RenameDialog(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void SetName(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                string newName = this.FindControl<TextBox>("name")?.Text;
                viewModel.RedactObj.SetName(newName);

                var parentWindow = this.VisualRoot as Window;
                parentWindow?.Close();
            }
        }
    }
}
