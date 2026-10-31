using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.States;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class SlotInfoViewModel : ViewModelBase
{
    public SlotInfoViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp,
        InfoPanelFactory infoPanelFactory,
        PanelState panelState
    )
        : base(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp
        ) { }
}
