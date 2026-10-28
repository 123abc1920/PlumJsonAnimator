using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class SlotInfo : UserControl, IInfoPanel
{
    public static InfoPanelTypes InfoPanelType => InfoPanelTypes.SLOTINFO;

    public SlotInfo(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
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
            plumApp
        );
    }

    public static IInfoPanel Create(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
    {
        return new SlotInfo(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp
        );
    }
}
