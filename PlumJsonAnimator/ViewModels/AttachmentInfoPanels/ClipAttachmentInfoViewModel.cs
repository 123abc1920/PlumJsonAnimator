using System.Windows.Input;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.ViewModels;

public partial class ClipAttachmentInfoViewModel : ReactiveObject
{
    public ClipAttachment ClipAttachment { get; set; }

    public ICommand AddVertice { get; }

    public ClipAttachmentInfoViewModel(ClipAttachment clipAttachment)
    {
        ClipAttachment = clipAttachment;

        AddVertice = new Command(_ =>
        {
            ClipAttachment.Vertices.Add(new PointModel(0, 0));
        });
    }
}
