using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddKeyFrameCommand : ICommand
{
    private readonly Animation _animation;
    private readonly Bone _bone;
    private readonly double _time;
    private readonly TransformModeTypes _type;
    private IKeyframeType _keyframe;

    public AddKeyFrameCommand(Animation animation, Bone bone, TransformModeTypes type)
    {
        this._animation = animation;
        this._bone = bone;
        this._type = type;

        this._time = animation.CurrentTime;
    }

    public void Execute()
    {
        _animation?.AddKeyFrame(_bone, _type, _time);
        this._keyframe = _animation.GetKeyframe(this._type, this._time, this._bone);
    }

    public void Undo()
    {
        _animation?.DeleteKeyFrame(_bone, _type, _time);
    }
}
