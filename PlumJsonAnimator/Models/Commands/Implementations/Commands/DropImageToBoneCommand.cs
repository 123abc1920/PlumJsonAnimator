using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

namespace PlumJsonAnimator.Models.Commands;

class DropImageToBoneCommand : ICommand
{
    private readonly Bone _selectedBone;
    private readonly Project _project;
    private readonly Res _res;
    private readonly Slot _slot;
    private readonly AttachmentFactory _attachmentFactory;
    private readonly AttachmentDTO _attachmentDTO;

    public DropImageToBoneCommand(
        Bone selectedBone,
        Project project,
        Res res,
        Slot slot,
        AttachmentFactory attachmentFactory,
        AttachmentDTO attachmentDTO
    )
    {
        _selectedBone = selectedBone;
        _project = project;
        _res = res;
        _slot = slot;
        _attachmentFactory = attachmentFactory;
        _attachmentDTO = attachmentDTO;
    }

    public void Execute()
    {
        _project.AddSlotToProject(_slot, _selectedBone);
        _project.CurrentSkin.BindSlotAttachment(
            _slot,
            _attachmentFactory.CreateAttachmentByType(AttachmentTypes.REGION, _attachmentDTO)
        );
        _selectedBone.UpdateSlots();
    }

    public void Undo()
    {
        _project.DeleteSlotFromProject(_slot);
        _project.CurrentSkin.DeleteSlot(_slot);
        _selectedBone.UpdateSlots();
    }
}
