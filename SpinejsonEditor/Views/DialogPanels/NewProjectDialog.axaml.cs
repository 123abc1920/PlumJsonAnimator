using System;
using AnimEngine;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Constants;
using SpinejsonEditor.ViewModels;

namespace SpinejsonEditor.Views
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
    }
}
