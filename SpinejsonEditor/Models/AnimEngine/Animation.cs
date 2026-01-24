using System;
using System.Collections.Generic;
using System.ComponentModel;
using AnimEngine;
using Newtonsoft.Json;

namespace AnimModels
{
    public class Animation : INotifyPropertyChanged
    {
        public string Name { get; set; } = "anim0";

        public event PropertyChangedEventHandler? PropertyChanged;
        public double currentTime = 0;
        public bool isRun = false;
        public SkeletonInAnimation skeletonInAnimation = new SkeletonInAnimation();
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
            //skeletonInAnimation.animationStep(this.currentTime);
            foreach (Bone b in BoneAnimationBinding.Keys)
            {
                BoneAnimationBinding[b].BoneStep(b, currentTime);
            }
            currentTime += 0.01666667;
        }

        public AnimationData generateJSONData()
        {
            var animationData = new AnimationData();
            animationData[this.Name] = this.skeletonInAnimation.generateJSONData();
            return animationData;
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

        public String generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
        }
    }
}

public class AnimationData : Dictionary<string, SkeletonInAnimationData> { }
