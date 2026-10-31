using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.States;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Factories;

public class InfoPanelFactory
{
    private readonly Dictionary<PanelTypes, Func<object>> _registry;

    public InfoPanelFactory(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp,
        PanelState panelState
    )
    {
        _registry = new()
        {
            {
                PanelTypes.BONEINFO,
                () =>
                    BoneInfo.Create(
                        globalState,
                        dialogs,
                        projectManager,
                        appSettings,
                        localizationService,
                        imageExporter,
                        plumApp,
                        this,
                        panelState
                    )
            },
            {
                PanelTypes.SLOTINFO,
                () =>
                    SlotInfo.Create(
                        globalState,
                        dialogs,
                        projectManager,
                        appSettings,
                        localizationService,
                        imageExporter,
                        plumApp,
                        this,
                        panelState
                    )
            },
        };
    }

    public IInfoPanel Create(PanelTypes infoPanelType)
    {
        if (_registry.TryGetValue(infoPanelType, out var createFunc))
        {
            return (IInfoPanel)createFunc();
        }

        throw new ArgumentException($"Тип {infoPanelType} не добавлен в список фабрики.");
    }
}
