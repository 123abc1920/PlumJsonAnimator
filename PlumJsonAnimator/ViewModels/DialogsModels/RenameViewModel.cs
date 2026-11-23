using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class RenameViewModel : ViewModelBase
{
    public IRenamable? RenamableObject { get; set; } = null;

    public RenameViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        ViewModelFactory viewModelFactory,
        PlumApp plumApp
    )
        : base(
            globalState,
            dialogs,
            appSettings,
            localizationService,
            imageExporter,
            viewModelFactory,
            plumApp
        ) { }
}
