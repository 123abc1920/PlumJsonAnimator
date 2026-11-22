using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

public class ClipAttachment : Attachment
{
    public override AttachmentTypes AttachmentType => AttachmentTypes.CLIPPING;

    private Project? _currentProject;

    [Reactive]
    public string? EndSlotName { get; set; } = "";
    public int? VertexCount { get; set; } = 0;
    public ObservableCollection<PointModel> Vertices { get; set; } =
        new ObservableCollection<PointModel>();

    private PointModel? _selectedVertice;
    public PointModel? SelectedVertice
    {
        get => _selectedVertice;
        set => this.RaiseAndSetIfChanged(ref _selectedVertice, value);
    }

    public ClipAttachment(
        AttachmentDTO attachmentDTO,
        AttachmentTypeConverter attachmentTypeConverter
    )
        : base(attachmentDTO.Name!, attachmentTypeConverter, new SolidColorBrush(Colors.LimeGreen))
    {
        EndSlotName = attachmentDTO.AttachmentData?.End;
        VertexCount = attachmentDTO.AttachmentData?.Vertices?.Count / 2 ?? 0;

        var xyArray = attachmentDTO.AttachmentData?.Vertices;
        if (xyArray is not null)
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
        var bone = Slot?.BoundedBone;
        if (bone is null)
        {
            return new List<Point>();
        }

        List<Point> result = new List<Point>();

        foreach (var p in Vertices)
        {
            result.Add(new Point(bone.X + p.X, bone.Y + p.Y));
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
        var drawVertices = GetAvaloniaPointsList();
        var bone = Slot?.BoundedBone;
        if (bone is null)
            return;

        if (drawVertices == null || drawVertices.Count == 0)
            return;

        for (int i = 0; i < drawVertices.Count - 1; i++)
        {
            var p1 = drawVertices[i];
            var p2 = drawVertices[i + 1];

            context.DrawLine(ClipOutlinePen, new Point(p1.X, p1.Y), new Point(p2.X, p2.Y));
        }

        if (drawVertices.Count > 1)
        {
            var first = drawVertices[0];
            var last = drawVertices[drawVertices.Count - 1];

            context.DrawLine(
                ClipOutlinePen,
                new Point(last.X, last.Y),
                new Point(first.X, first.Y)
            );
        }

        foreach (Point vertice in drawVertices)
        {
            var color = Brushes.Red;
            if (
                vertice.X == bone.X + SelectedVertice?.X
                && vertice.Y == bone.Y + SelectedVertice?.Y
            )
            {
                color = Brushes.LimeGreen;
            }
            context.DrawEllipse(color, null, vertice, 2.5, 2.5);
        }
    }
}
