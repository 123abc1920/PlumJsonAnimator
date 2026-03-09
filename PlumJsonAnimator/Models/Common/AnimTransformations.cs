using System;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Models.Common
{
    public enum KeyFrameTypes
    {
        TRANSLATE = 0,
        ROTATE,
        SCALE,
        SHEAR,
    }

    public abstract class IKeyframeType
    {
        public double time;

        public abstract IKeyframeTypeData generateJSONData();

        protected GlobalState globalState;

        public IKeyframeType(GlobalState globalState)
        {
            this.globalState = globalState;
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), this.globalState.jsonSettings);
        }
    }

    public class IKeyframeTypeData()
    {
        [JsonProperty("time")]
        public Double? Time { get; set; }

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public Double? X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public Double? Y { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public Double? Value { get; set; }
    }

    public class Translate : IKeyframeType
    {
        public double x;
        public double y;

        public Translate(GlobalState globalState, double _time, double _x, double _y)
            : base(globalState)
        {
            this.time = _time;
            this.x = _x;
            this.y = _y;
        }

        public override IKeyframeTypeData generateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = this.time,
                X = this.x,
                Y = this.y,
                Value = null,
            };
        }
    }

    public class Rotate : IKeyframeType
    {
        public double value;

        public Rotate(GlobalState globalState, double _time, double _value)
            : base(globalState)
        {
            this.time = _time;
            this.value = _value;
        }

        public override IKeyframeTypeData generateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = this.time,
                X = null,
                Y = null,
                Value = this.value,
            };
        }
    }

    class Shear : IKeyframeType
    {
        public double x;
        public double y;

        public Shear(GlobalState globalState, double _time, double _x, double _y)
            : base(globalState)
        {
            this.time = _time;
            this.x = _x;
            this.y = _y;
        }

        public override IKeyframeTypeData generateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = this.time,
                X = this.x,
                Y = this.y,
                Value = null,
            };
        }
    }

    class Scale : IKeyframeType
    {
        public double x;
        public double y;

        public Scale(GlobalState globalState, double _time, double _x, double _y)
            : base(globalState)
        {
            this.time = _time;
            this.x = _x;
            this.y = _y;
        }

        public override IKeyframeTypeData generateJSONData()
        {
            return new IKeyframeTypeData
            {
                Time = this.time,
                X = this.x,
                Y = this.y,
                Value = null,
            };
        }
    }
}
