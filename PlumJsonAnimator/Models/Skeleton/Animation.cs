using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
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

        public double currentTime = 0;
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

        public Animation(GlobalState globalState, Interpolation interpolation)
        {
            this._globalState = globalState;
            this._interpolation = interpolation;
        }

        public Animation(GlobalState globalState, Interpolation interpolation, string name)
        {
            this._globalState = globalState;
            this._interpolation = interpolation;
            this.Name = name;
        }

        /// <summary>
        /// Sets all bones according to the current time
        /// </summary>
        public void SetupBones()
        {
            foreach (Bone b in BoneAnimationBinding.Keys)
            {
                BoneAnimationBinding[b].BoneStep(b, currentTime);
            }
        }

        /// <summary>
        /// Makes animation step
        /// </summary>
        public void Step()
        {
            this.currentTime += 1.0 / (double)this._globalState.FPS;
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
            if (this.ContainsBone(bone) == true)
            {
                this.BoneAnimationBinding.Remove(bone);
            }
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

            foreach (Bone b in this._globalState.CurrentProject.MainSkeleton.Bones)
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
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }

        /// <summary>
        /// Collects data about keyframes for drawing in ui
        /// </summary>
        /// <returns>A dictionary, contains time-keys and another dictionary with keyframes</returns>
        public Dictionary<double, Dictionary<KeyFrameTypes, bool>> GetKeyFramesMarks(Bone b)
        {
            Dictionary<double, Dictionary<KeyFrameTypes, bool>> result =
                new Dictionary<double, Dictionary<KeyFrameTypes, bool>>();

            if (b != null && BoneAnimationBinding.ContainsKey(b))
            {
                BoneAnimation ba = BoneAnimationBinding[b];
                result = ba.GetKeyFeamesMarks();
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
                BoneAnimationBinding.Add(
                    b,
                    new BoneAnimation(this._globalState, this._interpolation)
                );
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
                BoneAnimationBinding[b].AddTranslateFrame(currentTime, (double)x, (double)y);
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
                BoneAnimationBinding[b].AddRotateFrame(currentTime, (double)value);
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
        public double FindKeyFrame(Bone b, double time, TransformModesTypes type, bool isNext)
        {
            if (BoneAnimationBinding.ContainsKey(b) && type != TransformModesTypes.NO)
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
        public void AddKeyFrame(Bone b, TransformModesTypes type)
        {
            if (b != null && b.IsBone && type != TransformModesTypes.NO)
            {
                if (type == TransformModesTypes.TRANSLATE)
                {
                    TranslateBone(b, b.X, b.Y);
                }
                if (type == TransformModesTypes.ROTATE)
                {
                    RotateBone(b, b.A);
                }
                if (type == TransformModesTypes.SCALE) { }
                if (type == TransformModesTypes.SHEAR) { }
            }
        }

        /// <summary>
        /// Delete keyframe
        /// </summary>
        /// <param name="b">Bone</param>
        /// <param name="type">Current transform type</param>
        public void DeleteKeyFrame(Bone b, TransformModesTypes type)
        {
            if (b != null && b.IsBone && type != TransformModesTypes.NO)
            {
                if (BoneAnimationBinding.ContainsKey(b))
                {
                    BoneAnimation ba = BoneAnimationBinding[b];
                    ba.DeleteKeyFrame(currentTime, type);
                }
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
}
