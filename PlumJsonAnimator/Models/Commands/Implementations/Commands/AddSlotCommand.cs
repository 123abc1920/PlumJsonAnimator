using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddSlotCommand : ICommand
{
    private readonly Slot _slot;
    private readonly Project _project;
    private readonly Bone _bone;

    public AddSlotCommand(Project project, Slot slot, Bone bone)
    {
        _project = project;
        _slot = slot;
        _bone = bone;
    }

    public void Execute()
    {
        _project?.Slots.Add(_slot);
        _project?.CurrentSkin.AddSlot(_slot);
        _bone.UpdateSlots();
    }

    public void Undo()
    {
        _project?.Slots.Remove(_slot);
        _project?.CurrentSkin.DeleteSlot(_slot);
        _bone?.UpdateSlots();
    }
}
