using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class DeleteKeyFrameCommand : ICommand
{
    private readonly Animation _animation;
    private readonly Bone _bone;
    private readonly double _time;
    private readonly TransformModesTypes _type;
    private readonly IKeyframeType _keyframe;

    public DeleteKeyFrameCommand(Animation animation, Bone bone, TransformModesTypes type)
    {
        this._animation = animation;
        this._bone = bone;
        this._type = type;

        this._time = animation.currentTime;

        this._keyframe = animation.GetKeyframe(this._type, this._time, this._bone);
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
