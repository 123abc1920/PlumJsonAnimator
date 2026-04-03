using System.Collections.Generic;
using Newtonsoft.Json;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    public abstract class Attachment : INotifyable
    {
        public string? Name { get; set; }

        public double x;
        public double y;
        public double a;

        protected int? width = null;
        protected int? height = null;

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

        public abstract AttachmentData generateJSONData();

        public void setSize(double width, double height)
        {
            this.width = (int)width;
            this.height = (int)height;
        }

        public Dictionary<string, int?> getSize()
        {
            return new Dictionary<string, int?>()
            {
                ["width"] = this.width,
                ["height"] = this.height,
            };
        }
    }

    public class ImageAttachment : Attachment
    {
        private ImageRes image;

        public ImageAttachment(ImageRes res)
        {
            this.image = res;
            this.Name = res.Name;
        }

        public ImageAttachment(ImageRes res, AttachmentData data)
        {
            this.image = res;
            this.Name = res.Name;

            this.x = data.X;
            this.y = data.Y;
            this.a = data.A;

            this.width = data.Width;
            this.height = data.Height;
        }

        public string getPath()
        {
            return this.image.Path;
        }

        public override AttachmentData generateJSONData()
        {
            return new AttachmentData
            {
                Name = this.image.Name,
                Width = this.width,
                Height = this.height,
                X = this.x,
                Y = this.y,
                A = this.a,
            };
        }

        public override Res GetRes()
        {
            return this.image;
        }
    }

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
