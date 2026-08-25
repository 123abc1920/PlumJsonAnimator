using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

public class BoneStatus
{
    public readonly double X,
        Y,
        A;

    public IKeyframeType T;
    public IKeyframeType R;

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
        BoneStatus newBone = new BoneStatus(this.X, this.Y, this.A);
        newBone.T = this.T;
        newBone.R = this.R;
        return newBone;
    }
}

public class ChangeBoneStatusCommand : ICommand
{
    private readonly Bone _bone;
    private readonly BoneStatus oldStatus;
    private readonly BoneStatus newStatus;
    private readonly Animation animation;
    private readonly bool isAnim;
    private readonly double time;

    public ChangeBoneStatusCommand(
        Bone bone,
        BoneStatus oldStatus,
        BoneStatus newStatus,
        Animation animation,
        bool isAnim,
        double time
    )
    {
        _bone = bone;

        this.oldStatus = oldStatus;
        this.newStatus = newStatus;
        this.animation = animation;
        this.isAnim = isAnim;
        this.time = time;
    }

    public void Execute()
    {
        _bone.X = newStatus.X;
        _bone.Y = newStatus.Y;
        _bone.A = newStatus.A;

        if (isAnim)
        {
            animation.SetKeyFrame(TransformModesTypes.TRANSLATE, time, newStatus.T, _bone);
            animation.SetKeyFrame(TransformModesTypes.ROTATE, time, newStatus.R, _bone);
        }
    }

    public void Undo()
    {
        _bone.X = oldStatus.X;
        _bone.Y = oldStatus.Y;
        _bone.A = oldStatus.A;

        if (isAnim)
        {
            if (isAnim)
            {
                animation.SetKeyFrame(TransformModesTypes.TRANSLATE, time, oldStatus.T, _bone);
                animation.SetKeyFrame(TransformModesTypes.ROTATE, time, oldStatus.R, _bone);
            }
        }
    }
}
