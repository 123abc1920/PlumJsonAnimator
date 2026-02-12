using AnimEngine;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Constants;
using PlumAnimation.ViewModels;

namespace PlumAnimation.Views
{
    public partial class NewProjectDialog : UserControl
    {
        public NewProjectDialog()
        {
            InitializeComponent();
        }

        public NewProjectDialog(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;

            if (this.FindControl<TextBox>("name") is TextBox nameTextBox)
            {
                nameTextBox.Text = "NewProject";
            }

            if (this.FindControl<TextBox>("workspace") is TextBox workspaceTextBox)
            {
                workspaceTextBox.Text = AppSettings.appSettings.Workspace;
            }
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            var projectName = this.FindControl<TextBox>("name")?.Text;
            var workspace = this.FindControl<TextBox>("workspace")?.Text;

            bool result = ProjectManager.ProjectManager.NewProject(projectName, workspace);

            if (result == true)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.CurrentProject = ConstantsClass.currentProject;
                }

                var parentWindow = this.VisualRoot as Window;
                parentWindow?.Close();
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

                if (this.FindControl<TextBox>("workspace") is TextBox pathTextBox)
                {
                    pathTextBox.Text = folderPath;
                }
            }
        }
    }
}
