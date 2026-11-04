using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Views;

public interface IInfoPanel
{
    static abstract PanelTypes InfoPanelType { get; }
    static abstract IInfoPanel Create(
        GlobalState globalState,
        HistoryManager historyManager,
        AttachmentTypeConverter? attachmentTypeConverter = null
    );
}
