using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using ReactiveUI;

namespace PlumJsonAnimator.ViewModels;

public partial class AttachmentInfoViewModel : ReactiveObject
{
    public readonly GlobalState GlobalState;
    private readonly HistoryManager _historyManager;

    public AttachmentInfoViewModel(GlobalState globalState, HistoryManager historyManager)
    {
        GlobalState = globalState;
        _historyManager = historyManager;
    }
}
