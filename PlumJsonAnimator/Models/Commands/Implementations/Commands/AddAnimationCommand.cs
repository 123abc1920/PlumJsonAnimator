using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddAnimationCommand : ICommand
{
    private readonly Project _project;
    private Animation _addedAnimation;

    public AddAnimationCommand(Project project)
    {
        _project = project;
    }

    public void Execute()
    {
        if (_addedAnimation != null)
        {
            _project.RestoreAnimation(_addedAnimation);
            return;
        }

        _addedAnimation = _project.AddAnimation();
    }

    public void Undo()
    {
        _project.DeleteAnimation(_addedAnimation);
    }
}
