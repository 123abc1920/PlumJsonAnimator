using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

// TODO: rename dialogs localization
public partial class NewProjectViewModel : ViewModelBase
{
    public NewProjectViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
        : base(
            globalState,
            dialogs,
            appSettings,
            localizationService,
            imageExporter,
            plumApp
        ) { }

    public bool NewProject(string? projectName, string? projectPath)
    {
        return PlumApp.NewProject(projectName, projectPath);
    }
}
