using System;
using System.Collections.Generic;
using System.ComponentModel;
using AnimEngine;

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

        public String generateCode()
        {
            String code = "";

            code += "\"" + this.AnimationName + "\": {\"bones\": {";

            code += this.skeletonInAnimation.generateCode();

            code += "}}";
            return code;
        }
    }
}
