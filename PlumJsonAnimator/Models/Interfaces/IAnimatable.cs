namespace PlumJsonAnimator.Models.Interfaces;

public interface IAnimatable
{
    void Move(double x, double y);
    void Rotate(double a);
    void Scale(double x, double y);
    void Shear(double x, double y);

    double ShearX { get; set; }
    double ShearY { get; set; }

    double ScaleX { get; set; }
    double ScaleY { get; set; }

    double X { get; set; }
    double Y { get; set; }

    double A { get; set; }
}
