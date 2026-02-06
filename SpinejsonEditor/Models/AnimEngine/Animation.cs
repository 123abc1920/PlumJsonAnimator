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

        public bool playOrPause()
        {
            if (this.isRun == false)
            {
                this.currentTime = 0;
            }
            this.isRun = !this.isRun;

            return this.isRun;
        }

        public void step()
        {
            foreach (Bone b in BoneAnimationBinding.Keys)
            {
                BoneAnimationBinding[b].BoneStep(b, currentTime);
            }
            currentTime += 0.01666667; // FPS=60
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

        public void TranslateBone(Bone b, double x, double y)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].addTranslateFrame(currentTime, x, y);
        }

        public void RotateBone(Bone b, double value)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].addRotateFrame(currentTime, value);
        }

        public void ScaleBone(Bone b, double x, double y)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].addScaleFrame(currentTime, x, y);
        }

        public void ShearBone(Bone b, double x, double y)
        {
            AnimateBone(b);
            BoneAnimationBinding[b].addShearFrame(currentTime, x, y);
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
