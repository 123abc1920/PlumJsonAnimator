using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DeleteSlotCommand : ICommand
{
    private readonly Slot _slot;
    private readonly Project _project;
    private readonly Bone _bone;
    private Attachment _attachment;

    public DeleteSlotCommand(Project project, Slot slot, Bone bone)
    {
        _project = project;
        _slot = slot;
        _bone = bone;
    }

    public void Execute()
    {
        _project?.Slots.Remove(_slot);
        _attachment = _project?.CurrentSkin.GetAttachment(_slot);
        _project?.CurrentSkin.DeleteSlot(_slot);
        _bone?.UpdateSlots();
    }

    public void Undo()
    {
        _project?.Slots.Add(_slot);
        _project?.CurrentSkin.RestoreSlot(_slot, _attachment);
        _bone?.UpdateSlots();
    }
}
