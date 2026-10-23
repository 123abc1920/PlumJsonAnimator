using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using PlumJsonAnimator.Models.Resources;

namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

/// <summary>
/// Binds slot and res
/// </summary>
public class ImageAttachment : Attachment
{
    private const string TypeStr = "region";

    private ImageRes _image;
    private Bitmap? _cachedBitmap;
    private string? _cachedPath;

    public ImageAttachment(ImageRes res)
        : base(res.Name, TypeStr)
    {
        _image = res;
    }

    public ImageAttachment(ImageRes res, AttachmentData data)
        : this(res)
    {
        _x = data.X;
        _y = data.Y;
        _a = data.A;

        _width = data.Width;
        _height = data.Height;
    }

    public string GetPath()
    {
        return _image.Path;
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

    public override Res GetRes()
    {
        return _image;
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

        double attachAngleRad = _a * Math.PI / 180.0;
        double ac = Math.Cos(attachAngleRad);
        double asin = Math.Sin(attachAngleRad);

        double f11 = boneM11 * ac + boneM21 * asin;
        double f12 = boneM12 * ac + boneM22 * asin;
        double f21 = -boneM11 * asin + boneM21 * ac;
        double f22 = -boneM12 * asin + boneM22 * ac;

        double imgWidth = _width ?? slot.LengthX;
        double imgHeight = _height ?? slot.LengthY;

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

        double worldX = slot.BoundedBone.GlobalX + (_x * boneM11 + _y * boneM21);
        double worldY = slot.BoundedBone.GlobalY + (_x * boneM12 + _y * boneM22);

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
