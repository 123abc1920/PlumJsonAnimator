using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
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

        private GlobalState globalState;
        private Interpolation interpolation;

        public Animation(GlobalState globalState, Interpolation interpolation)
        {
            this.globalState = globalState;
            this.interpolation = interpolation;
        }

        public Animation(GlobalState globalState, Interpolation interpolation, string name)
        {
            this.globalState = globalState;
            this.interpolation = interpolation;
            this.Name = $"{name}{Counter.GenerateName()}";
        }

        public void SetupBones()
        {
            /*var parallelOptions = this.globalState.GetParallelOptions();
            Parallel.ForEach(
                BoneAnimationBinding.Keys,
                parallelOptions,
                b => BoneAnimationBinding[b].BoneStep(b, currentTime)
            );*/
            foreach (Bone b in BoneAnimationBinding.Keys)
            {
                BoneAnimationBinding[b].BoneStep(b, currentTime);
            }
        }

        public void step()
        {
            this.currentTime += 1.0 / (double)this.globalState.FPS;
            SetupBones();
        }

        public AnimationData generateJSONData()
        {
            var animationData = new AnimationData();

            var boneListData = new BonesListData();
            foreach (Bone b in BoneAnimationBinding.Keys)
            {
                boneListData.Add(b.Name, BoneAnimationBinding[b].generateJSONData());
            }
            animationData.Bones = boneListData;

            var drawOrders = new List<DrawOrderItem>();

            foreach (Bone b in BoneAnimationBinding.Keys)
            {
                var slots = b.Slots;
                DrawOrderItem drawOrderItem = new DrawOrderItem()
                {
                    Time = 0,
                    Offsets = new List<DrawOrderOffset>(),
                };

                foreach (Slot s in slots)
                {
                    DrawOrderOffset drawOrderOffset = new DrawOrderOffset()
                    {
                        Slot = s.Name,
                        Offset = s.DrawOrderOffset,
                    };

                    drawOrderItem.Offsets.Add(drawOrderOffset);
                }

                drawOrders.Add(drawOrderItem);
            }

            animationData.DrawOrder = drawOrders;

            return animationData;
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), this.globalState.jsonSettings);
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
                    new BoneAnimation(this.globalState, this.interpolation)
                );
            }
        }

        public void TranslateBone(Bone b, double? x, double? y)
        {
            if (x != null && y != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addTranslateFrame(currentTime, (double)x, (double)y);
            }
        }

        public void TranslateBone(Bone b, double? x, double? y, double? currTime)
        {
            if (x != null && y != null && currTime != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addTranslateFrame((double)currTime, (double)x, (double)y);
            }
        }

        public void RotateBone(Bone b, double? value)
        {
            if (value != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addRotateFrame(currentTime, (double)value);
            }
        }

        public void RotateBone(Bone b, double? value, double? currTime)
        {
            if (value != null && currTime != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addRotateFrame((double)currTime, (double)value);
            }
        }

        public void ScaleBone(Bone b, double? x, double? y)
        {
            if (x != null && y != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addScaleFrame(currentTime, (double)x, (double)y);
            }
        }

        public void ScaleBone(Bone b, double? x, double? y, double? currTime)
        {
            if (x != null && y != null && currTime != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addScaleFrame((double)currTime, (double)x, (double)y);
            }
        }

        public void ShearBone(Bone b, double? x, double? y)
        {
            if (x != null && y != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addShearFrame(currentTime, (double)x, (double)y);
            }
        }

        public void ShearBone(Bone b, double? x, double? y, double? currTime)
        {
            if (x != null && y != null && currTime != null)
            {
                AnimateBone(b);
                BoneAnimationBinding[b].addShearFrame((double)currTime, (double)x, (double)y);
            }
        }

        public double FindKeyFrame(Bone b, double time, TransformModesTypes type, bool isNext)
        {
            if (BoneAnimationBinding.ContainsKey(b) && type != TransformModesTypes.NO)
            {
                return BoneAnimationBinding[b].FindTime(time, type, isNext);
            }
            return time;
        }

        public void AddKeyFrame(Bone b, TransformModesTypes type)
        {
            if (b != null && b.IsBone && type != TransformModesTypes.NO)
            {
                if (type == TransformModesTypes.TRANSLATE)
                {
                    TranslateBone(b, b.x, b.y);
                }
                if (type == TransformModesTypes.ROTATE)
                {
                    RotateBone(b, b.a);
                }
                if (type == TransformModesTypes.SCALE) { }
                if (type == TransformModesTypes.SHEAR) { }
            }
        }

        public void DeleteKeyFrame(Bone b, TransformModesTypes type)
        {
            if (b != null && b.IsBone && type != TransformModesTypes.NO)
            {
                if (BoneAnimationBinding.ContainsKey(b))
                {
                    BoneAnimation ba = BoneAnimationBinding[b];
                    ba.deleteKeyFrame(currentTime, type);
                }
            }
        }

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

    public class AnimationData
    {
        [JsonProperty("bones")]
        public BonesListData? Bones { get; set; }

        [JsonProperty("drawOrder")]
        public List<DrawOrderItem>? DrawOrder { get; set; }
    }

    public class BonesListData : Dictionary<string, BoneAnimationData> { }

    public class DrawOrderItem
    {
        [JsonProperty("time")]
        public float? Time { get; set; }

        [JsonProperty("offsets")]
        public List<DrawOrderOffset>? Offsets { get; set; }
    }

    public class DrawOrderOffset
    {
        [JsonProperty("slot")]
        public required string Slot { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }
    }
}
