using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DropImageToBoneCommand : ICommand
{
    private readonly Bone _selectedBone;
    private readonly Project _project;
    private readonly Res _res;
    private readonly Slot _slot;

    public DropImageToBoneCommand(Bone selectedBone, Project project, Res res, Slot slot)
    {
        this._selectedBone = selectedBone;
        this._project = project;
        this._res = res;
        this._slot = slot;
    }

    public void Execute()
    {
        _project.AddSlotToProject(_slot, _selectedBone);
        _project.CurrentSkin.BindSlotAttachment(_slot, new ImageAttachment((ImageRes)_res));
        _selectedBone.UpdateSlots();
    }

    public void Undo()
    {
        _project.DeleteSlotFromProject(_slot);
        _project.CurrentSkin.DeleteSlot(_slot);
        _selectedBone.UpdateSlots();
    }
}
