using System.IO;

namespace Resources
{
    /// <summary>
    /// Resources -- images, textures etc
    /// </summary>
    public class Res
    {
        public string Name { get; set; }
        public string path;
        public string ext;
    }

    /// <summary>
    /// Image resources, which are uploaded by user
    /// </summary>
    public class ImageRes : Res
    {
        public int width;
        public int height;

        public ImageRes() { }

        public ImageRes(string _path, string name, string _ext)
        {
            this.Name = name;
            this.ext = _ext;
            this.path = Path.Combine(_path, $"{this.Name}{this.ext}");
        }
    }
}
