using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    public class Skeleton : INotifyable
    {
        public string name = "default";
        public ObservableCollection<Bone> Bones { get; set; } = new ObservableCollection<Bone>();
        public ObservableCollection<Bone> RootBones { get; set; } =
            new ObservableCollection<Bone>();

        private int ids = 0;

        private GlobalState globalState;

        public Skeleton(GlobalState globalState)
        {
            var root = new Bone(globalState);
            Bones.Add(root);
            RootBones.Add(root);

            ids++;

            this.globalState = globalState;
        }

        public void addBone(int id)
        {
            Bone new_bone = new Bone(this.globalState, ids);
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

        public void addBone(Bone b)
        {
            this.Bones.Add(b);
            b.id = ids;
            ids++;
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

        public Bone? getBone(string? name)
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
            return JsonConvert.SerializeObject(generateJSONData(), this.globalState.jsonSettings);
        }
    }

    public class SkeletonData
    {
        [JsonProperty("bones")]
        public List<BoneData> Bones { get; set; } = new List<BoneData>();
    }
}
