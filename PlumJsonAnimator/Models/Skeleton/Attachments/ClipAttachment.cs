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
        Canvas? canvas = null,
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

    public override void Draw(Canvas canvas)
    {
        foreach (PointModel vertice in Vertices)
        {
            var dot = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Red,
            };
            Canvas.SetLeft(dot, vertice.X + canvas.Width / 2);
            Canvas.SetTop(dot, vertice.Y + canvas.Height / 2);
            canvas.Children.Add(dot);
        }

        for (int i = 0; i < Vertices.Count - 1; i++)
        {
            var p1 = Vertices[i];
            var p2 = Vertices[i + 1];

            var line = new Line
            {
                StartPoint = new Point(p1.X + 2 + canvas.Width / 2, p1.Y + 2 + canvas.Height / 2),
                EndPoint = new Point(p2.X + 2 + canvas.Width / 2, p2.Y + 2 + canvas.Height / 2),
                Stroke = Brushes.Red,
                StrokeThickness = 1,
            };
            canvas.Children.Add(line);
        }

        if (Vertices.Count > 1)
        {
            var first = Vertices[0];
            var last = Vertices[Vertices.Count - 1];

            var closingLine = new Line
            {
                StartPoint = new Point(
                    last.X + 2 + canvas.Width / 2,
                    last.Y + 2 + canvas.Height / 2
                ),
                EndPoint = new Point(
                    first.X + 2 + canvas.Width / 2,
                    first.Y + 2 + canvas.Height / 2
                ),
                Stroke = Brushes.Red,
                StrokeThickness = 1,
            };
            canvas.Children.Add(closingLine);
        }
    }
}
