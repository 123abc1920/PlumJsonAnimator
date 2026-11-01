using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.States;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class SlotInfoViewModel : ViewModelBase
{
    public Bone? CurrentBone
    {
        get => _globalState.SelectedItem as Bone;
        set
        {
            if (value is null)
            {
                _globalState.SelectedItem = null;
                _panelState.CurrentInfoPanel = null;
                OnPropertyChanged(nameof(CurrentBone));
                return;
            }

            if (value is Bone bone && _globalState.SelectedItem != bone)
            {
                _globalState.SelectedItem = (IPresentableItem?)bone;
                bone.UpdateSlots();

                _panelState.CurrentInfoPanel = _infoPanelFactory.Create(Views.PanelTypes.SLOTINFO);

                OnPropertyChanged(nameof(CurrentBone));
            }
        }
    }

    private readonly InfoPanelFactory _infoPanelFactory;
    private readonly PanelState _panelState;

    public SlotInfoViewModel(
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
    }
}
