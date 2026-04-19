using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

// TODO: Remove repetitions
namespace PlumJsonAnimator.Models
{
    /// <summary>
    /// Class BoneAnimation contains the bone's transformations during animation
    /// </summary>
    public class BoneAnimation
    {
        private SortedDictionary<double, IKeyframeType> _rotateKeyframes =
            new SortedDictionary<double, IKeyframeType>();
        private SortedDictionary<double, IKeyframeType> _translateKeyframes =
            new SortedDictionary<double, IKeyframeType>();
        private SortedDictionary<double, IKeyframeType> _shearKeyframes =
            new SortedDictionary<double, IKeyframeType>();
        private SortedDictionary<double, IKeyframeType> _scaleKeyframes =
            new SortedDictionary<double, IKeyframeType>();

        private double _rotateStart,
            _rotateEnd;
        private double _translateStart,
            _translateEnd;
        private double _scaleStart,
            _scaleEnd;
        private double _shearStart,
            _shearEnd;

        private GlobalState _globalState;
        private Interpolation _interpolation;

        public BoneAnimation(GlobalState globalState, Interpolation interpolation)
        {
            this._globalState = globalState;
            this._interpolation = interpolation;
        }

        /// <summary>
        /// Adds translate keyframe to bone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddTranslateFrame(double time, double x, double y)
        {
            if (_translateKeyframes.ContainsKey(time))
            {
                _translateKeyframes[time] = new Translate(this._globalState, time, x, y);
            }
            else
            {
                _translateKeyframes.Add(time, new Translate(this._globalState, time, x, y));
            }
        }

        /// <summary>
        /// Adds rotate keyframe to bone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value"></param>
        public void AddRotateFrame(double time, double value)
        {
            if (_rotateKeyframes.ContainsKey(time))
            {
                _rotateKeyframes[time] = new Rotate(this._globalState, time, value);
            }
            else
            {
                _rotateKeyframes.Add(time, new Rotate(this._globalState, time, value));
            }
        }

        /// <summary>
        /// Delete keyframe
        /// </summary>
        /// <param name="time"></param>
        /// <param name="keyFrameType"></param>
        public void DeleteKeyFrame(double time, TransformModesTypes keyFrameType)
        {
            if (keyFrameType == TransformModesTypes.TRANSLATE)
            {
                if (_translateKeyframes.ContainsKey(time))
                {
                    _translateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModesTypes.ROTATE)
            {
                if (_rotateKeyframes.ContainsKey(time))
                {
                    _rotateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModesTypes.SHEAR)
            {
                if (_shearKeyframes.ContainsKey(time))
                {
                    _shearKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModesTypes.SCALE)
            {
                if (_scaleKeyframes.ContainsKey(time))
                {
                    _scaleKeyframes.Remove(time);
                }
            }
        }

        public Dictionary<double, Dictionary<KeyFrameTypes, bool>> GetKeyFeamesMarks()
        {
            Dictionary<double, Dictionary<KeyFrameTypes, bool>> result =
                new Dictionary<double, Dictionary<KeyFrameTypes, bool>>();

            foreach (double time in _rotateKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<KeyFrameTypes, bool>());
                }
                result[time].Add(KeyFrameTypes.ROTATE, true);
            }

            foreach (double time in _translateKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<KeyFrameTypes, bool>());
                }
                result[time].Add(KeyFrameTypes.TRANSLATE, true);
            }

            foreach (double time in _scaleKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<KeyFrameTypes, bool>());
                }
                result[time].Add(KeyFrameTypes.SCALE, true);
            }

            foreach (double time in _shearKeyframes.Keys)
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
        /// Finds current segment using time
        /// </summary>
        /// <param name="currTime"></param>
        /// <param name="keyFrameType"></param>
        private void FindSegment(double currTime, KeyFrameTypes keyFrameType)
        {
            if (keyFrameType == KeyFrameTypes.TRANSLATE)
            {
                for (int i = 0; i < _translateKeyframes.Keys.Count - 1; i++)
                {
                    _translateStart = _translateKeyframes.Keys.ElementAt(i);
                    _translateEnd = _translateKeyframes.Keys.ElementAt(i + 1);
                    if (currTime <= _translateEnd && currTime >= _translateStart)
                    {
                        return;
                    }
                }
            }
            else if (keyFrameType == KeyFrameTypes.ROTATE)
            {
                for (int i = 0; i < _rotateKeyframes.Keys.Count - 1; i++)
                {
                    _rotateStart = _rotateKeyframes.Keys.ElementAt(i);
                    _rotateEnd = _rotateKeyframes.Keys.ElementAt(i + 1);
                    if (currTime <= _rotateEnd && currTime >= _rotateStart)
                    {
                        return;
                    }
                }
            }
            else if (keyFrameType == KeyFrameTypes.SCALE)
            {
                for (int i = 0; i < _scaleKeyframes.Keys.Count - 1; i++)
                {
                    _scaleStart = _scaleKeyframes.Keys.ElementAt(i);
                    _scaleEnd = _scaleKeyframes.Keys.ElementAt(i + 1);
                    if (currTime <= _scaleEnd && currTime >= _scaleStart)
                    {
                        return;
                    }
                }
            }
            else if (keyFrameType == KeyFrameTypes.SHEAR)
            {
                for (int i = 0; i < _shearKeyframes.Keys.Count - 1; i++)
                {
                    _shearStart = _shearKeyframes.Keys.ElementAt(i);
                    _shearEnd = _shearKeyframes.Keys.ElementAt(i + 1);
                    if (currTime <= _shearEnd && currTime >= _shearStart)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Translates bone according current time
        /// </summary>
        /// <param name="b">Bone</param>
        /// <param name="time">Current time</param>
        private void TranslateStep(Bone b, double time)
        {
            if (_translateKeyframes.Count == 0)
            {
                return;
            }

            if (_translateKeyframes.Count == 1)
            {
                var onlyKeyframe = _translateKeyframes.First().Value;
                b.Move(((Translate)onlyKeyframe).x, ((Translate)onlyKeyframe).y);
                return;
            }

            FindSegment(time, KeyFrameTypes.TRANSLATE);

            double t = this._interpolation.findInterpolateParam(
                _translateEnd - _translateStart,
                time - _translateStart
            );

            double interpolatedX = this._interpolation.linearInterpolation(
                ((Translate)_translateKeyframes[_translateStart]).x,
                ((Translate)_translateKeyframes[_translateEnd]).x,
                t
            );
            double interpolatedY = this._interpolation.linearInterpolation(
                ((Translate)_translateKeyframes[_translateStart]).y,
                ((Translate)_translateKeyframes[_translateEnd]).y,
                t
            );

            b.Move(interpolatedX, interpolatedY);
        }

        /// <summary>
        /// Rotates bone according current time
        /// </summary>
        /// <param name="b">Bone</param>
        /// <param name="time">Current time</param>
        private void RotateStep(Bone b, double time)
        {
            if (_rotateKeyframes.Count == 0)
            {
                return;
            }

            if (_rotateKeyframes.Count == 1)
            {
                var onlyKeyframe = _rotateKeyframes.First().Value;
                b.Rotate(((Rotate)onlyKeyframe).value);
                return;
            }

            FindSegment(time, KeyFrameTypes.ROTATE);

            double t = this._interpolation.findInterpolateParam(
                _rotateEnd - _rotateStart,
                time - _rotateStart
            );

            double interpolatedA = this._interpolation.angleInterpolation(
                ((Rotate)_rotateKeyframes[_rotateStart]).value,
                ((Rotate)_rotateKeyframes[_rotateEnd]).value,
                t
            );

            b.Rotate(interpolatedA);
        }

        /// <summary>
        /// Sets the bone to the desired state according current time
        /// </summary>
        /// <param name="b">Bone</param>
        /// <param name="time">Current time</param>
        public void BoneStep(Bone b, double time)
        {
            TranslateStep(b, time);
            RotateStep(b, time);
        }

        public BoneAnimationData GenerateJSONData()
        {
            List<IKeyframeTypeData> translatesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> rotatesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> scalesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> shearsJSON = new List<IKeyframeTypeData>();

            foreach (IKeyframeType frame in _translateKeyframes.Values)
            {
                translatesJSON.Add(frame.GenerateJSONData());
            }
            foreach (IKeyframeType frame in _rotateKeyframes.Values)
            {
                rotatesJSON.Add(frame.GenerateJSONData());
            }
            foreach (IKeyframeType frame in _scaleKeyframes.Values)
            {
                scalesJSON.Add(frame.GenerateJSONData());
            }
            foreach (IKeyframeType frame in _shearKeyframes.Values)
            {
                shearsJSON.Add(frame.GenerateJSONData());
            }

            return new BoneAnimationData
            {
                translate = translatesJSON,
                rotate = rotatesJSON,
                scale = scalesJSON,
                shear = shearsJSON,
            };
        }

        public string GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }

        /// <summary>
        /// Finds time of the next or the previous keyframe
        /// </summary>
        /// <param name="time">Current time</param>
        /// <param name="type">Type of target keyframe</param>
        /// <param name="isNext">Search for the next or the previous keyframe</param>
        /// <returns>Double, time of the target keyframe</returns>
        public double FindTime(double time, TransformModesTypes type, bool isNext)
        {
            SortedDictionary<double, IKeyframeType>? keyframes = null;
            if (type == TransformModesTypes.TRANSLATE)
            {
                keyframes = _translateKeyframes;
            }
            if (type == TransformModesTypes.ROTATE)
            {
                keyframes = _rotateKeyframes;
            }
            if (type == TransformModesTypes.SCALE)
            {
                keyframes = _scaleKeyframes;
            }
            if (type == TransformModesTypes.SHEAR)
            {
                keyframes = _shearKeyframes;
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

        /// <summary>
        /// Finds the last keyframe time
        /// </summary>
        /// <param name="dict">Dictionary in which to find the last keyframe time</param>
        /// <returns>Double, time of the last keyframe</returns>
        private double FindMax(SortedDictionary<double, IKeyframeType> dict)
        {
            if (dict.Count > 0)
            {
                return dict.Keys.Last();
            }

            return 0.0;
        }

        /// <summary>
        /// Finds the longest time from all keyframe dictionary
        /// </summary>
        public double MaxTime()
        {
            double maxRotate = FindMax(_rotateKeyframes);
            double maxTranslate = FindMax(_translateKeyframes);
            double maxScale = FindMax(_scaleKeyframes);
            double maxShear = FindMax(_shearKeyframes);

            return Math.Max(Math.Max(maxRotate, maxTranslate), Math.Max(maxScale, maxShear));
        }
    }

    /// <summary>
    /// BoneAnimation JSON data
    /// </summary>
    public class BoneAnimationData
    {
        [JsonProperty("translate")]
        public required List<IKeyframeTypeData> translate { get; set; }

        [JsonProperty("rotate", NullValueHandling = NullValueHandling.Ignore)]
        public List<IKeyframeTypeData>? rotate { get; set; }

        [JsonProperty("scale", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public required List<IKeyframeTypeData> scale { get; set; }

        [JsonProperty("shear", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public required List<IKeyframeTypeData> shear { get; set; }
    }
}
