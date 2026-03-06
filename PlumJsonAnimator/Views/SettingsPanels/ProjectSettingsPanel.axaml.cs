using System.IO;
using AnimEngine.Project;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Common.Constants;
using Constants.CommonItemsUI;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class ProjectSettingsPanel : UserControl
    {
        public ProjectSettingsPanel()
        {
            InitializeComponent();
        }

        public ProjectSettingsPanel(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            var oldName = ConstantsClass.currentProject.Name;
            ConstantsClass.currentProject.Name = pName.Text;
            ProjectManager.RenameProject(
                Path.Combine(ConstantsClass.currentProject.ProjectPath, oldName),
                Path.Combine(
                    ConstantsClass.currentProject.ProjectPath,
                    ConstantsClass.currentProject.Name
                )
            );

            var oldPath = ConstantsClass.currentProject.ProjectPath;
            ConstantsClass.currentProject.ProjectPath = path.Text;
            ProjectManager.CopyDir(
                Path.Combine(oldPath, ConstantsClass.currentProject.Name),
                Path.Combine(
                    ConstantsClass.currentProject.ProjectPath,
                    ConstantsClass.currentProject.Name
                )
            );

            AppSettings.SaveSettings();

            ConstantsClass.currentProject.MetaData.Spine = pVersion.Text;

            ProjectSettings.WriteSettings();
            Popups.ShowPopup("Saved", this);
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

                if (this.FindControl<TextBox>("path") is TextBox pathTextBox)
                {
                    pathTextBox.Text = folderPath;
                }
            }
        }
    }
}
