using System.Collections.Generic;
using Newtonsoft.Json;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;

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
