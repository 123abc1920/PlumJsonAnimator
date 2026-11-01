using System.Windows.Input;
using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Common;
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

    public ICommand AddSlot { get; }
    public ICommand DeleteSlot { get; }
    public ICommand RenameSlot { get; }

    public readonly GlobalState GlobalState;

    public BoneInfoViewModel(GlobalState globalState)
    {
        GlobalState = globalState;

        // TODO: fix commands. separate PlumApp parts
        AddSlot = new Command(_ =>
        {
            //plumApp.AddSlot();
        });

        DeleteSlot = new Command(parameter =>
        {
            // if (parameter is ListBox SlotsList)
            // {
            //     Slot? selectedSlot = SlotsList.SelectedItem as Slot;
            //     plumApp.DeleteSlot(selectedSlot);
            // }
        });

        RenameSlot = new Command(parameter =>
        {
            // if (parameter is ListBox SlotsList)
            // {
            //     Slot? selectedSlot = SlotsList.SelectedItem as Slot;
            //     _plumApp.OpenRenameDialog(selectedSlot);
            // }
        });
    }
}
