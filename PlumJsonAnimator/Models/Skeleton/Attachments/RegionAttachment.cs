using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Resources;

// TODO: fix SetSize GetSize Size
namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

/// <summary>
/// Binds slot and res
/// </summary>
public class RegionAttachment : Attachment
{
    public override AttachmentTypes AttachmentType => AttachmentTypes.REGION;

    private ImageRes _imageRes;
    private Bitmap? _cachedBitmap;
    private string? _cachedPath;

    public double X { get; set; }
    public double Y { get; set; }
    public double A { get; set; }

    protected int? Width { get; set; } = null;
    protected int? Height { get; set; } = null;

    public RegionAttachment(
        AttachmentDTO attachmentDTO,
        AttachmentTypeConverter attachmentTypeConverter
    )
        : base(attachmentDTO.Name, attachmentTypeConverter)
    {
        _imageRes = (ImageRes)attachmentDTO.Res;

        var data = attachmentDTO.AttachmentData;

        if (!(data is null))
        {
            X = data.X;
            Y = data.Y;
            A = data.A;

            Width = data.Width;
            Height = data.Height;
        }
    }

    public static Attachment Create(
        AttachmentTypeConverter attachmentTypeConverter,
        AttachmentDTO attachmentDTO
    )
    {
        return new RegionAttachment(attachmentDTO, attachmentTypeConverter);
    }

    public string GetPath()
    {
        return _imageRes.Path;
    }

    public override AttachmentData GenerateJSONData()
    {
        return new AttachmentData
        {
            Name = Name,
            Width = Width,
            Height = Height,
            X = X,
            Y = Y,
            A = A,
            Type = _attachmentTypeConverter.GetAttachmentTypeString(AttachmentType),
        };
    }

    public Res GetRes()
    {
        return _imageRes;
    }

    public override void SetPos(double x, double y, double a)
    {
        X = x;
        Y = y;
        A = a;
    }

    public override void SetSize(double width, double height)
    {
        Width = (int)width;
        Height = (int)height;
    }

    public Dictionary<string, int?> GetSize()
    {
        return new Dictionary<string, int?>() { ["width"] = Width, ["height"] = Height };
    }

    public override void DrawSlotAttachment(
        Slot slot,
        Canvas? canvas,
        ClipAttachment? clipAttachment = null
    )
    {
        if (canvas == null)
            return;

        string currentPath = GetPath();
        if (_cachedBitmap == null || _cachedPath != currentPath)
        {
            _cachedPath = currentPath;
            byte[] imageBytes = File.ReadAllBytes(currentPath);
            using var ms = new MemoryStream(imageBytes);
            _cachedBitmap?.Dispose();
            _cachedBitmap = new Bitmap(ms);
        }

        if (slot.BoundedBone is null)
            return;

        double boneM11 = slot.BoundedBone.G11;
        double boneM12 = slot.BoundedBone.G12;
        double boneM21 = slot.BoundedBone.G21;
        double boneM22 = slot.BoundedBone.G22;

        double attachAngleRad = A * Math.PI / 180.0;
        double ac = Math.Cos(attachAngleRad);
        double asin = Math.Sin(attachAngleRad);

        double f11 = boneM11 * ac + boneM21 * asin;
        double f12 = boneM12 * ac + boneM22 * asin;
        double f21 = -boneM11 * asin + boneM21 * ac;
        double f22 = -boneM12 * asin + boneM22 * ac;

        double imgWidth = Width ?? slot.LengthX;
        double imgHeight = Height ?? slot.LengthY;

        var image = new Image
        {
            Source = _cachedBitmap,
            Width = imgWidth,
            Height = imgHeight,
            RenderTransform = new MatrixTransform(new Matrix(f11, f12, f21, f22, 0, 0)),
            RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
        };

        var points = new List<Point>();

        if (clipAttachment is null)
        {
            points.Add(new Point(0, 0));
            points.Add(new Point(imgWidth, 0));
            points.Add(new Point(imgWidth, imgHeight));
            points.Add(new Point(0, imgHeight));
        }
        else
        {
            points = clipAttachment.GetAvaloniaPointsList();
        }

        if (points.Count < 3)
            return;

        var clipGeometry = new StreamGeometry();
        using (var ctx = clipGeometry.Open())
        {
            ctx.BeginFigure(points[0], isFilled: true);

            for (int i = 1; i < points.Count; i++)
            {
                ctx.LineTo(points[i]);
            }

            ctx.EndFigure(isClosed: true);
        }

        image.Clip = clipGeometry;

        double worldX = slot.BoundedBone.GlobalX + (X * boneM11 + Y * boneM21);
        double worldY = slot.BoundedBone.GlobalY + (X * boneM12 + Y * boneM22);

        double left = canvas.Width / 2 + worldX - image.Width / 2;
        double top = canvas.Height / 2 + worldY - image.Height / 2;

        Canvas.SetLeft(image, left);
        Canvas.SetTop(image, top);

        canvas.Children.Add(image);
    }

    /// <summary>
    /// Disposes cached bitmap
    /// </summary>
    private void Dispose()
    {
        _cachedBitmap?.Dispose();
    }
}
