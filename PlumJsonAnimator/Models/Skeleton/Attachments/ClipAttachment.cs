using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Common;

namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

public class ClipAttachment : Attachment
{
    public override AttachmentTypes AttachmentType => AttachmentTypes.CLIPPING;
    public List<PointModel> Points { get; } = new List<PointModel>();

    private string _endSlotName { get; set; } = "";
    private int _vertexCount { get; set; } = 3;
    private List<PointModel> _vertices { get; set; } = new List<PointModel>();

    public ClipAttachment(
        AttachmentDTO attachmentDTO,
        AttachmentTypeConverter attachmentTypeConverter
    )
        : base(attachmentDTO.Name!, attachmentTypeConverter)
    {
        _endSlotName = attachmentDTO.AttachmentData!.End!;
        _vertexCount = attachmentDTO.AttachmentData!.VertexCount!;

        var xyArray = attachmentDTO.AttachmentData!.Vertices!;
        for (int i = 0; i < _vertexCount * 2; i += 2)
        {
            _vertices.Add(new PointModel((int)xyArray[i], (int)xyArray[i + 1]));
        }
    }

    public static Attachment Create(
        AttachmentTypeConverter attachmentTypeConverter,
        AttachmentDTO attachmentDTO
    )
    {
        return new ClipAttachment(attachmentDTO, attachmentTypeConverter);
    }

    public override void DrawSlotAttachment(
        Slot slot,
        Canvas? canvas = null,
        ClipAttachment? clipAttachment = null
    )
    {
        throw new NotImplementedException();
    }

    private List<double> MakeFlatVerticesArray()
    {
        List<double> arr = new List<double>();
        foreach (PointModel pointModel in _vertices)
        {
            arr.Add(pointModel.X);
            arr.Add(pointModel.Y);
        }
        return arr;
    }

    public override AttachmentData GenerateJSONData()
    {
        return new AttachmentData
        {
            Name = Name,
            Type = _attachmentTypeConverter.GetAttachmentTypeString(AttachmentType),
            End = _endSlotName,
            VertexCount = _vertexCount,
            Vertices = MakeFlatVerticesArray(),
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

    public override void SetPos(double x, double y, double a) { }

    public override void SetSize(double width, double height) { }
}
