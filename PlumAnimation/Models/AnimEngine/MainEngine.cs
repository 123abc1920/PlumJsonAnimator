using System;
using AnimModels;
using Avalonia.Threading;
using Constants;

namespace AnimEngine
{
    public class Engine
    {
        private DispatcherTimer _animationLoop = new DispatcherTimer();

        public Engine()
        {
            this._animationLoop.Interval = TimeSpan.FromSeconds(1.0 / (double)ConstantsClass.FPS);
        }

        public void AddCustomTickHandler(EventHandler handler)
        {
            this._animationLoop.Tick += handler;
        }

        public void runAnimation()
        {
            if (ConstantsClass.currentProject.CurrentAnimation.IsRun)
            {
                this._animationLoop.Stop();
                ConstantsClass.currentProject.CurrentAnimation.IsRun = false;
            }
            else
            {
                this._animationLoop.Start();
                ConstantsClass.currentProject.CurrentAnimation.IsRun = true;
            }
        }
    }
}
