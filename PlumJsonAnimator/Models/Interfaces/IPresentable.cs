using Avalonia.Controls;
using PlumJsonAnimator.Models.DTO;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Interfaces;

public interface IPresentableItem
{
    public PanelTypes PanelType { get; }
    public void Draw(Canvas canvas);
    public InfoPanelDTO GetInfoPanelDTO();
}
