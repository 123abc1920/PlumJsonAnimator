using System;
using System.Collections.Generic;
using System.Linq;
using AnimModels;
using AnimTransformations;
using Constants;

namespace AnimEngine
{
    public class BoneInAnimation
    {
        public Bone? bone;
        public List<IKeyframeType> rotateKeyframes = new List<IKeyframeType>();
        public List<IKeyframeType> translateKeyframes = new List<IKeyframeType>();
        public List<IKeyframeType> shearKeyframes = new List<IKeyframeType>();
        public List<IKeyframeType> scaleKeyframes = new List<IKeyframeType>();
        public Translate currentTranslateFrame = null,
            nextTranslateFrame = null;
        public Rotate currentRotateFrame = null,
            nextRotateFrame = null;
        public double tTranslate = 0;
        public double tRotate = 0;

        public BoneInAnimation(Bone _bone)
        {
            this.bone = _bone;
        }

        public void setRotateKeyFrame(double value)
        {
            foreach (Rotate frame in rotateKeyframes)
            {
                if (
                    Math.Abs(frame.time - ConstantsClass.currentProject.GetAnimation().currentTime)
                    < 0.01f
                )
                {
                    frame.value = value;
                    return;
                }
            }

            rotateKeyframes.Add(
                new Rotate(ConstantsClass.currentProject.GetAnimation().currentTime, value)
            );
            rotateKeyframes = rotateKeyframes.OrderBy(k => k.time).ToList();
        }

        public void setTranslateKeyFrame(double x, double y)
        {
            foreach (Translate frame in translateKeyframes)
            {
                if (
                    Math.Abs(frame.time - ConstantsClass.currentProject.GetAnimation().currentTime)
                    < 0.01f
                )
                {
                    frame.x = x;
                    frame.y = y;
                    return;
                }
            }

            translateKeyframes.Add(
                new Translate(ConstantsClass.currentProject.GetAnimation().currentTime, x, y)
            );
            translateKeyframes = translateKeyframes.OrderBy(k => k.time).ToList();
        }

        public void setShearKeyFrame(double x, double y)
        {
            foreach (Shear frame in shearKeyframes)
            {
                if (frame.time == ConstantsClass.currentProject.GetAnimation().currentTime)
                {
                    frame.x = x;
                    frame.y = y;
                    return;
                }
            }

            shearKeyframes.Add(
                new Shear(ConstantsClass.currentProject.GetAnimation().currentTime, x, y)
            );
            shearKeyframes = shearKeyframes.OrderBy(k => k.time).ToList();
        }

        public void setScaleKeyFrame(double x, double y)
        {
            foreach (Scale frame in scaleKeyframes)
            {
                if (frame.time == ConstantsClass.currentProject.GetAnimation().currentTime)
                {
                    frame.x = x;
                    frame.y = y;
                    return;
                }
            }

            scaleKeyframes.Add(
                new Scale(ConstantsClass.currentProject.GetAnimation().currentTime, x, y)
            );
            scaleKeyframes = scaleKeyframes.OrderBy(k => k.time).ToList();
        }

        private void translateStep()
        {
            if (translateKeyframes.Count > 1)
            {
                for (int i = 0; i < translateKeyframes.Count - 1; i++)
                {
                    Translate frame = (Translate)translateKeyframes[i];
                    Translate nextFrame = (Translate)translateKeyframes[i + 1];
                    if (
                        Math.Abs(
                            frame.time - ConstantsClass.currentProject.GetAnimation().currentTime
                        ) < 0.01f
                    )
                    {
                        currentTranslateFrame = frame;
                        nextTranslateFrame = nextFrame;
                        tTranslate = 0;
                        break;
                    }
                }
            }
            else
            {
                if (currentTranslateFrame != null)
                {
                    this.bone?.move(
                        ((Translate)translateKeyframes[0]).x,
                        ((Translate)translateKeyframes[0]).y
                    );
                }
            }

            if (currentTranslateFrame != null && nextTranslateFrame != null && tTranslate <= 1.0)
            {
                tTranslate += 0.0167;

                double interpolatedX = Interpolations.Interpolation.linearInterpolation(
                    currentTranslateFrame.x,
                    nextTranslateFrame.x,
                    tTranslate
                );
                double interpolatedY = Interpolations.Interpolation.linearInterpolation(
                    currentTranslateFrame.y,
                    nextTranslateFrame.y,
                    tTranslate
                );
                this.bone?.move(interpolatedX, interpolatedY);
            }
        }

        private void rotateStep()
        {
            if (rotateKeyframes.Count > 1)
            {
                for (int i = 0; i < rotateKeyframes.Count - 1; i++)
                {
                    Rotate frame = (Rotate)rotateKeyframes[i];
                    Rotate nextFrame = (Rotate)rotateKeyframes[i + 1];
                    if (
                        Math.Abs(
                            frame.time - ConstantsClass.currentProject.GetAnimation().currentTime
                        ) < 0.01f
                    )
                    {
                        currentRotateFrame = frame;
                        nextRotateFrame = nextFrame;
                        tRotate = 0;
                        break;
                    }
                }
            }
            else
            {
                if (currentRotateFrame != null)
                {
                    this.bone?.rotate(currentRotateFrame.value);
                }
            }

            if (currentRotateFrame != null && nextRotateFrame != null && tRotate <= 1.0)
            {
                tRotate += 0.0167;

                double interpolatedA = Interpolations.Interpolation.angleInterpolation(
                    currentRotateFrame.value,
                    nextRotateFrame.value,
                    tRotate
                );
                this.bone?.rotate(interpolatedA);
            }
        }

        public void animationStep()
        {
            translateStep();
            rotateStep();
        }
    }
}
