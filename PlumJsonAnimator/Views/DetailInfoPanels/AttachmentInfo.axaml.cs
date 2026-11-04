using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class AttachmentInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.ATTACHMENTINFO;

    public AttachmentInfo(
        GlobalState globalState,
        HistoryManager historyManager,
        AttachmentTypeConverter attachmentTypeConverter
    )
    {
        InitializeComponent();
        DataContext = new AttachmentInfoViewModel(
            globalState,
            historyManager,
            attachmentTypeConverter
        );
    }

    public static IInfoPanel Create(
        GlobalState globalState,
        HistoryManager historyManager,
        AttachmentTypeConverter? attachmentTypeConverter = null
    )
    {
        return new AttachmentInfo(globalState, historyManager, attachmentTypeConverter!);
    }
}
