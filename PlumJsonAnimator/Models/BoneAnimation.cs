using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Easing;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

// TODO: Remove repetitions
// TODO: new project not working
namespace PlumJsonAnimator.Models
{
    /// <summary>
    /// Class BoneAnimation contains the bone's transformations during animation
    /// </summary>
    public class BoneAnimation
    {
        private NoNullSortedDictionary<double, IKeyframeType> _rotateKeyframes =
            new NoNullSortedDictionary<double, IKeyframeType>();
        private NoNullSortedDictionary<double, IKeyframeType> _translateKeyframes =
            new NoNullSortedDictionary<double, IKeyframeType>();
        private NoNullSortedDictionary<double, IKeyframeType> _shearKeyframes =
            new NoNullSortedDictionary<double, IKeyframeType>();
        private NoNullSortedDictionary<double, IKeyframeType> _scaleKeyframes =
            new NoNullSortedDictionary<double, IKeyframeType>();

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
        public void AddTranslateFrame(double time, double x, double y, IEasing easing)
        {
            if (_translateKeyframes.ContainsKey(time))
            {
                _translateKeyframes[time] = new TranslateKeyFrame(
                    this._globalState,
                    time,
                    x,
                    y,
                    easing
                );
            }
            else
            {
                _translateKeyframes.Add(
                    time,
                    new TranslateKeyFrame(this._globalState, time, x, y, easing)
                );
            }
        }

        /// <summary>
        /// Adds rotate keyframe to bone
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value"></param>
        /// <param name="easing"></param>
        public void AddRotateFrame(double time, double value, IEasing easing)
        {
            if (_rotateKeyframes.ContainsKey(time))
            {
                _rotateKeyframes[time] = new RotateKeyFrame(
                    this._globalState,
                    time,
                    value,
                    easing
                );
            }
            else
            {
                _rotateKeyframes.Add(
                    time,
                    new RotateKeyFrame(this._globalState, time, value, easing)
                );
            }
        }

        public void AddShearFrame(double time, double shearX, double shearY, IEasing easing)
        {
            if (_shearKeyframes.ContainsKey(time))
            {
                _shearKeyframes[time] = new ShearKeyFrame(
                    _globalState,
                    time,
                    shearX,
                    shearY,
                    easing
                );
            }
            else
            {
                _shearKeyframes.Add(
                    time,
                    new ShearKeyFrame(_globalState, time, shearX, shearY, easing)
                );
            }
        }

        public void AddScaleFrame(double time, double scaleX, double scaleY, IEasing easing)
        {
            if (_scaleKeyframes.ContainsKey(time))
            {
                _scaleKeyframes[time] = new ScaleKeyFrame(
                    _globalState,
                    time,
                    scaleX,
                    scaleY,
                    easing
                );
            }
            else
            {
                _scaleKeyframes.Add(
                    time,
                    new ScaleKeyFrame(_globalState, time, scaleX, scaleY, easing)
                );
            }
        }

        /// <summary>
        /// Delete keyframe
        /// </summary>
        /// <param name="time"></param>
        /// <param name="keyFrameType"></param>
        public void DeleteKeyFrame(double time, TransformModeTypes keyFrameType)
        {
            if (keyFrameType == TransformModeTypes.TRANSLATE)
            {
                if (_translateKeyframes.ContainsKey(time))
                {
                    _translateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModeTypes.ROTATE)
            {
                if (_rotateKeyframes.ContainsKey(time))
                {
                    _rotateKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModeTypes.SHEAR)
            {
                if (_shearKeyframes.ContainsKey(time))
                {
                    _shearKeyframes.Remove(time);
                }
            }
            else if (keyFrameType == TransformModeTypes.SCALE)
            {
                if (_scaleKeyframes.ContainsKey(time))
                {
                    _scaleKeyframes.Remove(time);
                }
            }
        }

        public Dictionary<double, Dictionary<TransformModeTypes, bool>> GetKeyFeamesMarks()
        {
            Dictionary<double, Dictionary<TransformModeTypes, bool>> result =
                new Dictionary<double, Dictionary<TransformModeTypes, bool>>();

            foreach (double time in _rotateKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<TransformModeTypes, bool>());
                }
                result[time].Add(TransformModeTypes.ROTATE, true);
            }

            foreach (double time in _translateKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<TransformModeTypes, bool>());
                }
                result[time].Add(TransformModeTypes.TRANSLATE, true);
            }

            foreach (double time in _scaleKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<TransformModeTypes, bool>());
                }
                result[time].Add(TransformModeTypes.SCALE, true);
            }

            foreach (double time in _shearKeyframes.Keys)
            {
                if (!result.ContainsKey(time))
                {
                    result.Add(time, new Dictionary<TransformModeTypes, bool>());
                }
                result[time].Add(TransformModeTypes.SHEAR, true);
            }

            return result;
        }

        public IKeyframeType? GetKeyFrame(TransformModeTypes type, double time)
        {
            if (type == TransformModeTypes.TRANSLATE)
            {
                if (!_translateKeyframes.ContainsKey(time))
                {
                    return null;
                }
                return _translateKeyframes[time];
            }
            else if (type == TransformModeTypes.ROTATE)
            {
                if (!_rotateKeyframes.ContainsKey(time))
                {
                    return null;
                }
                return _rotateKeyframes[time];
            }
            else if (type == TransformModeTypes.SHEAR)
            {
                if (!_shearKeyframes.ContainsKey(time))
                {
                    return null;
                }
                return _shearKeyframes[time];
            }
            else if (type == TransformModeTypes.SCALE)
            {
                return null;
            }

            return null;
        }

        public void SetKeyFrame(TransformModeTypes type, double time, IKeyframeType keyframe)
        {
            if (type == TransformModeTypes.TRANSLATE)
            {
                _translateKeyframes[time] = keyframe;
            }
            else if (type == TransformModeTypes.ROTATE)
            {
                _rotateKeyframes[time] = keyframe;
            }
            else if (type == TransformModeTypes.SHEAR)
            {
                _shearKeyframes[time] = keyframe;
            }
            else if (type == TransformModeTypes.SCALE)
            {
                _scaleKeyframes[time] = keyframe;
            }
        }

        public void RestoreKeyFrame(
            IKeyframeType keyframeType,
            double time,
            TransformModeTypes type
        )
        {
            if (type == TransformModeTypes.TRANSLATE)
            {
                _translateKeyframes[time] = keyframeType;
            }
            else if (type == TransformModeTypes.ROTATE)
            {
                _rotateKeyframes[time] = keyframeType;
            }
            else if (type == TransformModeTypes.SHEAR)
            {
                _shearKeyframes[time] = keyframeType;
            }
            else if (type == TransformModeTypes.SCALE)
            {
                _scaleKeyframes[time] = keyframeType;
            }
        }

        /// <summary>
        /// Finds current segment using time
        /// </summary>
        /// <param name="currTime"></param>
        /// <param name="keyFrameType"></param>
        private void FindSegment(double currTime, TransformModeTypes keyFrameType)
        {
            void FindInKeys(ICollection<double> keys, ref double startRes, ref double endRes)
            {
                if (keys.Count < 2)
                    return;

                double[] keyArray = new double[keys.Count];
                keys.CopyTo(keyArray, 0);

                if (currTime <= keyArray[0])
                {
                    startRes = keyArray[0];
                    endRes = keyArray[1];
                    return;
                }

                if (currTime >= keyArray[keyArray.Length - 1])
                {
                    startRes = keyArray[keyArray.Length - 2];
                    endRes = keyArray[keyArray.Length - 1];
                    return;
                }

                for (int i = 0; i < keyArray.Length - 1; i++)
                {
                    startRes = keyArray[i];
                    endRes = keyArray[i + 1];
                    if (currTime <= endRes && currTime >= startRes)
                    {
                        return;
                    }
                }
            }

            switch (keyFrameType)
            {
                case TransformModeTypes.TRANSLATE:
                    FindInKeys(_translateKeyframes.Keys, ref _translateStart, ref _translateEnd);
                    break;
                case TransformModeTypes.ROTATE:
                    FindInKeys(_rotateKeyframes.Keys, ref _rotateStart, ref _rotateEnd);
                    break;
                case TransformModeTypes.SCALE:
                    FindInKeys(_scaleKeyframes.Keys, ref _scaleStart, ref _scaleEnd);
                    break;
                case TransformModeTypes.SHEAR:
                    FindInKeys(_shearKeyframes.Keys, ref _shearStart, ref _shearEnd);
                    break;
            }
        }

        public IKeyframeType? FindKeyFrame(double time, TransformModeTypes transformMode)
        {
            IKeyframeType? result = null;
            double bestTime = double.NegativeInfinity;

            void Scan(NoNullSortedDictionary<double, IKeyframeType> dict)
            {
                var keys = new SortedSet<double>(dict.Keys);
                if (keys.Count == 0)
                    return;

                var view = keys.GetViewBetween(keys.Min, time);
                if (view.Count == 0)
                    return;

                var max = view.Max;
                if (max > bestTime)
                {
                    bestTime = max;
                    result = dict.Get(max);
                }
            }

            if (transformMode == TransformModeTypes.ROTATE)
            {
                Scan(_rotateKeyframes);
            }
            else if (transformMode == TransformModeTypes.TRANSLATE)
            {
                Scan(_translateKeyframes);
            }
            else if (transformMode == TransformModeTypes.SCALE)
            {
                Scan(_scaleKeyframes);
            }
            else if (transformMode == TransformModeTypes.SHEAR)
            {
                Scan(_shearKeyframes);
            }

            return result;
        }

        public SortedDictionary<double, IKeyframeType>? GetKeyFrameLine(
            TransformModeTypes keyFrameType
        )
        {
            var source = keyFrameType switch
            {
                TransformModeTypes.TRANSLATE => _translateKeyframes,
                TransformModeTypes.ROTATE => _rotateKeyframes,
                TransformModeTypes.SCALE => _scaleKeyframes,
                TransformModeTypes.SHEAR => _shearKeyframes,
                _ => null,
            };

            if (source is null)
                return null;

            return new SortedDictionary<double, IKeyframeType>(source);
        }

        /// <summary>
        /// Translates bone according current time
        /// </summary>
        /// <param name="b">Bone</param>
        /// <param name="time">Current time</param>
        private void TranslateStep(Bone b, double time)
        {
            if (_translateKeyframes.Count == 0)
                return;

            double localX,
                localY;

            if (_translateKeyframes.Count == 1)
            {
                var onlyKeyframe = (TranslateKeyFrame)_translateKeyframes.First().Value;
                localX = (double)onlyKeyframe.X;
                localY = (double)onlyKeyframe.Y;
            }
            else
            {
                if (_translateKeyframes.ContainsKey(_translateStart))
                {
                    var currentSegment = _translateKeyframes.Get(_translateStart);
                    FindSegment(time, TransformModeTypes.TRANSLATE);
                    double t = _interpolation.FindInterpolateParam(
                        _translateEnd - _translateStart,
                        time - _translateStart,
                        currentSegment.Curve
                    );
                    if (
                        _translateKeyframes.ContainsKey(_translateEnd)
                        && _translateKeyframes.ContainsKey(_translateStart)
                    )
                    {
                        localX = _interpolation.BaseInterpolation(
                            ((TranslateKeyFrame)_translateKeyframes[_translateStart]).X,
                            ((TranslateKeyFrame)_translateKeyframes[_translateEnd]).X,
                            t
                        );
                        localY = _interpolation.BaseInterpolation(
                            ((TranslateKeyFrame)_translateKeyframes[_translateStart]).Y,
                            ((TranslateKeyFrame)_translateKeyframes[_translateEnd]).Y,
                            t
                        );
                    }
                    else
                        return;

                    b.X = localX;
                    b.Y = localY;
                }
            }
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
                b.Rotate(((RotateKeyFrame)onlyKeyframe).Value);
                return;
            }

            if (_rotateKeyframes.ContainsKey(_rotateStart))
            {
                var currentSegment = _rotateKeyframes.Get(_rotateStart);

                FindSegment(time, TransformModeTypes.ROTATE);

                double t = _interpolation.FindInterpolateParam(
                    _rotateEnd - _rotateStart,
                    time - _rotateStart,
                    currentSegment.Curve
                );

                double interpolatedA = b.BaseA;
                if (
                    _rotateKeyframes.ContainsKey(_rotateEnd) == true
                    && _rotateKeyframes.ContainsKey(_rotateStart) == true
                )
                {
                    interpolatedA = _interpolation.AngleInterpolation(
                        ((RotateKeyFrame)_rotateKeyframes[_rotateStart]).Value,
                        ((RotateKeyFrame)_rotateKeyframes[_rotateEnd]).Value,
                        t
                    );
                }

                b.Rotate(interpolatedA);
            }
        }

        /// <summary>
        /// Shears bone according current time
        /// </summary>
        /// <param name="b">Bone</param>
        /// <param name="time">Current time</param>
        private void ShearStep(Bone b, double time)
        {
            if (_shearKeyframes.Count == 0)
                return;

            double localX,
                localY;

            if (_shearKeyframes.Count == 1)
            {
                var onlyKeyframe = (ShearKeyFrame)_shearKeyframes.First().Value;
                localX = (double)onlyKeyframe.X;
                localY = (double)onlyKeyframe.Y;
                b.Shear(localX, localY);
                return;
            }

            {
                if (_shearKeyframes.ContainsKey(_shearStart))
                {
                    var currentSegment = _shearKeyframes.Get(_shearStart);
                    FindSegment(time, TransformModeTypes.SHEAR);
                    double t = this._interpolation.FindInterpolateParam(
                        _shearEnd - _shearStart,
                        time - _shearStart,
                        currentSegment.Curve
                    );

                    if (
                        _shearKeyframes.ContainsKey(_shearEnd)
                        && _shearKeyframes.ContainsKey(_shearStart)
                    )
                    {
                        localX = _interpolation.BaseInterpolation(
                            ((ShearKeyFrame)_shearKeyframes[_shearStart]).X,
                            ((ShearKeyFrame)_shearKeyframes[_shearEnd]).X,
                            t
                        );
                        localY = _interpolation.BaseInterpolation(
                            ((ShearKeyFrame)_shearKeyframes[_shearStart]).Y,
                            ((ShearKeyFrame)_shearKeyframes[_shearEnd]).Y,
                            t
                        );
                    }
                    else
                        return;

                    b.Shear(localX, localY);
                }
            }
        }

        /// <summary>
        /// Scales bone according current time
        /// </summary>
        /// <param name="b">Bone</param>
        /// <param name="time">Current time</param>
        private void ScaleStep(Bone b, double time)
        {
            if (_scaleKeyframes.Count == 0)
                return;

            double localX,
                localY;

            if (_scaleKeyframes.Count == 1)
            {
                var onlyKeyframe = (ScaleKeyFrame)_scaleKeyframes.First().Value;
                localX = (double)onlyKeyframe.X;
                localY = (double)onlyKeyframe.Y;
                b.Scale(localX, localY);
                return;
            }

            {
                if (_scaleKeyframes.ContainsKey(_scaleStart))
                {
                    var currentSegment = _scaleKeyframes.Get(_scaleStart);
                    FindSegment(time, TransformModeTypes.SCALE);
                    double t = _interpolation.FindInterpolateParam(
                        _scaleEnd - _scaleStart,
                        time - _scaleStart,
                        currentSegment.Curve
                    );

                    if (
                        _scaleKeyframes.ContainsKey(_scaleEnd)
                        && _scaleKeyframes.ContainsKey(_scaleStart)
                    )
                    {
                        localX = _interpolation.BaseInterpolation(
                            ((ScaleKeyFrame)_scaleKeyframes[_scaleStart]).X,
                            ((ScaleKeyFrame)_scaleKeyframes[_scaleEnd]).X,
                            t
                        );
                        localY = _interpolation.BaseInterpolation(
                            ((ScaleKeyFrame)_scaleKeyframes[_scaleStart]).Y,
                            ((ScaleKeyFrame)_scaleKeyframes[_scaleEnd]).Y,
                            t
                        );
                    }
                    else
                        return;

                    b.Scale(localX, localY);
                }
            }
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
            ShearStep(b, time);
            ScaleStep(b, time);
        }

        public BoneAnimationData GenerateJSONData()
        {
            List<IKeyframeTypeData> translatesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> rotatesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> scalesJSON = new List<IKeyframeTypeData>();
            List<IKeyframeTypeData> shearsJSON = new List<IKeyframeTypeData>();

            translatesJSON.AddRange(
                _translateKeyframes
                    .Values.Where(frame => frame != null)
                    .Select(frame => frame.GenerateJSONData())
            );

            rotatesJSON.AddRange(
                _rotateKeyframes
                    .Values.Where(frame => frame != null)
                    .Select(frame => frame.GenerateJSONData())
            );

            scalesJSON.AddRange(
                _scaleKeyframes
                    .Values.Where(frame => frame != null)
                    .Select(frame => frame.GenerateJSONData())
            );

            shearsJSON.AddRange(
                _shearKeyframes
                    .Values.Where(frame => frame != null)
                    .Select(frame => frame.GenerateJSONData())
            );

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
        public double FindTime(double time, TransformModeTypes type, bool isNext)
        {
            SortedDictionary<double, IKeyframeType>? keyframes = null;
            if (type == TransformModeTypes.TRANSLATE)
            {
                keyframes = _translateKeyframes;
            }
            if (type == TransformModeTypes.ROTATE)
            {
                keyframes = _rotateKeyframes;
            }
            if (type == TransformModeTypes.SCALE)
            {
                keyframes = _scaleKeyframes;
            }
            if (type == TransformModeTypes.SHEAR)
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

        public double? FindNextTime(double time, TransformModeTypes type)
        {
            if (type == TransformModeTypes.TRANSLATE)
            {
                foreach (var kv in _translateKeyframes)
                {
                    if (kv.Key > time)
                    {
                        return kv.Key;
                    }
                }
            }
            if (type == TransformModeTypes.ROTATE)
            {
                foreach (var kv in _rotateKeyframes)
                {
                    if (kv.Key > time)
                    {
                        return kv.Key;
                    }
                }
            }
            if (type == TransformModeTypes.SCALE)
            {
                foreach (var kv in _scaleKeyframes)
                {
                    if (kv.Key > time)
                    {
                        return kv.Key;
                    }
                }
            }
            if (type == TransformModeTypes.SHEAR)
            {
                foreach (var kv in _shearKeyframes)
                {
                    if (kv.Key > time)
                    {
                        return kv.Key;
                    }
                }
            }

            return null;
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
