using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.States;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class SlotInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.SLOTINFO;

    public SlotInfo(
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
    {
        InitializeComponent();
        DataContext = new SlotInfoViewModel(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp,
            infoPanelFactory,
            panelState
        );
    }

    public static IInfoPanel Create(
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
    {
        return new SlotInfo(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp,
            infoPanelFactory,
            panelState
        );
    }
}
