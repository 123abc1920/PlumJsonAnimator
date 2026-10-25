using System;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

namespace PlumJsonAnimator.Common;

public class AttachmentTypeConverter
{
    public AttachmentTypes CreateAttachment(string attachmentStr)
    {
        if (Enum.TryParse(attachmentStr, true, out AttachmentTypes result))
        {
            return result;
        }

        return default(AttachmentTypes);
    }
}
