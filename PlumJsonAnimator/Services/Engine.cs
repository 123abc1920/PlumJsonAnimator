using System;
using Avalonia.Controls;
using Avalonia.Threading;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Plays animation and update time
    /// </summary>
    public class Engine
    {
        private DispatcherTimer _animationLoop = new DispatcherTimer();
        private Animation? currentAnimation;

        private GlobalState globalState;

        public Engine(GlobalState globalState)
        {
            this.globalState = globalState;

            this._animationLoop.Interval = TimeSpan.FromSeconds(1.0 / (double)this.globalState.FPS);
            this._animationLoop.Tick += AnimStep;
        }

        private void AnimStep(object? sender, EventArgs e)
        {
            if (this.currentAnimation != null)
            {
                this.currentAnimation.Step();
                this.globalState.OnTimeUpdated();
            }
        }

        /// <summary>
        /// Starts or stops animation and set it in MainEngine
        /// </summary>
        /// <param name="animation">Runned animation</param>
        public void runAnimation(Animation? animation)
        {
            if (animation == null)
            {
                return;
            }

            if (animation.IsRun)
            {
                this._animationLoop.Stop();
                animation.IsRun = false;
            }
            else
            {
                this._animationLoop.Start();
                animation.IsRun = true;
            }

            this.currentAnimation = animation;
        }

        private void UpdateCanvas(object? sender, EventArgs e)
        {
            if (this.globalState.currentTab == 0)
            {
                UpdateCanvas(
                    this.globalState.canvas,
                    this.globalState.drawBones,
                    this.globalState.captureMode
                );
            }
        }

        public void UpdateCanvas(Canvas canvas, bool isDrawBone, bool isDrawCapture)
        {
            canvas?.Children.Clear();
            this.globalState.CurrentProject?.DrawSlots(canvas);
            if (isDrawBone)
            {
                this.globalState.CurrentProject?.MainSkeleton?.DrawSkeleton(canvas);
            }
            if (isDrawCapture)
            {
                this.globalState.captureArea?.DrawCaptureArea(canvas);
            }
        }
    }
}
