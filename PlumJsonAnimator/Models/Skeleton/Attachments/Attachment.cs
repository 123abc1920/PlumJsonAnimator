using System.Collections.Generic;
using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;

// TODO: fix SetSize GetSize Size
namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

/// <summary>
/// Attachment class. Helps to bind slot and res or another functions
/// </summary>
public abstract class Attachment : INotifyable
{
    public string Name { get; set; }
    public abstract AttachmentTypes AttachmentType { get; }

    public double X { get; set; }
    public double Y { get; set; }
    public double A { get; set; }

    protected int? Width { get; set; } = null;
    protected int? Height { get; set; } = null;

    protected readonly AttachmentTypeConverter _attachmentTypeConverter;

    public Attachment(string name, AttachmentTypeConverter attachmentTypeConverter)
    {
        Name = name;
        _attachmentTypeConverter = attachmentTypeConverter;
    }

    public void SetPos(double x, double y, double a)
    {
        X = x;
        Y = y;
        A = a;
    }

    public virtual Res? GetRes()
    {
        return null;
    }

    public abstract AttachmentData GenerateJSONData();

    public void SetSize(double width, double height)
    {
        Width = (int)width;
        Height = (int)height;
    }

    public Dictionary<string, int?> GetSize()
    {
        return new Dictionary<string, int?>() { ["width"] = Width, ["height"] = Height };
    }

    public abstract void DrawSlotAttachment(
        Slot slot,
        Canvas? canvas = null,
        ClipAttachment? clipAttachment = null
    );
}
