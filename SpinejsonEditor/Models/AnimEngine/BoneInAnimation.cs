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

        public BoneInAnimation(Bone _bone)
        {
            this.bone = _bone;
        }

        public void setRotateKeyFrame(double value)
        {
            foreach (Rotate frame in rotateKeyframes)
            {
                if (frame.time == ConstantsClass.currentProject.GetAnimation().currentTime)
                {
                    frame.value = value;
                    return;
                }
            }

            rotateKeyframes.Add(
                new Rotate(ConstantsClass.currentProject.GetAnimation().currentTime, value)
            );
            rotateKeyframes = scaleKeyframes.OrderBy(k => k.time).ToList();
        }

        public void setTranslateKeyFrame(double x, double y)
        {
            foreach (Translate frame in translateKeyframes)
            {
                if (frame.time == ConstantsClass.currentProject.GetAnimation().currentTime)
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

        public void animationStep()
        {
            foreach (Translate frame in translateKeyframes)
            {
                Console.WriteLine(frame.time);
                if (
                    Math.Abs(frame.time - ConstantsClass.currentProject.GetAnimation().currentTime)
                    < 0.01f
                )
                {
                    this.bone?.move(frame.x, frame.y);
                }
            }
        }
    }
}
