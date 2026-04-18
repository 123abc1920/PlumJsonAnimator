using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    /// <summary>
    /// Provides methods for work with skeleton
    /// </summary>
    public class Skeleton : INotifyable
    {
        public ObservableCollection<Bone> Bones { get; set; } = new ObservableCollection<Bone>();
        public ObservableCollection<Bone> RootBones { get; set; } =
            new ObservableCollection<Bone>();

        private int _last_bone_id = 0;
        private GlobalState _globalState;

        public Skeleton(GlobalState globalState)
        {
            var root = new Bone(globalState);
            Bones.Add(root);
            RootBones.Add(root);

            _last_bone_id++;

            this._globalState = globalState;
        }

        /// <summary>
        /// Adds new bone into skeleton. Binds it with existing parent bone
        /// </summary>
        /// <param name="parentId">Parent bone id</param>
        public void AddBoneToParent(int parentId)
        {
            Bone new_bone = new Bone(this._globalState, _last_bone_id);
            this.Bones.Add(new_bone);
            foreach (Bone b in this.Bones)
            {
                if (b.id == parentId)
                {
                    b.AddChildren(new_bone);
                }
            }
            _last_bone_id++;
        }

        /// <summary>
        /// Adds new bone into skeleton
        /// </summary>
        /// <param name="b">New bone</param>
        public void AddBone(Bone b)
        {
            this.Bones.Add(b);
            b.id = _last_bone_id;
            _last_bone_id++;
        }

        public Bone? GetBoneById(int id)
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

        public Bone? GetBoneByName(string? name)
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

        public void DrawSkeleton(Canvas canvas)
        {
            foreach (Bone b in this.Bones)
            {
                b.DrawBone(canvas);
            }
        }

        public List<BoneData> GenerateJSONData()
        {
            List<BoneData> result = new List<BoneData>();

            foreach (Bone bone in Bones)
            {
                result.Add(bone.GenerateJSONData());
            }

            return result;
        }

        public String GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }
    }

    /// <summary>
    /// Skeleton JSON data
    /// </summary>
    public class SkeletonData
    {
        [JsonProperty("bones")]
        public List<BoneData> Bones { get; set; } = new List<BoneData>();
    }
}
