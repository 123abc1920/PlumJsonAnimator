using System.Collections.ObjectModel;
using Avalonia.Controls;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class ClipAttachmentInfo : UserControl
{
    public ClipAttachmentInfo(
        ClipAttachment clipAttachment,
        ObservableCollection<Slot>? availableSlotNames
    )
    {
        InitializeComponent();
        DataContext = new ClipAttachmentInfoViewModel(clipAttachment, availableSlotNames);
    }
}
