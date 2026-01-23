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
        public Dictionary<Bone, BoneInAnimation> BoneAnimationBinding =
            new Dictionary<Bone, BoneInAnimation>();

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

        public void run()
        {
            skeletonInAnimation.animationStep(this.currentTime);
            currentTime += 0.01666667;
        }

        public AnimationData generateJSONData()
        {
            var animationData = new AnimationData();
            animationData[this.Name] = this.skeletonInAnimation.generateJSONData();
            return animationData;
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
