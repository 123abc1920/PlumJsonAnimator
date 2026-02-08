using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using AnimModels;
using Constants;
using EngineModels;
using Newtonsoft.Json;

namespace SpinejsonGeneration
{
    public class ValidResult
    {
        public string Message { get; set; }
        public bool IsOk { get; set; }
        public object UpdatedArray { get; set; }
    }

    public class ProjectValidResult
    {
        public string Message { get; set; }
        public bool IsOk { get; set; }
    }

    public class SpinejsonCode : INotifyPropertyChanged
    {
        public String text = "";

        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public CodeData generateJSONData(Project project)
        {
            Dictionary<string, AnimationData> animations = new Dictionary<string, AnimationData>();
            for (int i = 0; i < project.Animations.Count; i++)
            {
                animations.Add(
                    project.Animations[i].Name,
                    project.Animations[i].generateJSONData()
                );
            }

            List<SlotData> slots = new List<SlotData>();
            foreach (Slot s in Constants.ConstantsClass.currentProject.Slots)
            {
                slots.Add(s.generateJSONData());
            }

            return new CodeData
            {
                Skeleton = Constants.ConstantsClass.currentProject.generateMetaData(),
                SkeletonData = project.MainSkeleton.generateJSONData(),
                Slots = slots,
                Animations = animations,
                Skins = Constants.ConstantsClass.currentProject.generateSkinsJSONData(),
            };
        }

        public void generateCode(Project project)
        {
            text = "";
            text = JsonConvert.SerializeObject(
                generateJSONData(project),
                Constants.ConstantsClass.jsonSettings
            );

            text = Prettify.Prettify.prettify(text);
            OnPropertyChanged(nameof(Text));
        }

        public ValidResult regenerateBones(List<BoneData> bones)
        {
            List<BoneData> newBones = bones;
            List<BoneData> oldBones = ConstantsClass
                .currentProject.MainSkeleton.generateJSONData()
                .Bones;
            Dictionary<string, BoneData> updatedBones = SpinejsonModel.regenerateBones(
                newBones.ToDictionary(b => b.Name, b => b),
                oldBones.ToDictionary(b => b.Name, b => b)
            );

            int rootBonesCount = 0;
            foreach (BoneData b in updatedBones.Values)
            {
                if (b.Parent == null)
                {
                    rootBonesCount++;
                }
            }

            if (rootBonesCount > 1)
            {
                return new ValidResult
                {
                    Message = "Ошибка: Несколько root костей!",
                    IsOk = false,
                    UpdatedArray = null,
                };
            }
            else if (rootBonesCount < 1)
            {
                return new ValidResult
                {
                    Message = "Ошибка: Нет root кости!",
                    IsOk = false,
                    UpdatedArray = null,
                };
            }

            return new ValidResult
            {
                Message = "",
                IsOk = true,
                UpdatedArray = updatedBones,
            };
        }

        public ValidResult regenerateSlots(List<SlotData> slots)
        {
            List<SlotData> newSlots = slots;
            List<SlotData> oldSlots = ConstantsClass.currentProject.generateSlotsJSONData();
            Dictionary<string, SlotData> updatedSlots = SpinejsonModel.regenerateSlots(
                newSlots.ToDictionary(b => b.Name, b => b),
                oldSlots.ToDictionary(b => b.Name, b => b)
            );

            return new ValidResult
            {
                Message = "",
                IsOk = true,
                UpdatedArray = updatedSlots,
            };
        }

        public ValidResult regenerateSkins(List<SkinData> skins)
        {
            List<SkinData> newSkins = skins;
            List<SkinData> oldSkins = ConstantsClass.currentProject.generateSkinsJSONData();
            Dictionary<string, SkinData> updatedSkins = SpinejsonModel.regenerateSkins(
                newSkins.ToDictionary(b => b.Name, b => b),
                oldSkins.ToDictionary(b => b.Name, b => b)
            );

            return new ValidResult
            {
                Message = "",
                IsOk = true,
                UpdatedArray = updatedSkins,
            };
        }

        public ValidResult regenerateAnimations(Dictionary<string, AnimationData> animations)
        {
            Dictionary<string, AnimationData> newAnimations = animations;
            Dictionary<string, AnimationData> oldAnimations =
                ConstantsClass.currentProject.generateAnimationsJSONData();
            Dictionary<string, AnimationData> updatedAnimations =
                SpinejsonModel.regenerateAnimations(newAnimations, oldAnimations);

            return new ValidResult
            {
                Message = "",
                IsOk = true,
                UpdatedArray = updatedAnimations,
            };
        }

        public ProjectValidResult regenerate()
        {
            CodeData newData = JsonConvert.DeserializeObject<CodeData>(text);
            if (newData == null)
            {
                return new ProjectValidResult { Message = "", IsOk = true };
            }

            var boneResult = regenerateBones(newData.Bones);
            if (!boneResult.IsOk)
            {
                return new ProjectValidResult { Message = boneResult.Message, IsOk = false };
            }

            var slotResult = regenerateSlots(newData.Slots);
            if (!slotResult.IsOk)
            {
                return new ProjectValidResult { Message = slotResult.Message, IsOk = false };
            }

            var skinResult = regenerateSkins(newData.Skins);
            if (!skinResult.IsOk)
            {
                return new ProjectValidResult { Message = skinResult.Message, IsOk = false };
            }

            var animationResult = regenerateAnimations(newData.Animations);
            if (!animationResult.IsOk)
            {
                return new ProjectValidResult { Message = animationResult.Message, IsOk = false };
            }

            ConstantsClass.currentProject.regenrateProject(
                (Dictionary<string, BoneData>)boneResult.UpdatedArray,
                (Dictionary<string, SlotData>)slotResult.UpdatedArray,
                (Dictionary<string, SkinData>)skinResult.UpdatedArray,
                (Dictionary<string, AnimationData>)animationResult.UpdatedArray
            );

            return new ProjectValidResult { Message = "", IsOk = true };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

public class CodeData
{
    [JsonProperty("skeleton")]
    public MetaData Skeleton { get; set; }

    [JsonIgnore]
    public SkeletonData SkeletonData { get; set; }

    [JsonProperty("bones")]
    public List<BoneData> Bones
    {
        get => SkeletonData?.Bones;
        set
        {
            if (SkeletonData == null)
                SkeletonData = new SkeletonData();
            SkeletonData.Bones = value ?? new List<BoneData>();
        }
    }

    [JsonProperty("slots")]
    public List<SlotData> Slots { get; set; }

    [JsonProperty("skins", NullValueHandling = NullValueHandling.Ignore)]
    public List<SkinData> Skins { get; set; }

    [JsonProperty("animations", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, AnimationData> Animations { get; set; }
}
