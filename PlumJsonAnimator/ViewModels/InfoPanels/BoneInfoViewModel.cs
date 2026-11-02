using System.Windows.Input;
using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Commands;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using ReactiveUI;

namespace PlumJsonAnimator.ViewModels;

public partial class BoneInfoViewModel : ReactiveObject
{
    public Bone? CurrentBone
    {
        get => GlobalState.SelectedItem as Bone;
        set
        {
            if (value is null)
            {
                GlobalState.SelectedItem = null;
                this.RaisePropertyChanged(nameof(CurrentBone));
                return;
            }

            if (value is Bone bone && GlobalState.SelectedItem != bone)
            {
                GlobalState.SelectedItem = (IPresentableItem?)bone;
                bone.UpdateSlots();
                this.RaisePropertyChanged(nameof(CurrentBone));
            }
        }
    }

    private Slot? _selectedSlot;
    public Slot? SelectedSlot
    {
        get => _selectedSlot;
        set => this.RaiseAndSetIfChanged(ref _selectedSlot, value);
    }

    public System.Windows.Input.ICommand AddSlot { get; }
    public System.Windows.Input.ICommand DeleteSlot { get; }
    public System.Windows.Input.ICommand RenameSlot { get; }

    public readonly GlobalState GlobalState;
    private readonly HistoryManager _historyManager;

    public BoneInfoViewModel(GlobalState globalState, HistoryManager historyManager)
    {
        GlobalState = globalState;
        _historyManager = historyManager;

        AddSlot = new Command(_ =>
        {
            if (GlobalState.SelectedItem is Bone bone)
            {
                Slot s = new Slot(GlobalState, bone);
                AddSlotCommand addSlotCommand = new AddSlotCommand(
                    GlobalState.CurrentProject,
                    s,
                    bone
                );
                _historyManager.DoCommand(addSlotCommand);
            }
        });

        DeleteSlot = new Command(execute: _ =>
        {
            if (GlobalState.SelectedItem is Bone bone && SelectedSlot is Slot slot)
            {
                _historyManager.DoCommand(
                    new DeleteSlotCommand(GlobalState.CurrentProject, slot, bone)
                );
            }
        });

        // TODO:fix renameDialog
        RenameSlot = new Command(execute: _ =>
        {
            if (SelectedSlot is Slot slot)
            {
                // _plumApp.OpenRenameDialog(slot);
            }
        });
    }
}
