using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common.TransformModes;

/// <summary>
/// Mode for no transformations
/// </summary>
public abstract class Mode
{
    public TransformModeTypes Type { get; set; }
    public string Name { get; set; } = "";

    protected GlobalState _globalState;

    public Mode(GlobalState globalState)
    {
        _globalState = globalState;
    }

    public abstract void ClearMode();

    public abstract void Transform(Bone bone, double a, double b, IEasing easing);
}
