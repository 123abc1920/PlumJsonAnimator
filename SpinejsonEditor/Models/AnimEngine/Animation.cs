using System;
using System.Collections.Generic;
using System.ComponentModel;
using AnimTransformations;
using Newtonsoft.Json;
using TransformModes;

namespace AnimModels
{
    public class Animation : INotifyPropertyChanged
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public double currentTime = 0;
        public bool isRun = false;
        public Dictionary<Bone, BoneAnimation> BoneAnimationBinding =
            new Dictionary<Bone, BoneAnimation>();

        public Animation() { }

        public Animation(string name)
        {
            this.Name = name;
        }

        public void SetupBones()
        {
            foreach (Bone b in BoneAnimationBinding.Keys)
            {
                BoneAnimationBinding[b].BoneStep(b, currentTime);
            }
        }

        public void step()
        {
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
            animationData["bones"] = boneListData;
            return animationData;
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
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
            if (!BoneAnimationBinding.ContainsKey(b))
            {
                BoneAnimationBinding.Add(b, new BoneAnimation());
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
            if (b != null && b.isBone && type != TransformModesTypes.NO)
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
            if (b != null && b.isBone && type != TransformModesTypes.NO)
            {
                if (BoneAnimationBinding.ContainsKey(b))
                {
                    BoneAnimation ba = BoneAnimationBinding[b];
                    ba.deleteKeyFrame(currentTime, type);
                }
            }
        }
    }
}

public class BonesListData : Dictionary<string, BoneAnimationData> { }

public class AnimationData : Dictionary<string, BonesListData> { }
