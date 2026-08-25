using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DeleteSkinCommand : ICommand
{
    private readonly Skin _skin;
    private readonly Project _project;

    public DeleteSkinCommand(Project project, Skin skin)
    {
        this._project = project;
        this._skin = skin;
    }

    public void Execute()
    {
        _project?.DeleteSkin(this._skin);
    }

    public void Undo()
    {
        _project?.RestoreSkin(this._skin);
    }
}
