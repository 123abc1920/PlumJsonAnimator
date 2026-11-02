using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

    public ObservableCollection<Bone> Items => GlobalState.CurrentProject.MainSkeleton.Bones;

    private Bone? _selectedItem = null;
    public Bone? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem != value)
            {
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
                CurrentSlot.BoundedBone = value;
            }
        }
    }

    

    public readonly GlobalState GlobalState;
    private readonly HistoryManager _historyManager;

    public SlotInfoViewModel(GlobalState globalState, HistoryManager historyManager)
    {
        GlobalState = globalState;
        _historyManager = historyManager;

        SelectedItem = Items.FirstOrDefault(b => b == CurrentSlot.BoundedBone);
    }
}
