using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

// TODO: rename dialogs localization
public partial class NewProjectViewModel : ViewModelBase
{
    private ProjectManager projectManager;

    public NewProjectViewModel(
        ProjectManager projectManager,
        GlobalState globalState,
        Dialogs dialogs,
        ProjectSettings projectSettings,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter
    )
        : base(
            globalState,
            dialogs,
            projectSettings,
            projectManager,
            appSettings,
            localizationService,
            imageExporter
        )
    {
        this.projectManager = projectManager;
    }

    public bool NewProject(string? projectName, string? projectPath)
    {
        Project? result = this.projectManager.NewProject(projectName, projectPath);
        if (result != null)
        {
            this.CurrentProject = result;
            return true;
        }
        return false;
    }
}
