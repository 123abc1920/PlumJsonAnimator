using System;
using Newtonsoft.Json;

namespace AnimTransformations
{
    public abstract class IKeyframeType
    {
        public double time;

        public abstract IKeyframeTypeData generateJSONData();

        public String generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
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

        public Translate(double _time, double _x, double _y)
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

        public Rotate(double _time, double _value)
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

        public Shear(double _time, double _x, double _y)
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

        public Scale(double _time, double _x, double _y)
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
