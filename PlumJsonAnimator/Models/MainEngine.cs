using System;
using Avalonia.Threading;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Models
{
    public class Engine
    {
        private DispatcherTimer _animationLoop = new DispatcherTimer();

        private int FPS;

        public Engine(int FPS)
        {
            this._animationLoop.Interval = TimeSpan.FromSeconds(1.0 / (double)FPS);
        }

        public void AddCustomTickHandler(EventHandler handler)
        {
            this._animationLoop.Tick += handler;
        }

        public void runAnimation()
        {
            /*if (ConstantsClass.currentProject.CurrentAnimation.IsRun)
            {
                this._animationLoop.Stop();
                ConstantsClass.currentProject.CurrentAnimation.IsRun = false;
            }
            else
            {
                this._animationLoop.Start();
                ConstantsClass.currentProject.CurrentAnimation.IsRun = true;
            }*/
        }
    }
}
