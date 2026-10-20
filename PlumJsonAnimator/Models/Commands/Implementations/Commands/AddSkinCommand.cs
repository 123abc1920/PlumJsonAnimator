using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddSkinCommand : ICommand
{
    private readonly Project _project;
    private Skin _addedSkin;

    public AddSkinCommand(Project project)
    {
        _project = project;
    }

    public void Execute()
    {
        if (_addedSkin != null)
        {
            _project.RestoreSkin(_addedSkin);
            return;
        }

        _addedSkin = _project?.AddSkin();
    }

    public void Undo()
    {
        _project?.DeleteSkin(_addedSkin);
    }
}
