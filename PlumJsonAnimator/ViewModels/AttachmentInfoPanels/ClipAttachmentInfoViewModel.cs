using System.Collections.ObjectModel;
using System.Windows.Input;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.ViewModels;

public partial class ClipAttachmentInfoViewModel : ReactiveObject
{
    public ObservableCollection<Slot>? AvailableEndSlotNames { get; }

    public ClipAttachment ClipAttachment { get; set; }

    public ICommand AddVertice { get; }

    public ClipAttachmentInfoViewModel(
        ClipAttachment clipAttachment,
        ObservableCollection<Slot>? availableSlotNames
    )
    {
        ClipAttachment = clipAttachment;

        AvailableEndSlotNames = availableSlotNames;

        AddVertice = new Command(_ =>
        {
            ClipAttachment.Vertices.Add(new PointModel(0, 0));
        });
    }
}
