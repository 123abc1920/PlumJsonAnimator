using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
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

    public static IInfoPanel Create(GlobalState globalState, HistoryManager historyManager)
    {
        return new SlotInfo(globalState, historyManager);
    }
}
