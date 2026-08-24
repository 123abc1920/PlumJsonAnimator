using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddSkinCommand : ICommand
{
    private readonly Project _project;
    private Skin _addedSkin;

    public AddSkinCommand(Project project)
    {
        this._project = project;
    }

    public void Execute()
    {
        if (this._addedSkin != null)
        {
            _project.RestoreSkin(this._addedSkin);
            return;
        }

        this._addedSkin = _project?.AddSkin();
    }

    public void Undo()
    {
        _project?.DeleteSkin(this._addedSkin);
    }
}
