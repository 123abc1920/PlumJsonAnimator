using System;
using System.Collections.Generic;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Factories;

public class InfoPanelFactory
{
    private readonly Dictionary<InfoPanelTypes, Func<object>> _registry;

    public InfoPanelFactory()
    {
        _registry = new()
        {
            { InfoPanelTypes.BONEINFO, () => BoneInfo.Create() },
            { InfoPanelTypes.SLOTINFO, () => SlotInfo.Create() },
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
