using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.Services;
using ReactiveUI;

namespace PlumJsonAnimator.ViewModels;

public partial class SlotInfoViewModel : ReactiveObject
{
    public Slot? CurrentSlot
    {
        get => GlobalState.SelectedItem as Slot;
        set
        {
            if (value is null)
            {
                GlobalState.SelectedItem = null;
                this.RaisePropertyChanged(nameof(CurrentSlot));
                return;
            }

            if (value is Slot slot && GlobalState.SelectedItem != slot)
            {
                GlobalState.SelectedItem = (IPresentableItem?)slot;
                this.RaisePropertyChanged(nameof(CurrentSlot));
            }
        }
    }

    public ObservableCollection<Bone> Bones => GlobalState.CurrentProject.MainSkeleton.Bones;

    private Bone? _selectedBone = null;
    public Bone? SelectedBone
    {
        get => _selectedBone;
        set
        {
            if (_selectedBone != value)
            {
                this.RaiseAndSetIfChanged(ref _selectedBone, value);
                CurrentSlot.BoundedBone = value;
            }
        }
    }

    private Attachment? _selectedAttachment;
    public Attachment? SelectedAttachment
    {
        get => _selectedAttachment;
        set => this.RaiseAndSetIfChanged(ref _selectedAttachment, value);
    }

    public ICommand OpenBone { get; }
    public ICommand OpenAttachment { get; }

    public readonly GlobalState GlobalState;
    private readonly HistoryManager _historyManager;

    public SlotInfoViewModel(GlobalState globalState, HistoryManager historyManager)
    {
        GlobalState = globalState;
        _historyManager = historyManager;

        SelectedBone = Bones.FirstOrDefault(b => b == CurrentSlot.BoundedBone);
        SelectedAttachment = CurrentSlot.CurrentAttachment;

        OpenBone = new Command(_ =>
        {
            GlobalState.SelectedItem = SelectedBone;
        });
        OpenAttachment = new Command(_ =>
        {
            // TODO: Open Attachment command
        });
    }
}
