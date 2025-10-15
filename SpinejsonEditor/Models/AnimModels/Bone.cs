using System.Collections.Generic;

namespace AnimModels
{
    public class Bone
    {
        public string name = "root";
        public int id = 0;

        public Bone() { }

        public Bone(int _id)
        {
            this.id = _id;
            this.name = "name" + this.id.ToString();
        }
    }
}
