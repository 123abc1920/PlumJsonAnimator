using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class SlotInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.SLOTINFO;

    public SlotInfo(GlobalState globalState)
    {
        InitializeComponent();
        DataContext = new SlotInfoViewModel(globalState);
    }

    public static IInfoPanel Create(GlobalState globalState)
    {
        return new SlotInfo(globalState);
    }
}
