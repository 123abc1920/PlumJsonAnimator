using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.DTO;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class AttachmentInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.ATTACHMENTINFO;

    public AttachmentInfo(
        GlobalState globalState,
        HistoryManager historyManager,
        AttachmentTypeConverter attachmentTypeConverter,
        Slot slot
    )
    {
        InitializeComponent();
        DataContext = new AttachmentInfoViewModel(
            globalState,
            historyManager,
            attachmentTypeConverter,
            slot
        );
    }

    public static IInfoPanel Create(InfoPanelDTO infoPanelDTO)
    {
        return new AttachmentInfo(
            infoPanelDTO.GlobalState!,
            infoPanelDTO.HistoryManager!,
            infoPanelDTO.AttachmentTypeConverter!,
            infoPanelDTO.Slot!
        );
    }
}
