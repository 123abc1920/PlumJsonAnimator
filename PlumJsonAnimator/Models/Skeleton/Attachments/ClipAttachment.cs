using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Views;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

public class ClipAttachment : Attachment
{
    public override AttachmentTypes AttachmentType => AttachmentTypes.CLIPPING;

    private Project? _currentProject;

    public List<PointModel> Points { get; } = new List<PointModel>();

    [Reactive]
    public string? EndSlotName { get; set; } = "";
    public int? VertexCount { get; set; } = 3;
    public ObservableCollection<PointModel> Vertices { get; set; } =
        new ObservableCollection<PointModel>();

    public ClipAttachment(
        AttachmentDTO attachmentDTO,
        AttachmentTypeConverter attachmentTypeConverter
    )
        : base(attachmentDTO.Name!, attachmentTypeConverter)
    {
        EndSlotName = attachmentDTO.AttachmentData?.End;
        VertexCount = attachmentDTO.AttachmentData?.VertexCount;

        var xyArray = attachmentDTO.AttachmentData?.Vertices;
        if (!(xyArray is null))
        {
            for (int i = 0; i < VertexCount * 2; i += 2)
            {
                Vertices.Add(new PointModel((int)xyArray[i], (int)xyArray[i + 1]));
            }
        }

        _currentProject = attachmentDTO.CurrentProject;
    }

    public static Attachment Create(
        AttachmentTypeConverter attachmentTypeConverter,
        AttachmentDTO attachmentDTO
    )
    {
        return new ClipAttachment(attachmentDTO, attachmentTypeConverter);
    }

    public override void DrawAttachment(
        Slot slot,
        DrawingContext? context = null,
        ClipAttachment? clipAttachment = null
    ) { }

    private List<double> MakeFlatVerticesArray()
    {
        List<double> arr = new List<double>();
        foreach (PointModel pointModel in Vertices)
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
            End = EndSlotName,
            VertexCount = VertexCount,
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

    public override object CreateInfoPanel()
    {
        ObservableCollection<Slot>? slots = _currentProject?.Slots;
        return new ClipAttachmentInfo(this, slots);
    }

    private static readonly IPen ClipOutlinePen = new Pen(Brushes.Red, 1);

    public override void Draw(DrawingContext context)
    {
        if (Vertices == null || Vertices.Count == 0)
            return;

        foreach (PointModel vertice in Vertices)
        {
            context.DrawEllipse(Brushes.Red, null, new Point(vertice.X, vertice.Y), 2.5, 2.5);
        }

        for (int i = 0; i < Vertices.Count - 1; i++)
        {
            var p1 = Vertices[i];
            var p2 = Vertices[i + 1];

            context.DrawLine(ClipOutlinePen, new Point(p1.X, p1.Y), new Point(p2.X, p2.Y));
        }

        if (Vertices.Count > 1)
        {
            var first = Vertices[0];
            var last = Vertices[Vertices.Count - 1];

            context.DrawLine(
                ClipOutlinePen,
                new Point(last.X, last.Y),
                new Point(first.X, first.Y)
            );
        }
    }
}
