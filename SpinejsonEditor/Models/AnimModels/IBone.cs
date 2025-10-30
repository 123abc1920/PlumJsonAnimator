namespace AnimModels
{
    public abstract class IBone
    {
        public double x;
        public double y;
        public double a;
        public int id;

        public virtual void move(double x, double y) { }

        public virtual void scale(double x, double y) { }

        public virtual void rotate(double a) { }
    }
}
