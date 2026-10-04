using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddKeyFrameCommand : ICommand
{
    private readonly Animation _animation;
    private readonly Bone _bone;
    private readonly double _time;
    private readonly TransformModeTypes _type;
    private IKeyframeType _keyframe;
    private IEasing _easing;

    public AddKeyFrameCommand(
        Animation animation,
        Bone bone,
        TransformModeTypes type,
        IEasing easing
    )
    {
        _animation = animation;
        _bone = bone;
        _type = type;
        _easing = easing;
        _time = animation.CurrentTime;
    }

    public void Execute()
    {
        _animation?.AddKeyFrame(_bone, _type, _time, _easing);
        _keyframe = _animation.GetKeyframe(_type, _time, _bone);
    }

    public void Undo()
    {
        _animation?.DeleteKeyFrame(_bone, _type, _time);
    }
}
