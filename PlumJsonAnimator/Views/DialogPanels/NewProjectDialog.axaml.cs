using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class NewProjectDialog : UserControl
    {
        public NewProjectDialog()
        {
            InitializeComponent();
        }

        public NewProjectDialog(ViewModelBase viewModel)
            : this()
        {
            DataContext = viewModel;

            if (this.FindControl<TextBox>("name") is TextBox nameTextBox)
            {
                nameTextBox.Text = "NewProject";
            }

            if (this.FindControl<TextBox>("workspace") is TextBox workspaceTextBox)
            {
                workspaceTextBox.Text = viewModel.CurrentProject!.ProjectPath;
            }
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            var projectName = this.FindControl<TextBox>("name")?.Text;
            var workspace = this.FindControl<TextBox>("workspace")?.Text;

            bool result = false;
            if (DataContext is NewProjectViewModel vm)
            {
                result = vm.NewProject(projectName, workspace);
            }

            if (result == true)
            {
                var parentWindow = this.VisualRoot as Window;
                parentWindow?.Close();
            }
        }

        private async void SelectFolder(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var storageProvider = topLevel!.StorageProvider;

            var folders = await storageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions { Title = "Выберите папку", AllowMultiple = false }
            );

            if (folders.Count > 0)
            {
                string folderPath = folders[0].Path.LocalPath;

                if (this.FindControl<TextBox>("workspace") is TextBox pathTextBox)
                {
                    pathTextBox.Text = folderPath;
                }
            }
        }
    }
}
