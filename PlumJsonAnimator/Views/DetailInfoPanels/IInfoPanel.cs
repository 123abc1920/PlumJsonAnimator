using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Views;

public interface IInfoPanel
{
    static abstract PanelTypes InfoPanelType { get; }
    static abstract IInfoPanel Create(GlobalState globalState);
}
