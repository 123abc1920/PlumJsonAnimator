using Avalonia.Controls;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class SlotInfo : UserControl, IInfoPanel
{
    public static InfoPanelTypes InfoPanelType => InfoPanelTypes.SLOTINFO;

    public SlotInfo()
    {
        InitializeComponent();
    }

    public SlotInfo(ViewModelBase viewModel)
        : this()
    {
        DataContext = viewModel;
    }

    public static IInfoPanel Create()
    {
        return new SlotInfo();
    }
}
