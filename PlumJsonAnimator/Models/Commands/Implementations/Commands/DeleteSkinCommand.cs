using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DeleteSkinCommand : ICommand
{
    private readonly Skin _skin;
    private readonly Project _project;

    public DeleteSkinCommand(Project project, Skin skin)
    {
        _project = project;
        _skin = skin;
    }

    public void Execute()
    {
        _project?.DeleteSkin(_skin);
    }

    public void Undo()
    {
        _project?.RestoreSkin(_skin);
    }
}
