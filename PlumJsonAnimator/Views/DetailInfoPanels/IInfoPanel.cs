using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.DTO;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Views;

public interface IInfoPanel
{
    static abstract PanelTypes InfoPanelType { get; }
    static abstract IInfoPanel Create(InfoPanelDTO infoPanelDTO);
}
