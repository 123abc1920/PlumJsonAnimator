using PlumJsonAnimator.Models.Resources;

namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

public class AttachmentDTO
{
    public required string Name { get; set; }
    public AttachmentData? AttachmentData { get; set; }
    public Res? Res { get; set; }
}
