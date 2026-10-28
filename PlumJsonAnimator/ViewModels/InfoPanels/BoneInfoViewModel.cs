using System.Windows.Input;
using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class BoneInfoViewModel : ViewModelBase
{
    public Bone? CurrentBone
    {
        get => _globalState.CurrentBone;
    }

    public ICommand AddSlot { get; }
    public ICommand DeleteSlot { get; }
    public ICommand RenameSlot { get; }

    public BoneInfoViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
        : base(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp
        )
    {
        AddSlot = new Command(_ =>
        {
            PlumApp.AddSlot();
        });

        DeleteSlot = new Command(parameter =>
        {
            if (parameter is ListBox SlotsList)
            {
                Slot? selectedSlot = SlotsList.SelectedItem as Slot;
                PlumApp.DeleteSlot(selectedSlot);
            }
        });

        RenameSlot = new Command(parameter =>
        {
            if (parameter is ListBox SlotsList)
            {
                Slot? selectedSlot = SlotsList.SelectedItem as Slot;
                OpenRenameDialog(selectedSlot);
            }
        });
    }
}
