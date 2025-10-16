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

        public Skeleton()
        {
            bones.Add(new Bone());
        }

        public void addBone()
        {
            this.bones.Add(new Bone(bones.Count));
        }

        public string getLast()
        {
            return "bones1";
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
