using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Media;
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

        public void AddShearFrame(double time, double shearX, double shearY)
        {
            if (_shearKeyframes.ContainsKey(time))
            {
                _shearKeyframes[time] = new Shear(_globalState, time, shearX, shearY);
            }
            else
            {
                _shearKeyframes.Add(time, new Shear(_globalState, time, shearX, shearY));
            }
        }

        public void AddScaleFrame(double time, double scaleX, double scaleY)
        {
            if (_scaleKeyframes.ContainsKey(time))
            {
                _scaleKeyframes[time] = new Scale(_globalState, time, scaleX, scaleY);
            }
            else
            {
                _scaleKeyframes.Add(time, new Scale(_globalState, time, scaleX, scaleY));
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

        public IKeyframeType? GetKeyFrame(TransformModesTypes type, double time)
        {
            if (type == TransformModesTypes.TRANSLATE)
            {
                if (!_translateKeyframes.ContainsKey(time))
                {
                    return null;
                }
                return _translateKeyframes[time];
            }
            else if (type == TransformModesTypes.ROTATE)
            {
                if (!_rotateKeyframes.ContainsKey(time))
                {
                    return null;
                }
                return _rotateKeyframes[time];
            }
            else if (type == TransformModesTypes.SHEAR)
            {
                if (!_shearKeyframes.ContainsKey(time))
                {
                    return null;
                }
                return _shearKeyframes[time];
            }
            else if (type == TransformModesTypes.SCALE)
            {
                return null;
            }

            return null;
        }

        public void SetKeyFrame(TransformModesTypes type, double time, IKeyframeType keyframe)
        {
            if (type == TransformModesTypes.TRANSLATE)
            {
                _translateKeyframes[time] = keyframe;
            }
            else if (type == TransformModesTypes.ROTATE)
            {
                _rotateKeyframes[time] = keyframe;
            }
            else if (type == TransformModesTypes.SHEAR)
            {
                _shearKeyframes[time] = keyframe;
            }
            else if (type == TransformModesTypes.SCALE)
            {
                _scaleKeyframes[time] = keyframe;
            }
        }

        public void RestoreKeyFrame(
            IKeyframeType keyframeType,
            double time,
            TransformModesTypes type
        )
        {
            if (type == TransformModesTypes.TRANSLATE)
            {
                _translateKeyframes[time] = keyframeType;
            }
            else if (type == TransformModesTypes.ROTATE)
            {
                _rotateKeyframes[time] = keyframeType;
            }
            else if (type == TransformModesTypes.SHEAR)
            {
                _shearKeyframes[time] = keyframeType;
            }
            else if (type == TransformModesTypes.SCALE)
            {
                _scaleKeyframes[time] = keyframeType;
            }
        }

        /// <summary>
        /// Finds current segment using time
        /// </summary>
        /// <param name="currTime"></param>
        /// <param name="keyFrameType"></param>
        private void FindSegment(double currTime, KeyFrameTypes keyFrameType)
        {
            // Выносим общую логику поиска в маленькую локальную функцию,
            // чтобы не дублировать код для каждого типа анимации
            void FindInKeys(
                System.Collections.Generic.ICollection<double> keys,
                ref double startRes,
                ref double endRes
            )
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
                case KeyFrameTypes.TRANSLATE:
                    FindInKeys(_translateKeyframes.Keys, ref _translateStart, ref _translateEnd);
                    break;
                case KeyFrameTypes.ROTATE:
                    FindInKeys(_rotateKeyframes.Keys, ref _rotateStart, ref _rotateEnd);
                    break;
                case KeyFrameTypes.SCALE:
                    FindInKeys(_scaleKeyframes.Keys, ref _scaleStart, ref _scaleEnd);
                    break;
                case KeyFrameTypes.SHEAR:
                    FindInKeys(_shearKeyframes.Keys, ref _shearStart, ref _shearEnd);
                    break;
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
                return;

            double localX,
                localY;

            if (_translateKeyframes.Count == 1)
            {
                var onlyKeyframe = (Translate)_translateKeyframes.First().Value;
                localX = (double)onlyKeyframe.x;
                localY = (double)onlyKeyframe.y;
            }
            else
            {
                FindSegment(time, KeyFrameTypes.TRANSLATE);
                double t = this._interpolation.findInterpolateParam(
                    _translateEnd - _translateStart,
                    time - _translateStart
                );

                if (
                    this._translateKeyframes.ContainsKey(_translateEnd)
                    && this._translateKeyframes.ContainsKey(_translateStart)
                )
                {
                    localX = this._interpolation.linearInterpolation(
                        ((Translate)_translateKeyframes[_translateStart]).x,
                        ((Translate)_translateKeyframes[_translateEnd]).x,
                        t
                    );
                    localY = this._interpolation.linearInterpolation(
                        ((Translate)_translateKeyframes[_translateStart]).y,
                        ((Translate)_translateKeyframes[_translateEnd]).y,
                        t
                    );
                }
                else
                    return;
            }

            b.X = localX;
            b.Y = localY;
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

            double interpolatedA = b.BaseA;
            if (
                this._rotateKeyframes.ContainsKey(_rotateEnd) == true
                && this._rotateKeyframes.ContainsKey(_rotateStart) == true
            )
            {
                interpolatedA = this._interpolation.angleInterpolation(
                    ((Rotate)_rotateKeyframes[_rotateStart]).value,
                    ((Rotate)_rotateKeyframes[_rotateEnd]).value,
                    t
                );
            }

            b.Rotate(interpolatedA);
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
                var onlyKeyframe = (Shear)_shearKeyframes.First().Value;
                localX = (double)onlyKeyframe.x;
                localY = (double)onlyKeyframe.y;
            }
            else
            {
                FindSegment(time, KeyFrameTypes.SHEAR);
                double t = this._interpolation.findInterpolateParam(
                    _shearEnd - _shearStart,
                    time - _shearStart
                );

                if (
                    this._shearKeyframes.ContainsKey(_shearEnd)
                    && this._shearKeyframes.ContainsKey(_shearStart)
                )
                {
                    localX = this._interpolation.linearInterpolation(
                        ((Shear)_shearKeyframes[_shearStart]).x,
                        ((Shear)_shearKeyframes[_shearEnd]).x,
                        t
                    );
                    localY = this._interpolation.linearInterpolation(
                        ((Shear)_shearKeyframes[_shearStart]).y,
                        ((Shear)_shearKeyframes[_shearEnd]).y,
                        t
                    );
                }
                else
                    return;
            }

            b.Shear(localX, localY);
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
                var onlyKeyframe = (Scale)_scaleKeyframes.First().Value;
                localX = (double)onlyKeyframe.x;
                localY = (double)onlyKeyframe.y;
            }
            else
            {
                FindSegment(time, KeyFrameTypes.SCALE);
                double t = _interpolation.findInterpolateParam(
                    _scaleEnd - _scaleStart,
                    time - _scaleStart
                );

                if (
                    _scaleKeyframes.ContainsKey(_scaleEnd)
                    && _scaleKeyframes.ContainsKey(_scaleStart)
                )
                {
                    localX = _interpolation.linearInterpolation(
                        ((Scale)_scaleKeyframes[_scaleStart]).x,
                        ((Scale)_scaleKeyframes[_scaleEnd]).x,
                        t
                    );
                    localY = _interpolation.linearInterpolation(
                        ((Scale)_scaleKeyframes[_scaleStart]).y,
                        ((Scale)_scaleKeyframes[_scaleEnd]).y,
                        t
                    );
                }
                else
                    return;
            }

            b.Scale(localX, localY);
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
