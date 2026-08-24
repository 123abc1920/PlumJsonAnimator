using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DeleteAnimationCommand : ICommand
{
    private readonly Animation _animation;
    private readonly Project _project;

    public DeleteAnimationCommand(Project project, Animation animation)
    {
        this._project = project;
        this._animation = animation;
    }

    public void Execute()
    {
        _project?.DeleteAnimation(this._animation);
    }

    public void Undo()
    {
        _project?.RestoreAnimation(this._animation);
    }
}
