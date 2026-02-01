using System;
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
            initOldVars();
        }
    }
}
