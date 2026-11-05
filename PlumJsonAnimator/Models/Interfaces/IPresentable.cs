using Avalonia.Controls;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Interfaces;

public interface IPresentableItem
{
    public PanelTypes PanelType { get; }
    public void Draw(Canvas canvas);
}
