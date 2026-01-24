using System;
using System.Collections.Generic;
using System.Linq;
using AnimTransformations;

namespace AnimModels
{
    /// <summary>
    /// Class BoneAnimation contains the bone's transformations during animation
    /// </summary>
    public class BoneAnimation
    {
        public SortedDictionary<double, IKeyframeType> rotateKeyframes =
            new SortedDictionary<double, IKeyframeType>();
        public SortedDictionary<double, IKeyframeType> translateKeyframes =
            new SortedDictionary<double, IKeyframeType>();
        public SortedDictionary<double, IKeyframeType> shearKeyframes =
            new SortedDictionary<double, IKeyframeType>();
        public SortedDictionary<double, IKeyframeType> scaleKeyframes =
            new SortedDictionary<double, IKeyframeType>();

        private double rotateStart,
            rotateEnd;
        private double translateStart,
            translateEnd;
        private double scaleStart,
            scaleEnd;
        private double shearStart,
            shearEnd;

        /// <summary>
        /// Add translate keyframe to bone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void addTranslateFrame(double time, double x, double y)
        {
            if (translateKeyframes.ContainsKey(time))
            {
                translateKeyframes[time] = new Translate(time, x, y);
            }
            else
            {
                translateKeyframes.Add(time, new Translate(time, x, y));
            }
        }

        /// <summary>
        /// Add rotate keyframe to bone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value"></param>
        public void addRotateFrame(double time, double value)
        {
            if (rotateKeyframes.ContainsKey(time))
            {
                rotateKeyframes[time] = new Rotate(time, value);
            }
            else
            {
                rotateKeyframes.Add(time, new Rotate(time, value));
            }
        }

        /// <summary>
        /// Add scale keyframe to bone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void addScaleFrame(double time, double x, double y)
        {
            if (scaleKeyframes.ContainsKey(time))
            {
                scaleKeyframes[time] = new Scale(time, x, y);
            }
            else
            {
                scaleKeyframes.Add(time, new Scale(time, x, y));
            }
        }

        /// <summary>
        /// Add shear keyframe to bone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void addShearFrame(double time, double x, double y)
        {
            if (shearKeyframes.ContainsKey(time))
            {
                shearKeyframes[time] = new Shear(time, x, y);
            }
            else
            {
                shearKeyframes.Add(time, new Shear(time, x, y));
            }
        }

        /// <summary>
        /// Delete keyframe
        /// </summary>
        /// <param name="time"></param>
        /// <param name="keyFrameType"></param>
        public void deleteKeyFrame(double time, KeyFrameTypes keyFrameType)
        {
            if (keyFrameType == KeyFrameTypes.TRANSLATE)
            {
                if (translateKeyframes.ContainsKey(time))
                {
                    translateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == KeyFrameTypes.ROTATE)
            {
                if (rotateKeyframes.ContainsKey(time))
                {
                    rotateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == KeyFrameTypes.SHEAR)
            {
                if (shearKeyframes.ContainsKey(time))
                {
                    shearKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == KeyFrameTypes.SCALE)
            {
                if (scaleKeyframes.ContainsKey(time))
                {
                    scaleKeyframes.Remove(time);
                }
            }
        }

        /// <summary>
        /// Finds current time segment
        /// </summary>
        /// <param name="currTime"></param>
        /// <param name="keyFrameType"></param>
        private void FindSegment(double currTime, KeyFrameTypes keyFrameType)
        {
            if (keyFrameType == KeyFrameTypes.TRANSLATE)
            {
                for (int i = 0; i < translateKeyframes.Keys.Count - 1; i++)
                {
                    translateStart = translateKeyframes.Keys.ElementAt(i);
                    translateEnd = translateKeyframes.Keys.ElementAt(i + 1);
                    if (currTime < translateEnd && currTime > translateStart)
                    {
                        return;
                    }
                }
            }
            else if (keyFrameType == KeyFrameTypes.ROTATE)
            {
                for (int i = 0; i < rotateKeyframes.Keys.Count - 1; i++)
                {
                    rotateStart = rotateKeyframes.Keys.ElementAt(i);
                    rotateEnd = rotateKeyframes.Keys.ElementAt(i + 1);
                    if (currTime < rotateEnd && currTime > rotateStart)
                    {
                        return;
                    }
                }
            }
            else if (keyFrameType == KeyFrameTypes.SCALE)
            {
                for (int i = 0; i < scaleKeyframes.Keys.Count - 1; i++)
                {
                    scaleStart = scaleKeyframes.Keys.ElementAt(i);
                    scaleEnd = scaleKeyframes.Keys.ElementAt(i + 1);
                    if (currTime < scaleEnd && currTime > scaleStart)
                    {
                        return;
                    }
                }
            }
            else if (keyFrameType == KeyFrameTypes.SHEAR)
            {
                for (int i = 0; i < shearKeyframes.Keys.Count - 1; i++)
                {
                    shearStart = shearKeyframes.Keys.ElementAt(i);
                    shearEnd = shearKeyframes.Keys.ElementAt(i + 1);
                    if (currTime < shearEnd && currTime > shearStart)
                    {
                        return;
                    }
                }
            }
        }

        private void TranslateStep(Bone b, double time)
        {
            if (translateKeyframes.Count < 2)
            {
                return;
            }

            if (time > translateEnd)
            {
                FindSegment(time, KeyFrameTypes.TRANSLATE);
            }

            double t = Interpolations.Interpolation.findInterpolateParam(
                translateEnd - translateStart,
                time - translateStart
            );

            double interpolatedX = Interpolations.Interpolation.linearInterpolation(
                ((Translate)translateKeyframes[translateStart]).x,
                ((Translate)translateKeyframes[translateEnd]).x,
                t
            );
            double interpolatedY = Interpolations.Interpolation.linearInterpolation(
                ((Translate)translateKeyframes[translateStart]).y,
                ((Translate)translateKeyframes[translateEnd]).y,
                t
            );

            b.move(interpolatedX, interpolatedY);
        }

        private void RotateStep(Bone b, double time)
        {
            if (rotateKeyframes.Count < 2)
            {
                return;
            }

            if (time > rotateEnd)
            {
                FindSegment(time, KeyFrameTypes.ROTATE);
            }

            double t = Interpolations.Interpolation.findInterpolateParam(
                rotateEnd - rotateStart,
                time - rotateStart
            );

            double interpolatedA = Interpolations.Interpolation.angleInterpolation(
                ((Rotate)rotateKeyframes[rotateStart]).value,
                ((Rotate)rotateKeyframes[rotateEnd]).value,
                t
            );

            b.rotate(interpolatedA);
        }

        public void BoneStep(Bone b, double time)
        {
            TranslateStep(b, time);
            RotateStep(b, time);
        }
    }
}
