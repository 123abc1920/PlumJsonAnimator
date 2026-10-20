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

    private BoneStatus(double x, double y, double a)
    {
        X = x;
        Y = y;
        A = a;
    }

    public BoneStatus(Bone b)
    {
        X = b.X;
        Y = b.Y;
        A = b.A;
    }

    public BoneStatus Copy()
    {
        BoneStatus newBone = new BoneStatus(X, Y, A);
        newBone.T = T;
        newBone.R = R;
        return newBone;
    }
}

public class ChangeBoneStatusCommand : ICommand
{
    private readonly Bone _bone;
    private readonly BoneStatus _oldStatus;
    private readonly BoneStatus _newStatus;
    private readonly Animation _animation;
    private readonly bool _isAnim;
    private readonly double _time;

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

        _oldStatus = oldStatus;
        _newStatus = newStatus;
        _animation = animation;
        _isAnim = isAnim;
        _time = time;
    }

    public void Execute()
    {
        _bone.X = _newStatus.X;
        _bone.Y = _newStatus.Y;
        _bone.A = _newStatus.A;

        if (_isAnim == true)
        {
            _animation.SetKeyFrame(TransformModeTypes.TRANSLATE, _time, _newStatus.T, _bone);
            _animation.SetKeyFrame(TransformModeTypes.ROTATE, _time, _newStatus.R, _bone);
        }
    }

    public void Undo()
    {
        _bone.X = _oldStatus.X;
        _bone.Y = _oldStatus.Y;
        _bone.A = _oldStatus.A;

        if (_isAnim == true)
        {
            _animation.SetKeyFrame(TransformModeTypes.TRANSLATE, _time, _oldStatus.T, _bone);
            _animation.SetKeyFrame(TransformModeTypes.ROTATE, _time, _oldStatus.R, _bone);
        }
    }
}
