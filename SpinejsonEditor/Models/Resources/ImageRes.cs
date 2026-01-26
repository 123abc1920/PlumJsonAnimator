namespace Resources
{
    /// <summary>
    /// Resources -- images, textures etc
    /// </summary>
    public class Res
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// Image resources, which are uploaded by user
    /// </summary>
    public class ImageRes : Res
    {
        public string path;
        public int width;
        public int height;

        public ImageRes() { }

        public ImageRes(string _path, string name)
        {
            this.path = _path;
            this.Name = name;
        }
    }
}
