using System.Collections.Generic;
using Avalonia.Controls;

namespace AnimModels
{
    public class Skin
    {
        public string Name { get; set; } = "defualt";
        public Dictionary<Bone, Slot> BoneSlotBinding = new Dictionary<Bone, Slot>();

        public Skin() { }

        public Skin(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Binds a bone with a slot
        /// </summary>
        /// <param name="b"></param>
        /// <param name="s"></param>
        public void BindBoneAndSlot(Bone b, Slot s)
        {
            if (BoneSlotBinding.ContainsKey(b))
            {
                BoneSlotBinding[b] = s;
            }
            else
            {
                BoneSlotBinding.Add(b, s);
            }
            b.Slot = s;
            s.BoundedBone = b;
        }

        /// <summary>
        /// Unbinds a bone and a slot
        /// </summary>
        /// <param name="b"></param>
        public void UnbindBoneAndSlot(Bone b)
        {
            if (b == null)
            {
                return;
            }

            if (b.Slot != null)
            {
                b.Slot.BoundedBone = null;
            }
            b.Slot = null;
            BoneSlotBinding.Remove(b);
        }

        /// <summary>
        /// Returns bone slot in this skin
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public Slot? GetSlot(Bone bone)
        {
            if (BoneSlotBinding.ContainsKey(bone))
            {
                return BoneSlotBinding[bone];
            }
            return null;
        }

        /// <summary>
        /// Must be called before skin deleting. Deletes bone slots
        /// </summary>
        public void DeleteSkin()
        {
            foreach (Bone b in BoneSlotBinding.Keys)
            {
                if (b.Slot != null)
                {
                    b.Slot.BoundedBone = null;
                }
                b.Slot = null;
            }
        }

        /// <summary>
        /// Draws skin
        /// </summary>
        /// <param name="canvas"></param>
        public void DrawSkin(Canvas canvas)
        {
            foreach (Slot s in BoneSlotBinding.Values)
            {
                s.drawSlot(canvas);
            }
        }
    }
}
