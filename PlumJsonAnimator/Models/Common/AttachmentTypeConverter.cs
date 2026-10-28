using System;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

namespace PlumJsonAnimator.Common;

public class AttachmentTypeConverter
{
    public AttachmentTypes CreateAttachmentTypeData(string attachmentStr)
    {
        if (Enum.TryParse(attachmentStr, true, out AttachmentTypes result))
        {
            return result;
        }

        return default(AttachmentTypes);
    }

    public string GetAttachmentTypeString(AttachmentTypes attachmentType)
    {
        return attachmentType.ToString().ToLowerInvariant();
    }
}
