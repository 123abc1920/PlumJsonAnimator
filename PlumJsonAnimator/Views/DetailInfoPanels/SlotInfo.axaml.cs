using Avalonia.Controls;
using Avalonia.Interactivity;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.DTO;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class SlotInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.SLOTINFO;

    public SlotInfo(
        GlobalState globalState,
        HistoryManager historyManager,
        AttachmentFactory attachmentFactory
    )
    {
        InitializeComponent();
        DataContext = new SlotInfoViewModel(globalState, historyManager, attachmentFactory);
    }

    public static IInfoPanel Create(InfoPanelDTO infoPanelDTO)
    {
        return new SlotInfo(
            infoPanelDTO.GlobalState!,
            infoPanelDTO.HistoryManager!,
            infoPanelDTO.AttachmentFactory!
        );
    }
}
