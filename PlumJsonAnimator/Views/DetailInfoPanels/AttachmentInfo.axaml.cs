using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class AttachmentInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.ATTACHMENTINFO;

    public AttachmentInfo(GlobalState globalState, HistoryManager historyManager)
    {
        InitializeComponent();
        DataContext = new AttachmentInfoViewModel(globalState, historyManager);
    }

    public static IInfoPanel Create(GlobalState globalState, HistoryManager historyManager)
    {
        return new AttachmentInfo(globalState, historyManager);
    }
}
