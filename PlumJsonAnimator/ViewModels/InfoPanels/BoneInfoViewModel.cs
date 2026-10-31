using System;
using System.Windows.Input;
using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.States;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class BoneInfoViewModel : ViewModelBase
{
    public Bone? CurrentBone
    {
        get => _globalState.CurrentBone;
        set
        {
            if (_globalState.CurrentBone != value)
            {
                _globalState.CurrentBone = value;
                _globalState.CurrentBone?.UpdateSlots();

                if (_globalState.CurrentBone != null)
                {
                    _panelState.CurrentInfoPanel = _infoPanelFactory.Create(
                        Views.PanelTypes.SLOTINFO
                    );
                }

                OnPropertyChanged(nameof(CurrentBone));
            }
        }
    }

    public ICommand AddSlot { get; }
    public ICommand DeleteSlot { get; }
    public ICommand RenameSlot { get; }

    private readonly InfoPanelFactory _infoPanelFactory;
    private readonly PanelState _panelState;

    public BoneInfoViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp,
        InfoPanelFactory infoPanelFactory,
        PanelState panelState
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
        _infoPanelFactory = infoPanelFactory;
        _panelState = panelState;

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
