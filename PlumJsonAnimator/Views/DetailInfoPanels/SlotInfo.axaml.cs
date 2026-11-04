using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class SlotInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.SLOTINFO;

    public SlotInfo(GlobalState globalState, HistoryManager historyManager)
    {
        InitializeComponent();
        DataContext = new SlotInfoViewModel(globalState, historyManager);
    }

    public static IInfoPanel Create(
        GlobalState globalState,
        HistoryManager historyManager,
        AttachmentTypeConverter? attachmentTypeConverter = null
    )
    {
        return new SlotInfo(globalState, historyManager);
    }
}
