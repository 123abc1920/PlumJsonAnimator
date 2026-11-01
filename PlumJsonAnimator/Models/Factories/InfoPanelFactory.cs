using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Factories;

public class InfoPanelFactory
{
    private readonly Dictionary<PanelTypes, Func<GlobalState, object>> _registry;

    public InfoPanelFactory()
    {
        _registry = new()
        {
            [PanelTypes.BONEINFO] = gs => BoneInfo.Create(gs),
            [PanelTypes.SLOTINFO] = gs => SlotInfo.Create(gs),
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
