using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Factories;

public class InfoPanelFactory
{
    private readonly Dictionary<PanelTypes, Func<GlobalState, object>> _registry;

    public InfoPanelFactory(
        HistoryManager historyManager,
        AttachmentTypeConverter attachmentTypeConverter
    )
    {
        _registry = new()
        {
            [PanelTypes.BONEINFO] = gs => BoneInfo.Create(gs, historyManager),
            [PanelTypes.SLOTINFO] = gs => SlotInfo.Create(gs, historyManager),
            [PanelTypes.ATTACHMENTINFO] = gs =>
                AttachmentInfo.Create(gs, historyManager, attachmentTypeConverter),
        };
    }

    public IInfoPanel Create(GlobalState globalState, PanelTypes infoPanelType)
    {
        if (_registry.TryGetValue(infoPanelType, out var createFunc))
        {
            return (IInfoPanel)createFunc(globalState);
        }

        throw new ArgumentException($"Тип {infoPanelType} не добавлен в список фабрики.");
    }
}
