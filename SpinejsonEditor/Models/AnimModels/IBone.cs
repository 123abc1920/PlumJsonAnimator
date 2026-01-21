using System.Collections.Generic;

namespace AnimModels
{
    public class IBone
    {
        public double x;
        public double y;
        public double a;
        public int id;
        public bool isBone;

        public virtual void move(double x, double y) { }

        public virtual void scale(double x, double y) { }

        public virtual void rotate(double a) { }

        public virtual IEnumerable<IBone> CombinedChildren
        {
            get { yield break; }
        }
    }
}
