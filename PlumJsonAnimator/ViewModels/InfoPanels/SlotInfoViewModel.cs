using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.States;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using ReactiveUI;

namespace PlumJsonAnimator.ViewModels;

public partial class SlotInfoViewModel : ReactiveObject
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

    public readonly GlobalState GlobalState;

    public SlotInfoViewModel(GlobalState globalState)
    {
        GlobalState = globalState;
    }
}
