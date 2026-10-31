using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.Common.States;

public partial class PanelState : ReactiveObject
{
    [Reactive]
    public object? CurrentInfoPanel { get; set; }
}
