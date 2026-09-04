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

        public double x;
        public double y;
        public double a;

        protected int? _width = null;
        protected int? _height = null;

        public void SetPos(double x, double y, double a)
        {
            this.x = x;
            this.y = y;
            this.a = a;
        }

        public virtual Res? GetRes()
        {
            return null;
        }

        public abstract AttachmentData GenerateJSONData();

        public void SetSize(double width, double height)
        {
            this._width = (int)width;
            this._height = (int)height;
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
            this._image = res;
            this.Name = res.Name;
        }

        public ImageAttachment(ImageRes res, AttachmentData data)
        {
            this._image = res;
            this.Name = res.Name;

            this.x = data.X;
            this.y = data.Y;
            this.a = data.A;

            this._width = data.Width;
            this._height = data.Height;
        }

        public string GetPath()
        {
            return this._image.Path;
        }

        public override AttachmentData GenerateJSONData()
        {
            return new AttachmentData
            {
                Name = this._image.Name,
                Width = this._width,
                Height = this._height,
                X = this.x,
                Y = this.y,
                A = this.a,
            };
        }

        public override Res GetRes()
        {
            return this._image;
        }

        private Bitmap _cachedBitmap;
        private string _cachedPath;

        public override void DrawAttachment(Slot slot, Canvas? canvas)
        {
            string currentPath = this.GetPath();
            if (_cachedBitmap == null || _cachedPath != currentPath)
            {
                _cachedPath = currentPath;
                byte[] imageBytes = File.ReadAllBytes(currentPath);
                using var ms = new MemoryStream(imageBytes);
                _cachedBitmap?.Dispose();
                _cachedBitmap = new Bitmap(ms);
            }

            var image = new Image
            {
                Source = _cachedBitmap,
                Width = slot.LengthX,
                Height = slot.LengthY,
                RenderTransform = new RotateTransform(slot.GlobalA),
                RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
            };

            double left = canvas.Width / 2 + slot.GlobalX - image.Width / 2;
            double top = canvas.Height / 2 + slot.GlobalY - image.Height / 2;

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
