using Avalonia.Controls;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class RegionAttachmentInfo : UserControl
{
    public RegionAttachmentInfo(RegionAttachment regionAttachment)
    {
        InitializeComponent();
        DataContext = new RegionAttachmentInfoViewModel(regionAttachment);
    }
}
