using System;
using System.Collections.Generic;
using System.Linq;
using AnimModels;
using Common.AnimMath;
using Common.Constants;
using Common.Constants.CommonModels;
using Newtonsoft.Json;

namespace AnimEngine.Models
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
        public void deleteKeyFrame(double time, TransformModesTypes keyFrameType)
        {
            if (keyFrameType == TransformModesTypes.TRANSLATE)
            {
                if (translateKeyframes.ContainsKey(time))
                {
                    translateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModesTypes.ROTATE)
            {
                if (rotateKeyframes.ContainsKey(time))
                {
                    rotateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModesTypes.SHEAR)
            {
                if (shearKeyframes.ContainsKey(time))
                {
                    shearKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModesTypes.SCALE)
            {
                if (scaleKeyframes.ContainsKey(time))
                {
                    scaleKeyframes.Remove(time);
                }
            }
        }

        public Dictionary<double, Dictionary<KeyFrameTypes, bool>> GetKeyFeamesMarks()
        {
            Dictionary<double, Dictionary<KeyFrameTypes, bool>> result =
                new Dictionary<double, Dictionary<KeyFrameTypes, bool>>();

            foreach (double time in rotateKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<KeyFrameTypes, bool>());
                }
                result[time].Add(KeyFrameTypes.ROTATE, true);
            }

            foreach (double time in translateKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<KeyFrameTypes, bool>());
                }
                result[time].Add(KeyFrameTypes.TRANSLATE, true);
            }

            foreach (double time in scaleKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<KeyFrameTypes, bool>());
                }
                result[time].Add(KeyFrameTypes.SCALE, true);
            }

            foreach (double time in shearKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<KeyFrameTypes, bool>());
                }
                result[time].Add(KeyFrameTypes.SHEAR, true);
            }

            return result;
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
                    if (currTime <= translateEnd && currTime >= translateStart)
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
                    if (currTime <= rotateEnd && currTime >= rotateStart)
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
                    if (currTime <= scaleEnd && currTime >= scaleStart)
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
                    if (currTime <= shearEnd && currTime >= shearStart)
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
            FindSegment(time, KeyFrameTypes.TRANSLATE);

            double t = Interpolation.findInterpolateParam(
                translateEnd - translateStart,
                time - translateStart
            );

            double interpolatedX = Interpolation.linearInterpolation(
                ((Translate)translateKeyframes[translateStart]).x,
                ((Translate)translateKeyframes[translateEnd]).x,
                t
            );
            double interpolatedY = Interpolation.linearInterpolation(
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
            FindSegment(time, KeyFrameTypes.ROTATE);

            double t = Interpolation.findInterpolateParam(
                rotateEnd - rotateStart,
                time - rotateStart
            );

            double interpolatedA = Interpolation.angleInterpolation(
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

        public BoneAnimationData generateJSONData()
        {
            List<IKeyframeTypeData> translatesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> rotatesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> scalesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> shearsJSON = new List<IKeyframeTypeData>();

            foreach (IKeyframeType frame in translateKeyframes.Values)
            {
                translatesJSON.Add(frame.generateJSONData());
            }
            foreach (IKeyframeType frame in rotateKeyframes.Values)
            {
                rotatesJSON.Add(frame.generateJSONData());
            }
            foreach (IKeyframeType frame in scaleKeyframes.Values)
            {
                scalesJSON.Add(frame.generateJSONData());
            }
            foreach (IKeyframeType frame in shearKeyframes.Values)
            {
                shearsJSON.Add(frame.generateJSONData());
            }

            return new BoneAnimationData
            {
                translate = translatesJSON,
                rotate = rotatesJSON,
                scale = scalesJSON,
                shear = shearsJSON,
            };
        }

        public string generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), ConstantsClass.jsonSettings);
        }

        public double FindTime(double time, TransformModesTypes type, bool isNext)
        {
            SortedDictionary<double, IKeyframeType> keyframes = null;
            if (type == TransformModesTypes.TRANSLATE)
            {
                keyframes = translateKeyframes;
            }
            if (type == TransformModesTypes.ROTATE)
            {
                keyframes = rotateKeyframes;
            }
            if (type == TransformModesTypes.SCALE)
            {
                keyframes = scaleKeyframes;
            }
            if (type == TransformModesTypes.SHEAR)
            {
                keyframes = shearKeyframes;
            }

            if (keyframes == null || keyframes.Count == 0)
                return time;

            if (isNext)
            {
                foreach (var key in keyframes.Keys)
                {
                    if (key > time)
                        return key;
                }
            }
            else
            {
                double previousKey = time;
                foreach (var key in keyframes.Keys)
                {
                    if (key >= time)
                        break;
                    previousKey = key;
                }
                return previousKey;
            }

            return time;
        }

        private double findMax(SortedDictionary<double, IKeyframeType> dict)
        {
            if (dict.Count > 0)
            {
                return dict.Keys.Last();
            }

            return 0.0;
        }

        public double MaxTime()
        {
            double maxRotate = findMax(rotateKeyframes);
            double maxTranslate = findMax(translateKeyframes);
            double maxScale = findMax(scaleKeyframes);
            double maxShear = findMax(shearKeyframes);

            return Math.Max(Math.Max(maxRotate, maxTranslate), Math.Max(maxScale, maxShear));
        }
    }
}

public class BoneAnimationData
{
    [JsonProperty("translate")]
    public List<IKeyframeTypeData> translate { get; set; }

    [JsonProperty("rotate", NullValueHandling = NullValueHandling.Ignore)]
    public List<IKeyframeTypeData> rotate { get; set; }

    [JsonProperty("scale", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<IKeyframeTypeData> scale { get; set; }

    [JsonProperty("shear", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<IKeyframeTypeData> shear { get; set; }
}
