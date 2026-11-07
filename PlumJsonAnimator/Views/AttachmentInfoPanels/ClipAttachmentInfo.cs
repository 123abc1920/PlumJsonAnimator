using Avalonia.Controls;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class ClipAttachmentInfo : UserControl
{
    public ClipAttachmentInfo(ClipAttachment clipAttachment)
    {
        InitializeComponent();
        DataContext = new ClipAttachmentInfoViewModel(clipAttachment);
    }
}
