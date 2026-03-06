using AnimEngine.Resources;
using Newtonsoft.Json;

namespace AnimModels
{
    public abstract class Attachment
    {
        public string Name { get; set; }

        public virtual Res GetRes()
        {
            return new Res();
        }

        public abstract AttachmentData generateJSONData();
    }

    public class ImageAttachment : Attachment
    {
        public ImageRes image;

        public ImageAttachment(ImageRes res)
        {
            this.image = res;
            this.Name = res.Name;
        }

        public string getPath()
        {
            return this.image.path;
        }

        public override AttachmentData generateJSONData()
        {
            return new AttachmentData
            {
                Name = this.image.Name,
                Width = this.image.width,
                Height = this.image.height,
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
        public string Name { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int Width { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int Height { get; set; }
    }
}
