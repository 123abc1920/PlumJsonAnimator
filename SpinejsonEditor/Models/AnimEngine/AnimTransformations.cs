using System;

namespace AnimTransformations
{
    public abstract class IKeyframeType
    {
        public double time;

        public abstract String generateCode();
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

        public override string generateCode()
        {
            return "{\"time\": " + this.time.ToString().Replace(",", ".") + "}";
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

        public override string generateCode()
        {
            return "{\"time\": " + this.time.ToString().Replace(",", ".") + "}";
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

        public override string generateCode()
        {
            return "{\"time\": " + this.time.ToString().Replace(",", ".") + "}";
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

        public override string generateCode()
        {
            return "{\"time\": " + this.time.ToString().Replace(",", ".") + "}";
        }
    }
}
