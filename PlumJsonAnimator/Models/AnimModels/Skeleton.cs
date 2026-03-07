using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Common.Constants;
using Newtonsoft.Json;

namespace AnimModels
{
    public class Skeleton
    {
        public string name = "default";
        public ObservableCollection<Bone> Bones { get; set; } = new ObservableCollection<Bone>();
        public ObservableCollection<Bone> RootBones { get; set; } =
            new ObservableCollection<Bone>();

        private int ids = 0;

        public Skeleton()
        {
            Bones.Add(new Bone());
            RootBones.Add(Bones[0]);
        }

        public void addBone(int id)
        {
            Bone new_bone = new Bone(Bones.Count);
            this.Bones.Add(new_bone);
            foreach (Bone b in this.Bones)
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
            foreach (Bone b in this.Bones)
            {
                if (b.id == id)
                {
                    return b;
                }
            }
            return null;
        }

        public Bone? getBone(string name)
        {
            foreach (Bone b in this.Bones)
            {
                if (b.Name == name)
                {
                    return b;
                }
            }
            return null;
        }

        public void drawSkeleton(Canvas canvas)
        {
            foreach (Bone b in this.Bones)
            {
                b.drawBone(canvas);
            }
        }

        public List<BoneData> generateJSONData()
        {
            List<BoneData> result = new List<BoneData>();

            foreach (Bone bone in Bones)
            {
                result.Add(bone.generateJSONData());
            }

            return result;
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), ConstantsClass.jsonSettings);
        }

        public void regenerateBones(List<BoneData> boneDatas) { }
    }
}

public class SkeletonData
{
    [JsonProperty("bones")]
    public List<BoneData> Bones { get; set; } = new List<BoneData>();
}
