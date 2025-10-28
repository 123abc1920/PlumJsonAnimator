using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Constants;
using SpinejsonEditor.ViewModels;

namespace AnimModels
{
    public class Skeleton
    {
        public string name = "default";
        private List<Bone> bones = new List<Bone>();
        private List<Skin> skins = new List<Skin>();
        private List<Slot> slots = new List<Slot>();

        private int ids = 0;

        public Skeleton()
        {
            bones.Add(new Bone());
        }

        public void addBone(int id)
        {
            Bone new_bone = new Bone(bones.Count);
            this.bones.Add(new_bone);
            foreach (Bone b in this.bones)
            {
                if (b.id == id)
                {
                    b.addChildren(new_bone);
                }
            }
            ids++;
        }

        public string getLast()
        {
            return "bones1";
        }

        public int getId()
        {
            return ids;
        }

        public Bone? getBone(int id)
        {
            foreach (Bone b in this.bones)
            {
                if (b.id == id)
                {
                    return b;
                }
            }
            return null;
        }

        public void drawSkeleton(Canvas canvas)
        {
            foreach (Bone b in this.bones)
            {
                b.drawBone(canvas);
            }
        }
    }
}
