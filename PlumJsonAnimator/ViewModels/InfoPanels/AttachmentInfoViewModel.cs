using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.ViewModels;

public partial class AttachmentInfoViewModel : ReactiveObject
{
    public Attachment? CurrentAttachment
    {
        get => GlobalState.SelectedItem as Attachment;
        set
        {
            if (value is null)
            {
                GlobalState.SelectedItem = null;
                this.RaisePropertyChanged(nameof(CurrentAttachment));
                return;
            }

            if (value is Attachment attachment && GlobalState.SelectedItem != attachment)
            {
                GlobalState.SelectedItem = (IPresentableItem?)attachment;
                this.RaisePropertyChanged(nameof(CurrentAttachment));
            }
        }
    }

    public ObservableCollection<Slot> Slots => GlobalState.CurrentProject.Slots;
    private Slot? _selectedSlot = null;
    public Slot? SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            if (_selectedSlot != value)
            {
                this.RaiseAndSetIfChanged(ref _selectedSlot, value);
            }
        }
    }

    public ObservableCollection<Skin> Skins => GlobalState.CurrentProject.Skins;
    private Skin? _selectedSkin = null;
    public Skin? SelectedSkin
    {
        get => _selectedSkin;
        set
        {
            if (_selectedSkin != value)
            {
                this.RaiseAndSetIfChanged(ref _selectedSkin, value);
            }
        }
    }

    [Reactive]
    public string Type { get; set; }

    [Reactive]
    public object? CurrentAttachmentProperties { get; set; } = null;

    public readonly GlobalState GlobalState;
    private readonly HistoryManager _historyManager;

    public AttachmentInfoViewModel(
        GlobalState globalState,
        HistoryManager historyManager,
        AttachmentTypeConverter attachmentTypeConverter
    )
    {
        GlobalState = globalState;
        _historyManager = historyManager;

        Type = attachmentTypeConverter.GetAttachmentTypeString(CurrentAttachment!.AttachmentType);
        SelectedSlot = Slots.FirstOrDefault(s =>
            s == GlobalState.CurrentProject.CurrentSkin.GetSlot(CurrentAttachment)
        );
        SelectedSkin = Skins.FirstOrDefault(s => s == GlobalState.CurrentProject.CurrentSkin);
        CurrentAttachmentProperties = CurrentAttachment.CreateInfoPanel();
    }
}
