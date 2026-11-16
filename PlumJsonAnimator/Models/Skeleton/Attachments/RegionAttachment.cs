using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Views;

// TODO: fix SetSize GetSize Size
namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

/// <summary>
/// Binds slot and res
/// </summary>
public class RegionAttachment : Attachment
{
    public override AttachmentTypes AttachmentType => AttachmentTypes.REGION;

    public ImageRes? ImageRes;
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
        ImageRes = (ImageRes)attachmentDTO.Res;

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

    public string? GetPath()
    {
        if (ImageRes is null)
            return null;

        return ImageRes.Path;
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
        return ImageRes;
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

    public override void DrawAttachment(
        Slot slot,
        DrawingContext? context,
        ClipAttachment? clipAttachment = null
    )
    {
        if (context == null)
            return;

        string? currentPath = GetPath();
        if (currentPath is null)
            return;

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

        Rect destRect = new Rect(0, 0, imgWidth, imgHeight);
        if (
            _cachedBitmap.Size.Width > 0
            && _cachedBitmap.Size.Height > 0
            && imgWidth > 0
            && imgHeight > 0
        )
        {
            double scale = Math.Min(
                imgWidth / _cachedBitmap.Size.Width,
                imgHeight / _cachedBitmap.Size.Height
            );
            double finalW = _cachedBitmap.Size.Width * scale;
            double finalH = _cachedBitmap.Size.Height * scale;
            double offsetX = (imgWidth - finalW) / 2.0;
            double offsetY = (imgHeight - finalH) / 2.0;
            destRect = new Rect(offsetX, offsetY, finalW, finalH);
        }

        double worldX = slot.BoundedBone.GlobalX + (X * boneM11 + Y * boneM21);
        double worldY = slot.BoundedBone.GlobalY + (X * boneM12 + Y * boneM22);

        var transform =
            Matrix.CreateTranslation(-imgWidth / 2.0, -imgHeight / 2.0)
            * new Matrix(f11, f12, f21, f22, 0, 0)
            * Matrix.CreateTranslation(worldX, worldY);

        using (context.PushTransform(transform))
        {
            if (clipAttachment != null)
            {
                var points = clipAttachment.GetAvaloniaPointsList();
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

                using (context.PushGeometryClip(clipGeometry))
                {
                    context.DrawImage(_cachedBitmap, destRect);
                }
            }
            else
            {
                context.DrawImage(_cachedBitmap, destRect);
            }
        }
    }

    /// <summary>
    /// Disposes cached bitmap
    /// </summary>
    private void Dispose()
    {
        _cachedBitmap?.Dispose();
    }

    public override object CreateInfoPanel()
    {
        return new RegionAttachmentInfo(this);
    }

    public override void Draw(DrawingContext context) { }
}
