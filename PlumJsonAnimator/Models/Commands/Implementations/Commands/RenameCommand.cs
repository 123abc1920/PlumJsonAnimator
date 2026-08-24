using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Commands;

class RenameCommand : ICommand
{
    private readonly IRenamable _renamableObject;
    private readonly string _oldName;
    private readonly string _newName;

    public RenameCommand(IRenamable renamable, string oldName, string newName)
    {
        this._renamableObject = renamable;
        this._oldName = oldName;
        this._newName = newName;
    }

    public void Execute()
    {
        _renamableObject.SetName(_newName);
    }

    public void Undo()
    {
        _renamableObject.SetName(_oldName);
    }
}
