using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Constants;
using Newtonsoft.Json;
using SpinejsonEditor.ViewModels;

namespace AnimModels
{
    public class Skeleton
    {
        public string name = "default";
        public List<Bone> bones = new List<Bone>();
        private List<Skin> skins = new List<Skin>();

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

        public SkeletonData generateJSONData()
        {
            var skeletonData = new SkeletonData();

            var boneList = new List<BoneData>();
            foreach (var bone in this.bones)
            {
                boneList.Add(bone.generateJSONData());
            }
            skeletonData.Bones = boneList;

            var slotsList = new List<SlotData>();
            foreach (var bone in this.bones)
            {
                if (bone.slot != null)
                {
                    slotsList.Add(bone.slot.generateJSONData());
                }
            }
            skeletonData.Slots = slotsList;

            return skeletonData;
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), ConstantsClass.jsonSettings);
        }
    }
}

public class SkeletonData
{
    [JsonProperty("bones")]
    public List<BoneData> Bones { get; set; } = new List<BoneData>();

    [JsonProperty("slots")]
    public List<SlotData> Slots { get; set; } = new List<SlotData>();

    /*[JsonProperty("skins", NullValueHandling = NullValueHandling.Ignore)]
    public List<SkinData> Skins { get; set; } = new List<SkinData>();*/
}
