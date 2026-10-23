using System.Collections.Generic;
using Avalonia.Controls;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;

// TODO: fix SetSize GetSize Size
namespace PlumJsonAnimator.Models.SkeletonNameSpace;

/// <summary>
/// Attachment class. Helps to bind slot and res or another functions
/// </summary>
public abstract class Attachment : INotifyable
{
    public string Name { get; set; }
    protected readonly string AttachmentType;

    public double _x;
    public double _y;
    public double _a;

    protected int? _width = null;
    protected int? _height = null;

    public Attachment(string name, string type)
    {
        Name = name;
        AttachmentType = type;
    }

    public void SetPos(double x, double y, double a)
    {
        _x = x;
        _y = y;
        _a = a;
    }

    public virtual Res? GetRes()
    {
        return null;
    }

    public abstract AttachmentData GenerateJSONData();

    public void SetSize(double width, double height)
    {
        _width = (int)width;
        _height = (int)height;
    }

    public Dictionary<string, int?> GetSize()
    {
        return new Dictionary<string, int?>() { ["width"] = _width, ["height"] = _height };
    }

    public abstract void DrawSlotAttachment(
        Slot slot,
        Canvas? canvas = null,
        ClipAttachment? clipAttachment = null
    );
}
