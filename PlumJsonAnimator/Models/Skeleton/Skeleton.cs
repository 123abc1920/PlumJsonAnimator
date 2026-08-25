using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

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
        private LocalizationService _localizationService;

        public Skeleton(GlobalState globalState, LocalizationService localizationService)
        {
            var root = new Bone(globalState, localizationService);
            Bones.Add(root);
            RootBones.Add(root);

            _last_bone_id++;

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Adds new bone into skeleton. Binds it with existing parent bone
        /// </summary>
        /// <param name="parentId">Parent bone id</param>
        public Bone AddBoneToParent(int parentId)
        {
            Bone parentBone = this.RootBones[0];
            foreach (Bone b in this.Bones)
            {
                if (b.id == parentId)
                {
                    parentBone = b;
                    break;
                }
            }
            Bone newBone = new Bone(
                this._globalState,
                parentBone,
                _last_bone_id,
                this._localizationService
            );
            this.Bones.Add(newBone);
            parentBone.AddChildren(newBone);
            _last_bone_id++;

            return newBone;
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
            RootBones[0].DrawBone(canvas);
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
