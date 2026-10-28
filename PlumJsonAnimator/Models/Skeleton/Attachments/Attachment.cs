using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

/// <summary>
/// Attachment class. Helps to bind slot and res or another functions
/// </summary>
public abstract class Attachment : INotifyable
{
    public string Name { get; set; }
    public abstract AttachmentTypes AttachmentType { get; }

    protected readonly AttachmentTypeConverter _attachmentTypeConverter;

    public Attachment(string name, AttachmentTypeConverter attachmentTypeConverter)
    {
        Name = name;
        _attachmentTypeConverter = attachmentTypeConverter;
    }

    public abstract AttachmentData GenerateJSONData();

    public abstract void SetPos(double x, double y, double a);

    public abstract void SetSize(double width, double height);

    public abstract void DrawSlotAttachment(
        Slot slot,
        Canvas? canvas = null,
        ClipAttachment? clipAttachment = null
    );
}
