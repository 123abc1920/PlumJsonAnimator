using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

public class BoneStatus
{
    public readonly double X,
        Y,
        A;

    private BoneStatus(double X, double Y, double A)
    {
        this.X = X;
        this.Y = Y;
        this.A = A;
    }

    public BoneStatus(Bone b)
    {
        this.X = b.X;
        this.Y = b.Y;
        this.A = b.A;
    }

    public BoneStatus Copy()
    {
        return new BoneStatus(this.X, this.Y, this.A);
    }
}

public class ChangeBoneStatusCommand : ICommand
{
    private readonly Bone _bone;
    private readonly BoneStatus oldStatus;
    private readonly BoneStatus newStatus;

    private readonly IKeyframeType oldT;
    private readonly IKeyframeType newT;
    private readonly IKeyframeType oldR;
    private readonly IKeyframeType newR;

    public ChangeBoneStatusCommand(
        Bone bone,
        BoneStatus oldStatus,
        BoneStatus newStatus,
        Animation animation,
        bool isAnim
    )
    {
        _bone = bone;

        this.oldStatus = oldStatus;
        this.newStatus = newStatus;

        if (isAnim)
        {
            oldT = animation.GetKeyframe(
                TransformModesTypes.TRANSLATE,
                animation.currentTime,
                _bone
            );
            oldR = animation.GetKeyframe(TransformModesTypes.ROTATE, animation.currentTime, _bone);
        }
    }

    public void Execute()
    {
        _bone.X = newStatus.X;
        _bone.Y = newStatus.Y;
        _bone.A = newStatus.A;
    }

    public void Undo()
    {
        _bone.X = oldStatus.X;
        _bone.Y = oldStatus.Y;
        _bone.A = oldStatus.A;
    }
}
