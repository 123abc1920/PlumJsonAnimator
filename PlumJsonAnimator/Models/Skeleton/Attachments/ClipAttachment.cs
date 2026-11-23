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
        if (bone is null || Vertices == null)
        {
            return new List<Point>();
        }

        var result = new List<Point>(Vertices.Count);

        foreach (var p in Vertices)
        {
            double gx = bone.GlobalX + (p.X * bone.G11) + (p.Y * bone.G21);
            double gy = bone.GlobalY + (p.X * bone.G12) + (p.Y * bone.G22);
            result.Add(new Point(gx, gy));
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
        var globalVertices = GetAvaloniaPointsList();
        if (globalVertices == null || globalVertices.Count == 0)
            return;

        for (int i = 0; i < globalVertices.Count - 1; i++)
        {
            context.DrawLine(ClipOutlinePen, globalVertices[i], globalVertices[i + 1]);
        }

        if (globalVertices.Count > 1)
        {
            context.DrawLine(ClipOutlinePen, globalVertices[^1], globalVertices[0]);
        }

        for (int i = 0; i < globalVertices.Count; i++)
        {
            var global = globalVertices[i];
            var rawLocal = Vertices[i];

            var color = Brushes.Red;
            if (rawLocal.X == SelectedVertice?.X && rawLocal.Y == SelectedVertice?.Y)
            {
                color = Brushes.LimeGreen;
            }

            context.DrawEllipse(color, null, global, 2.5, 2.5);
        }
    }
}
