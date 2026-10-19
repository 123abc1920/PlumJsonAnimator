using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DeleteAnimationCommand : ICommand
{
    private readonly Animation _animation;
    private readonly Project _project;

    public DeleteAnimationCommand(Project project, Animation animation)
    {
        _project = project;
        _animation = animation;
    }

    public void Execute()
    {
        _project?.DeleteAnimation(_animation);
    }

    public void Undo()
    {
        _project?.RestoreAnimation(_animation);
    }
}
