using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class RenameViewModel : ViewModelBase
{
    public RenameViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectSettings projectSettings,
        ProjectManager projectManager,
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
        ) { }
}
