using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.ViewModels;

public partial class RegionAttachmentInfoViewModel : ReactiveObject
{
    public ImageRes? Image
    {
        get => RegionAttachment.ImageRes;
        set
        {
            if (
                RegionAttachment.ImageRes is ImageRes imageRes
                && RegionAttachment.ImageRes != value
            )
            {
                RegionAttachment.ImageRes = value;
                this.RaisePropertyChanged(nameof(Image));
            }
        }
    }

    public RegionAttachment RegionAttachment { get; set; }

    public RegionAttachmentInfoViewModel(RegionAttachment regionAttachment)
    {
        RegionAttachment = regionAttachment;
    }
}
