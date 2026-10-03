using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models.SkeletonNameSpace;

/// <summary>
/// Provides methods for work with animations
/// </summary>
public class Animation : INotifyable
{
    private string _name = "anim0";
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public double CurrentTime = 0;
    private bool _isRun = false;
    public bool IsRun
    {
        get => _isRun;
        set
        {
            if (_isRun != value)
            {
                _isRun = value;
                OnPropertyChanged(nameof(IsRun));
            }
        }
    }
    public Dictionary<Bone, BoneAnimation> BoneAnimationBinding =
        new Dictionary<Bone, BoneAnimation>();

    private GlobalState _globalState;
    private Interpolation _interpolation;
    private EasingFactory _easingFactory;

    public Animation(
        GlobalState globalState,
        Interpolation interpolation,
        EasingFactory easingFactory
    )
    {
        _globalState = globalState;
        _interpolation = interpolation;
        _easingFactory = easingFactory;
    }

    public Animation(
        GlobalState globalState,
        Interpolation interpolation,
        EasingFactory easingFactory,
        string name
    )
        : this(globalState, interpolation, easingFactory)
    {
        Name = name;
    }

    /// <summary>
    /// Sets all bones according to the current time
    /// </summary>
    public void SetupBones()
    {
        foreach (Bone b in BoneAnimationBinding.Keys)
        {
            BoneAnimationBinding[b].BoneStep(b, CurrentTime);
            UpdateDrawOrder(b.Slots);
        }
    }

    public void UpdateDrawOrder(ObservableCollection<Slot> slots)
    {
        foreach (Slot s in slots)
        {
            s.UpdateDrawOrderOffset();
        }
    }

    public void UpdateAllDrawOrder()
    {
        foreach (Bone b in BoneAnimationBinding.Keys)
        {
            UpdateDrawOrder(b.Slots);
        }
    }

    /// <summary>
    /// Makes animation step
    /// </summary>
    public void Step()
    {
        CurrentTime += 1.0 / (double)_globalState.FPS;
        SetupBones();
    }

    /// <summary>
    /// Checks whether a bone is involved in an animation
    /// </summary>
    public bool ContainsBone(Bone bone)
    {
        return BoneAnimationBinding.ContainsKey(bone);
    }

    /// <summary>
    /// Checks if the bone has any movement
    /// </summary>
    public bool ContainsAnimationBone(BoneAnimation boneAnimation)
    {
        return BoneAnimationBinding.ContainsValue(boneAnimation);
    }

    public void DeleteBoneFromAnimation(Bone bone)
    {
        if (ContainsBone(bone) == true)
        {
            BoneAnimationBinding.Remove(bone);
        }
    }

    public void RestoreBoneAnimation(Bone bone, BoneAnimation? boneAnimation)
    {
        if (bone == null || boneAnimation == null)
        {
            return;
        }

        BoneAnimationBinding[bone] = boneAnimation;
    }

    public BoneAnimation GetBoneAnimation(Bone bone)
    {
        return BoneAnimationBinding[bone];
    }

    /// <summary>
    /// Turn animation data into JSON object
    /// </summary>
    public AnimationData GenerateJSONData()
    {
        var animationData = new AnimationData();

        var boneListData = new BonesListData();
        foreach (Bone b in BoneAnimationBinding.Keys)
        {
            boneListData.Add(b.Name, BoneAnimationBinding[b].GenerateJSONData());
        }
        animationData.Bones = boneListData;

        var drawOrders = new List<DrawOrderItem>();

        foreach (Bone b in _globalState.CurrentProject.MainSkeleton.Bones)
        {
            var slots = b.Slots;
            Dictionary<double, DrawOrderItem> drawOrderItems =
                new Dictionary<double, DrawOrderItem>();

            foreach (Slot s in slots)
            {
                foreach (var kv in s.drawOrders)
                {
                    if (drawOrderItems.Keys.Contains(kv.Key))
                    {
                        drawOrderItems[kv.Key].Offsets?.Add(kv.Value);
                    }
                    else
                    {
                        drawOrderItems.Add(
                            kv.Key,
                            new DrawOrderItem()
                            {
                                Time = (float)kv.Key,
                                Offsets = new List<DrawOrderOffset>() { kv.Value },
                            }
                        );
                    }
                }

                foreach (var v in drawOrderItems.Values)
                {
                    if (!drawOrders.Contains(v))
                    {
                        drawOrders.Add(v);
                    }
                }
            }
        }

        animationData.DrawOrder = drawOrders;

        return animationData;
    }

    /// <summary>
    /// Turn animation JSON object into JSON string
    /// </summary>
    public String GenerateCode()
    {
        return JsonConvert.SerializeObject(GenerateJSONData(), _globalState.jsonSettings);
    }

    /// <summary>
    /// Collects data about keyframes for drawing in ui
    /// </summary>
    /// <returns>A dictionary, contains time-keys and another dictionary with keyframes</returns>
    public Dictionary<double, Dictionary<TransformModeTypes, bool>> GetKeyFramesMarks(Bone b)
    {
        Dictionary<double, Dictionary<TransformModeTypes, bool>> result =
            new Dictionary<double, Dictionary<TransformModeTypes, bool>>();

        if (b != null && BoneAnimationBinding.ContainsKey(b))
        {
            BoneAnimation ba = BoneAnimationBinding[b];
            result = ba.GetKeyFeamesMarks();
        }

        return result;
    }

    /// <summary>
    /// Collects all bones animation keyframes
    /// </summary>
    /// <returns>Dictionary of bone and its keyframes</returns>
    public Dictionary<
        Bone,
        Dictionary<double, Dictionary<TransformModeTypes, bool>>
    > GetAllKeyFrameMarks()
    {
        Dictionary<Bone, Dictionary<double, Dictionary<TransformModeTypes, bool>>> result =
            new Dictionary<Bone, Dictionary<double, Dictionary<TransformModeTypes, bool>>>();

        foreach (var element in BoneAnimationBinding)
        {
            result.Add(element.Key, element.Value.GetKeyFeamesMarks());
        }

        return result;
    }

    /// <summary>
    /// Add animation to bone if it hasn`t
    /// </summary>
    /// <param name="b"></param>
    private void AnimateBone(Bone b)
    {
        if (b == null)
        {
            return;
        }

        if (!BoneAnimationBinding.ContainsKey(b))
        {
            BoneAnimationBinding.Add(b, new BoneAnimation(_globalState, _interpolation));
        }
    }

    /// <summary>
    /// Add bone translating into current animation
    /// </summary>
    /// <param name="b">Bone that has to be moved</param>
    /// <param name="x">Target x coordinate</param>
    /// <param name="y">Target y coordinate</param>
    public void TranslateBone(Bone b, double? x, double? y)
    {
        if (b != null && x != null && y != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddTranslateFrame(CurrentTime, (double)x, (double)y);
        }
    }

    /// <summary>
    /// Add bone translating into current animation
    /// </summary>
    /// <param name="b">Bone that has to be moved</param>
    /// <param name="x">Target x coordinate</param>
    /// <param name="y">Target y coordinate</param>
    /// <param name="currTime">Target time</param>
    public void TranslateBone(Bone b, double? x, double? y, double? currTime)
    {
        if (b != null && x != null && y != null && currTime != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddTranslateFrame((double)currTime, (double)x, (double)y);
        }
    }

    /// <summary>
    /// Add bone rotating into current animation
    /// </summary>
    /// <param name="b">Bone that has to be moved</param>
    /// <param name="value">Target angle</param>
    public void RotateBone(Bone b, double? value)
    {
        if (b != null && value != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddRotateFrame(CurrentTime, (double)value);
        }
    }

    public void ShearBone(Bone b, double? shearX, double? shearY)
    {
        if (b != null && shearX != null && shearY != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddShearFrame(CurrentTime, (double)shearX, (double)shearY);
        }
    }

    public void ShearBone(Bone b, double? shearX, double? shearY, double? time)
    {
        if (b != null && shearX != null && shearY != null && time != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddShearFrame((double)time, (double)shearX, (double)shearY);
        }
    }

    public void ScaleBone(Bone b, double? scaleX, double? scaleY)
    {
        if (b != null && scaleX != null && scaleY != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddScaleFrame(CurrentTime, (double)scaleX, (double)scaleY);
        }
    }

    public void ScaleBone(Bone b, double? scaleX, double? scaleY, double? time)
    {
        if (b != null && scaleX != null && scaleY != null && time != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddScaleFrame((double)time, (double)scaleX, (double)scaleY);
        }
    }

    /// <summary>
    /// Add bone translating into current animation
    /// </summary>
    /// <param name="b">Bone that has to be moved</param>
    /// <param name="value">Target angle</param>
    /// <param name="currTime">Target time</param>
    public void RotateBone(Bone b, double? value, double? currTime)
    {
        if (b != null && value != null && currTime != null)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].AddRotateFrame((double)currTime, (double)value);
        }
    }

    /// <summary>
    /// Find current keyframes
    /// </summary>
    /// <param name="b">Bone</param>
    /// <param name="time">Current time</param>
    /// <param name="type">Current transform type</param>
    /// <param name="isNext">Next or previous keyframe</param>
    public double FindKeyFrame(Bone b, double time, TransformModeTypes type, bool isNext)
    {
        if (BoneAnimationBinding.ContainsKey(b) && type != TransformModeTypes.NO)
        {
            return BoneAnimationBinding[b].FindTime(time, type, isNext);
        }
        return time;
    }

    /// <summary>
    /// Add keyframe to animation via UI
    /// </summary>
    /// <param name="b">Bone</param>
    /// <param name="type">Current transform type</param>
    /// <param name="time">Current time</param>
    public void AddKeyFrame(Bone b, TransformModeTypes type, double time)
    {
        if (b != null && b.IsBone && type != TransformModeTypes.NO)
        {
            if (type == TransformModeTypes.TRANSLATE)
            {
                TranslateBone(b, b.X, b.Y, time);
            }
            if (type == TransformModeTypes.ROTATE)
            {
                RotateBone(b, b.A, time);
            }
            if (type == TransformModeTypes.SCALE) { }
            if (type == TransformModeTypes.SHEAR) { }
        }
    }

    public void RestoreKeyFrame(
        IKeyframeType keyframe,
        Bone bone,
        double time,
        TransformModeTypes type
    )
    {
        BoneAnimation boneAnimation = BoneAnimationBinding[bone];
        boneAnimation.RestoreKeyFrame(keyframe, time, type);
    }

    /// <summary>
    /// Delete keyframe
    /// </summary>
    /// <param name="b">Bone</param>
    /// <param name="type">Current transform type</param>
    /// <param name="time">Current time</param>
    public void DeleteKeyFrame(Bone b, TransformModeTypes type, double time)
    {
        if (b != null && b.IsBone && type != TransformModeTypes.NO)
        {
            if (BoneAnimationBinding.ContainsKey(b))
            {
                BoneAnimation ba = BoneAnimationBinding[b];
                ba.DeleteKeyFrame(time, type);
            }
        }
    }

    public IKeyframeType? GetKeyframe(TransformModeTypes type, double time, Bone bone)
    {
        if (BoneAnimationBinding.TryGetValue(bone, out var animation))
        {
            return animation.GetKeyFrame(type, time);
        }
        return null;
    }

    public void SetKeyFrame(TransformModeTypes type, double time, IKeyframeType keyframe, Bone bone)
    {
        if (BoneAnimationBinding.TryGetValue(bone, out var boneAnimation))
        {
            boneAnimation.SetKeyFrame(type, time, keyframe);
        }
    }

    /// <summary>
    /// Animation end time
    /// </summary>
    public double MaxTime()
    {
        double maxTime = 0.0;
        foreach (var b in BoneAnimationBinding)
        {
            if (b.Value != null)
            {
                maxTime = Math.Max(maxTime, b.Value.MaxTime());
            }
        }
        return maxTime;
    }

    public SortedDictionary<double, IKeyframeType>? GetKeyFrameLine(Mode mode, Bone b)
    {
        if (!BoneAnimationBinding.ContainsKey(b))
            return null;

        BoneAnimation boneAnimation = BoneAnimationBinding[b];
        return boneAnimation.GetKeyFrameLine(mode.Type);
    }

    public IKeyframeType? FindKeyFrameByTime(
        Bone? b,
        TransformModeTypes transformModeType,
        double time
    )
    {
        if (b is null)
            return null;
        var boneAnimation = BoneAnimationBinding[b];
        return boneAnimation.FindKeyFrame(time, transformModeType);
    }

    public double? FindNextTime(double time, Bone? bone, TransformModeTypes transformModeType)
    {
        if (bone is null || ContainsBone(bone) == false)
            return 0;

        BoneAnimation boneAnimation = BoneAnimationBinding[bone];
        return boneAnimation.FindNextTime(time, transformModeType);
    }

    public double? FindKeyFrameTime(double time, Bone? bone, TransformModeTypes transformModeType)
    {
        if (bone is null || ContainsBone(bone) == false)
            return 0;

        BoneAnimation boneAnimation = BoneAnimationBinding[bone];
        return boneAnimation.FindTime(time, transformModeType, false);
    }

    public void ChangeCurrentEasingMode(
        Bone? b,
        EasingTypes newEasingType,
        TransformModeTypes transformModeType
    )
    {
        if (b is null)
            return;

        var keyFrame = FindKeyFrameByTime(b, transformModeType, CurrentTime);

        if (keyFrame is null)
            return;

        keyFrame.Curve = _easingFactory.CreateEasing(newEasingType);
    }
}

/// <summary>
/// Animation data
/// </summary>
public class AnimationData
{
    [JsonProperty("bones")]
    public BonesListData? Bones { get; set; }

    [JsonProperty("drawOrder")]
    public List<DrawOrderItem>? DrawOrder { get; set; }
}

/// <summary>
/// List of bones
/// </summary>
public class BonesListData : Dictionary<string, BoneAnimationData> { }

/// <summary>
/// Draw order list
/// </summary>
public class DrawOrderItem
{
    [JsonProperty("time")]
    public float? Time { get; set; }

    [JsonProperty("offsets")]
    public List<DrawOrderOffset>? Offsets { get; set; }
}

/// <summary>
/// Draw order list item
/// </summary>
public class DrawOrderOffset
{
    [JsonProperty("slot")]
    public required string Slot { get; set; }

    [JsonProperty("offset")]
    public int Offset { get; set; }
}
