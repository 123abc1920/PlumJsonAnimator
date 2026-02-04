using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Constants;
using SpinejsonEditor.ViewModels;

namespace SpinejsonEditor.Views
{
    public partial class ProjectSettingsPanel : UserControl
    {
        private string oldName;
        private string oldPath;
        private string oldSpineVersion;

        public ProjectSettingsPanel()
        {
            InitializeComponent();

            initOldVars();
        }

        public ProjectSettingsPanel(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        /// <summary>
        /// Intialises old settings values
        /// </summary>
        private void initOldVars()
        {
            oldName = ConstantsClass.currentProject.Name;
            oldPath = ConstantsClass.currentProject.ProjectPath;
            oldSpineVersion = ConstantsClass.currentProject.MetaData.Spine;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (oldName != ConstantsClass.currentProject.Name)
            {
                ProjectManager.ProjectManager.RenameProject(
                    Path.Combine(ConstantsClass.currentProject.ProjectPath, oldName),
                    Path.Combine(
                        ConstantsClass.currentProject.ProjectPath,
                        ConstantsClass.currentProject.Name
                    )
                );
            }

            if (
                Path.Combine(oldPath, ConstantsClass.currentProject.Name)
                != Path.Combine(
                    ConstantsClass.currentProject.ProjectPath,
                    ConstantsClass.currentProject.Name
                )
            )
            {
                ProjectManager.ProjectManager.CopyDir(
                    Path.Combine(oldPath, ConstantsClass.currentProject.Name),
                    Path.Combine(
                        ConstantsClass.currentProject.ProjectPath,
                        ConstantsClass.currentProject.Name
                    )
                );
            }

            initOldVars();
            ProjectSettings.ProjectSettings.WriteSettings();
        }
    }
}
