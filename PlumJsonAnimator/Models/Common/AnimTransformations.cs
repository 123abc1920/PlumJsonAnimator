using System;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Easing;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Common
{
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
        public IEasing? Curve { get; set; } = new LinearEasing();

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public Double? X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public Double? Y { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public Double? Value { get; set; }

        public bool ShouldSerializeCurve() => Curve is not LinearEasing && Curve is not null;
    }

    public class TranslateKeyFrame : IKeyframeType
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public TranslateKeyFrame(
            GlobalState globalState,
            double time,
            double x,
            double y,
            IEasing easing
        )
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

    public class RotateKeyFrame : IKeyframeType
    {
        public double Value { get; private set; }

        public RotateKeyFrame(GlobalState globalState, double time, double value, IEasing easing)
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

    class ShearKeyFrame : IKeyframeType
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public ShearKeyFrame(
            GlobalState globalState,
            double time,
            double x,
            double y,
            IEasing easing
        )
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

    class ScaleKeyFrame : IKeyframeType
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public ScaleKeyFrame(
            GlobalState globalState,
            double time,
            double x,
            double y,
            IEasing easing
        )
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
