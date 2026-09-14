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
            if (canvas == null)
                return;

            string currentPath = this.GetPath();
            if (_cachedBitmap == null || _cachedPath != currentPath)
            {
                _cachedPath = currentPath;
                byte[] imageBytes = File.ReadAllBytes(currentPath);
                using var ms = new MemoryStream(imageBytes);
                _cachedBitmap?.Dispose();
                _cachedBitmap = new Bitmap(ms);
            }

            // 1. Получаем матрицу кости (уже вычисленную в DrawBone)
            double boneM11 = slot.BoundedBone.G11;
            double boneM12 = slot.BoundedBone.G12;
            double boneM21 = slot.BoundedBone.G21;
            double boneM22 = slot.BoundedBone.G22;

            // 2. Локальный поворот аттачмента
            double attachAngleRad = this.a * Math.PI / 180.0;
            double ac = Math.Cos(attachAngleRad);
            double asin = Math.Sin(attachAngleRad);

            // 3. Комбинируем матрицы
            double f11 = boneM11 * ac + boneM21 * asin;
            double f12 = boneM12 * ac + boneM22 * asin;
            double f21 = -boneM11 * asin + boneM21 * ac;
            double f22 = -boneM12 * asin + boneM22 * ac;

            // 4. Размеры
            double imgWidth = this._width ?? slot.LengthX;
            double imgHeight = this._height ?? slot.LengthY;

            var image = new Image
            {
                Source = _cachedBitmap,
                Width = imgWidth,
                Height = imgHeight,
                RenderTransform = new MatrixTransform(new Matrix(f11, f12, f21, f22, 0, 0)),
                RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
            };

            // 5. Мировые координаты
            double worldX = slot.BoundedBone.GlobalX + (this.x * boneM11 + this.y * boneM21);
            double worldY = slot.BoundedBone.GlobalY + (this.x * boneM12 + this.y * boneM22);

            // 6. Позиционирование
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
