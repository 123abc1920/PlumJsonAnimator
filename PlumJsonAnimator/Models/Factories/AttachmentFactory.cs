using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

namespace PlumJsonAnimator.Models.Factories;

public class AttachmentFactory
{
    private readonly AttachmentTypeConverter _attachmentTypeConverter;

    private readonly Dictionary<
        AttachmentTypes,
        Func<AttachmentTypeConverter, AttachmentDTO, Attachment>
    > _registry;

    public AttachmentFactory(AttachmentTypeConverter attachmentTypeConverter)
    {
        _attachmentTypeConverter = attachmentTypeConverter;

        _registry = new Dictionary<
            AttachmentTypes,
            Func<AttachmentTypeConverter, AttachmentDTO, Attachment>
        >
        {
            { AttachmentTypes.REGION, (converter, dto) => RegionAttachment.Create(converter, dto) },
            { AttachmentTypes.CLIPPING, (converter, dto) => ClipAttachment.Create(converter, dto) },
        };
    }

    public Attachment CreateAttachmentByStr(string typeStr, AttachmentDTO attachmentDTO)
    {
        var type = _attachmentTypeConverter.CreateAttachmentTypeData(typeStr);

        if (_registry.TryGetValue(type, out var createFunc))
        {
            return createFunc(_attachmentTypeConverter, attachmentDTO);
        }

        throw new ArgumentException($"Тип {type} не добавлен в список фабрики.");
    }

    public Attachment CreateAttachmentByType(AttachmentTypes type, AttachmentDTO attachmentDTO)
    {
        if (_registry.TryGetValue(type, out var createFunc))
        {
            return createFunc(_attachmentTypeConverter, attachmentDTO);
        }

        throw new ArgumentException($"Тип {type} не добавлен в список фабрики.");
    }
}
