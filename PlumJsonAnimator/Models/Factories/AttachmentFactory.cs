using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

namespace PlumJsonAnimator.Models.Factories;

public class AttachmentFactory
{
    private readonly ResFactory _resFactory;
    private readonly AttachmentTypeConverter _attachmentTypeConverter;

    public AttachmentFactory(ResFactory resFactory)
    {
        _resFactory = resFactory;

        _attachmentTypeConverter = new AttachmentTypeConverter();
    }

    public Attachment? CreateAttachment(AttachmentData attachmentData)
    {
        var attachmentType = _attachmentTypeConverter.CreateAttachment(attachmentData.Type);

        switch (attachmentType)
        {
            case AttachmentTypes.REGION:
                var imageRes = _resFactory.CreateRes(
                    ResTypes.IMAGE,
                    new ResDTO
                    {
                        ImageName = "",
                        ImageExt = "",
                        ImagePath = "",
                    }
                );
                if (!(imageRes is null))
                {
                    return new RegionAttachment(attachmentData, (ImageRes)imageRes);
                }
                break;
            case AttachmentTypes.CLIPPING:
                return new ClipAttachment(attachmentData);
        }

        return null;
    }

    public Attachment? CreateAttachmentWithRes(AttachmentData attachmentData, Res? res)
    {
        if (res is null)
        {
            return null;
        }

        var attachmentType = _attachmentTypeConverter.CreateAttachment(attachmentData.Type);

        switch (attachmentType)
        {
            case AttachmentTypes.REGION:
                return new RegionAttachment(attachmentData, (ImageRes)res);
            case AttachmentTypes.CLIPPING:
                return new ClipAttachment(attachmentData);
        }

        return null;
    }
}
