using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DeleteKeyFrameCommand : ICommand
{
    private readonly Animation _animation;
    private readonly Bone _bone;
    private readonly double _time;
    private readonly TransformModeTypes _type;
    private readonly IKeyframeType _keyframe;

    public DeleteKeyFrameCommand(Animation animation, Bone bone, TransformModeTypes type)
    {
        _animation = animation;
        _bone = bone;
        _type = type;

        _time = animation.CurrentTime;

        _keyframe = animation.GetKeyframe(_type, _time, _bone);
    }

    public void Execute()
    {
        _animation?.DeleteKeyFrame(_bone, _type, _time);
    }

    public void Undo()
    {
        _animation?.RestoreKeyFrame(_keyframe, _bone, _time, _type);
    }
}
