using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models.DTO;

public class InfoPanelDTO
{
    public AttachmentFactory? AttachmentFactory { get; set;}
    public GlobalState? GlobalState { get; set; }
    public HistoryManager? HistoryManager { get; set; }
    public AttachmentTypeConverter? AttachmentTypeConverter { get; set; }
}
