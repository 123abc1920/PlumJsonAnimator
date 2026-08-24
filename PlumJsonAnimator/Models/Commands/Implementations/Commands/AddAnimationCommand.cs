using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddAnimationCommand : ICommand
{
    private readonly Project _project;
    private Animation _addedAnimation;

    public AddAnimationCommand(Project project)
    {
        this._project = project;
    }

    public void Execute()
    {
        if (this._addedAnimation != null)
        {
            _project.RestoreAnimation(_addedAnimation);
            return;
        }

        this._addedAnimation = _project.AddAnimation();
    }

    public void Undo()
    {
        _project.DeleteAnimation(_addedAnimation);
    }
}
