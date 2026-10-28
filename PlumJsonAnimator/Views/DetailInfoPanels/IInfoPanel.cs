namespace PlumJsonAnimator.Views;

public interface IInfoPanel
{
    static abstract InfoPanelTypes InfoPanelType { get; }
    static abstract IInfoPanel Create();
}
