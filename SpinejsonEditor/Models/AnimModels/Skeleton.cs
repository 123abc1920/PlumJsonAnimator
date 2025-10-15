using System;
using System.Collections.Generic;
using Constants;
using SpinejsonEditor.ViewModels;

namespace AnimModels
{
    public class Skeleton
    {
        public string name = "default";
        public List<Bone> bones = new List<Bone>();
        public List<Skin> skins = new List<Skin>();
        public List<Slot> slots = new List<Slot>();

        public Skeleton()
        {
            bones.Add(new Bone());
        }

        public void addBone()
        {
            this.bones.Add(new Bone(bones.Count));
        }
    }
}
