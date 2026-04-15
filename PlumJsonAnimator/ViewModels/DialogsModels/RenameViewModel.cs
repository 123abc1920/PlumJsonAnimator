using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class RenameViewModel : ViewModelBase
{
    public IRenamable? RedactObj { get; set; } = null;
    
    public RenameViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectSettings projectSettings,
        ProjectFilesManager projectManager,
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
