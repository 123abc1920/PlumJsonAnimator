using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using PlumJsonAnimator.Models.Common;

namespace PlumJsonAnimator.Models.SkeletonNameSpace;

public class ClipAttachment : Attachment
{
    private const string TypeStr = "clipping";
    public List<PointModel> Points { get; } = new List<PointModel>();

    public ClipAttachment(string name)
        : base(name, TypeStr) { }

    public override void DrawSlotAttachment(
        Slot slot,
        Canvas? canvas = null,
        ClipAttachment? clipAttachment = null
    )
    {
        throw new NotImplementedException();
    }

    public override AttachmentData GenerateJSONData()
    {
        return new AttachmentData
        {
            Name = Name,
            Width = _width,
            Height = _height,
            X = _x,
            Y = _y,
            A = _a,
            Type = AttachmentType,
        };
    }

    public List<Point> GetAvaloniaPointsList()
    {
        List<Point> result = new List<Point>();

        foreach (var p in this.Points)
        {
            result.Add(new Point(p.X, p.Y));
        }

        return result;
    }
}
