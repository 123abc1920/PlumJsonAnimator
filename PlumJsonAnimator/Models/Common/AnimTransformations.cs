using System;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Common
{
    /// <summary>
    /// Key frames types. Not transform modes, transform modes provides transformation.
    /// </summary>
    public enum KeyFrameTypes
    {
        TRANSLATE = 0,
        ROTATE,
        SCALE,
        SHEAR,
    }

    public abstract class IKeyframeType(GlobalState globalState, IEasing curve, double time)
    {
        protected double _time = time;
        public IEasing Curve { get; set; } = curve;

        public abstract IKeyframeTypeData GenerateJSONData();

        protected GlobalState _globalState = globalState;

        public String GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }
    }

    public class IKeyframeTypeData()
    {
        [JsonProperty("time")]
        public Double? Time { get; set; }

        [JsonProperty("curve", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EasingConverter))]
        public IEasing? Curve { get; set; }

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public Double? X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public Double? Y { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public Double? Value { get; set; }
    }

    public class Translate : IKeyframeType
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Translate(GlobalState globalState, double time, double x, double y, IEasing easing)
            : base(globalState, easing, time)
        {
            X = x;
            Y = y;
        }

        public override IKeyframeTypeData GenerateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = _time,
                Curve = Curve,
                X = X,
                Y = Y,
                Value = null,
            };
        }
    }

    public class Rotate : IKeyframeType
    {
        public double Value { get; private set; }

        public Rotate(GlobalState globalState, double time, double value, IEasing easing)
            : base(globalState, easing, time)
        {
            Value = value;
        }

        public override IKeyframeTypeData GenerateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = _time,
                Curve = Curve,
                X = null,
                Y = null,
                Value = Value,
            };
        }
    }

    class Shear : IKeyframeType
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Shear(GlobalState globalState, double time, double x, double y, IEasing easing)
            : base(globalState, easing, time)
        {
            X = x;
            Y = y;
        }

        public override IKeyframeTypeData GenerateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = _time,
                Curve = Curve,
                X = X,
                Y = Y,
                Value = null,
            };
        }
    }

    class Scale : IKeyframeType
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Scale(GlobalState globalState, double time, double x, double y, IEasing easing)
            : base(globalState, easing, time)
        {
            X = x;
            Y = y;
        }

        public override IKeyframeTypeData GenerateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = _time,
                Curve = Curve,
                X = X,
                Y = Y,
                Value = null,
            };
        }
    }
}
