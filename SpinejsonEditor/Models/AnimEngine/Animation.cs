using System;
using System.Collections.Generic;
using System.ComponentModel;
using AnimEngine;
using Newtonsoft.Json;

namespace AnimModels
{
    public class Animation : INotifyPropertyChanged
    {
        public string name = "anim0";
        public string AnimationName
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnimationName)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public double currentTime = 0;
        public bool isRun = false;
        public SkeletonInAnimation skeletonInAnimation = new SkeletonInAnimation();

        public Animation() { }

        public Animation(string name)
        {
            this.name = name;
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
            animationData[this.AnimationName] = this.skeletonInAnimation.generateJSONData();
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
