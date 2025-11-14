using System;
using System.Collections.Generic;
using AnimEngine;

namespace AnimModels
{
    public class Animation
    {
        public string name = "anim0";
        public double currentTime = 0;
        public bool isRun = false;
        public SkeletonInAnimation skeletonInAnimation = new SkeletonInAnimation();

        public Animation() { }

        public Animation(string name)
        {
            this.name = name;
        }

        public void playOrPause()
        {
            if (this.isRun == false)
            {
                this.currentTime = 0;
            }
            this.isRun = !this.isRun;
        }

        public void run()
        {
            skeletonInAnimation.animationStep(this.currentTime);
            currentTime += Math.Round(currentTime + 0.0167, 4);
        }
    }
}
