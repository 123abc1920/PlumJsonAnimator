using System;
using Avalonia.Threading;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Services
{
    public class Engine
    {
        private DispatcherTimer _animationLoop = new DispatcherTimer();

        private GlobalState globalState;

        public Engine(GlobalState globalState)
        {
            this.globalState = globalState;

            this._animationLoop.Interval = TimeSpan.FromSeconds(1.0 / (double)this.globalState.FPS);
            this._animationLoop.Tick += AnimStep;
        }

        private void AnimStep(object? sender, EventArgs e)
        {
            this.globalState.currentProject.CurrentAnimation.step();
            this.globalState.OnTimeUpdated();
        }

        public void runAnimation()
        {
            if (this.globalState.currentProject.CurrentAnimation.IsRun)
            {
                this._animationLoop.Stop();
                this.globalState.currentProject.CurrentAnimation.IsRun = false;
            }
            else
            {
                this._animationLoop.Start();
                this.globalState.currentProject.CurrentAnimation.IsRun = true;
            }
        }
    }
}
