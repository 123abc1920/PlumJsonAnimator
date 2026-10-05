using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Services;

// TODO: fix SetSize GetSize Size
namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    /// <summary>
    /// Attachment class. Helps to bind slot and res or another functions
    /// </summary>
    public abstract class Attachment : INotifyable
    {
        public string? Name { get; set; }

        public double _x;
        public double _y;
        public double _a;

        protected int? _width = null;
        protected int? _height = null;

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
            return new Dictionary<string, int?>()
            {
                ["width"] = this._width,
                ["height"] = this._height,
            };
        }

        public abstract void DrawAttachment(Slot slot, Canvas? canvas = null);
    }

    /// <summary>
    /// Binds slot and res
    /// </summary>
    public class ImageAttachment : Attachment
    {
        private ImageRes _image;

        public ImageAttachment(ImageRes res)
        {
            _image = res;
            Name = res.Name;
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
                Name = _image.Name,
                Width = _width,
                Height = _height,
                X = _x,
                Y = _y,
                A = _a,
            };
        }

        public override Res GetRes()
        {
            return _image;
        }

        private Bitmap _cachedBitmap;
        private string _cachedPath;

        public override void DrawAttachment(Slot slot, Canvas? canvas)
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

    /// <summary>
    /// Jsonifyed attachment data
    /// </summary>
    public class AttachmentData
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int? Width { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public double X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public double Y { get; set; }

        [JsonProperty("a", NullValueHandling = NullValueHandling.Ignore)]
        public double A { get; set; }
    }
}
