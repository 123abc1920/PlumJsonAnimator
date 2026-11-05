using PlumJsonAnimator.Models.Resources;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.ViewModels;

public partial class RegionAttachmentInfoViewModel : ReactiveObject
{
    [Reactive]
    public ImageRes? Image { get; set; }

    public RegionAttachmentInfoViewModel(ImageRes? imageRes)
    {
        Image = imageRes;
    }
}
