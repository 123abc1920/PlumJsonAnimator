using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Factories;

public class InfoPanelFactory
{
    private readonly Dictionary<InfoPanelTypes, Func<object>> _registry;

    public InfoPanelFactory(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
    {
        _registry = new()
        {
            {
                InfoPanelTypes.BONEINFO,
                () =>
                    BoneInfo.Create(
                        globalState,
                        dialogs,
                        projectManager,
                        appSettings,
                        localizationService,
                        imageExporter,
                        plumApp
                    )
            },
            {
                InfoPanelTypes.SLOTINFO,
                () =>
                    SlotInfo.Create(
                        globalState,
                        dialogs,
                        projectManager,
                        appSettings,
                        localizationService,
                        imageExporter,
                        plumApp
                    )
            },
        };
    }

    public IInfoPanel Create(InfoPanelTypes infoPanelType)
    {
        if (_registry.TryGetValue(infoPanelType, out var createFunc))
        {
            return (IInfoPanel)createFunc();
        }

        throw new ArgumentException($"Тип {infoPanelType} не добавлен в список фабрики.");
    }
}
