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
        private Animation? _currentAnimation;

        private GlobalState _globalState;

        public Engine(GlobalState globalState)
        {
            _globalState = globalState;

            _animationLoop.Interval = TimeSpan.FromSeconds(1.0 / (double)_globalState.FPS);
            _animationLoop.Tick += AnimStep;
        }

        private void AnimStep(object? sender, EventArgs e)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Step();
                _globalState.OnTimeUpdated();
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
                _animationLoop.Stop();
                animation.IsRun = false;
            }
            else
            {
                _animationLoop.Start();
                animation.IsRun = true;
            }

            _currentAnimation = animation;
        }

        private void UpdateCanvas(object? sender, EventArgs e)
        {
            if (_globalState.CurrentTab == 0)
            {
                UpdateCanvas(
                    _globalState.Canvas,
                    _globalState.DrawBones,
                    _globalState.CaptureMode
                );
            }
        }

        public void UpdateCanvas(Canvas canvas, bool isDrawBone, bool isDrawCapture)
        {
            canvas?.Children.Clear();
            _globalState.CurrentProject?.DrawSlots(canvas);
            if (isDrawBone)
            {
                _globalState.CurrentProject?.MainSkeleton?.DrawSkeleton(canvas);
            }
            if (isDrawCapture)
            {
                _globalState.CaptureArea?.DrawCaptureArea(canvas);
            }
        }
    }
}
