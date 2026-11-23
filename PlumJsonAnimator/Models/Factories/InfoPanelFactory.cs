using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.DTO;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.Models.Factories;

public class InfoPanelFactory
{
    private readonly Dictionary<PanelTypes, Func<InfoPanelDTO, object>> _registry;

    private HistoryManager _historyManager;
    private AttachmentTypeConverter _attachmentTypeConverter;
    private AttachmentFactory _attachmentFactory;

    public InfoPanelFactory(
        HistoryManager historyManager,
        AttachmentTypeConverter attachmentTypeConverter,
        AttachmentFactory attachmentFactory
    )
    {
        _historyManager = historyManager;
        _attachmentFactory = attachmentFactory;
        _attachmentTypeConverter = attachmentTypeConverter;

        _registry = new()
        {
            [PanelTypes.BONEINFO] = dto => BoneInfo.Create(dto),
            [PanelTypes.SLOTINFO] = dto => SlotInfo.Create(dto),
            [PanelTypes.ATTACHMENTINFO] = dto => AttachmentInfo.Create(dto),
        };
    }

    public IInfoPanel Create(InfoPanelDTO dto, PanelTypes infoPanelType)
    {
        if (_registry.TryGetValue(infoPanelType, out var createFunc))
        {
            dto.AttachmentFactory = _attachmentFactory;
            dto.AttachmentTypeConverter = _attachmentTypeConverter;
            dto.HistoryManager = _historyManager;

            return (IInfoPanel)createFunc(dto);
        }

        throw new ArgumentException($"Тип {infoPanelType} не добавлен в список фабрики.");
    }
}
