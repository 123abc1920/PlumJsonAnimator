using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AnimModels;
using Constants;
using EngineModels;
using Newtonsoft.Json;

namespace SpinejsonGeneration
{
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

        public void regenerate()
        {
            CodeData newData = JsonConvert.DeserializeObject<CodeData>(text);
            if (newData == null)
            {
                return;
            }

            List<BoneData> newBones = newData.Bones;
            List<BoneData> oldBones = ConstantsClass
                .currentProject.MainSkeleton.generateJSONData()
                .Bones;
            Dictionary<string, BoneData> updatedBones = SpinejsonModel.regenerateBones(
                newBones.ToDictionary(b => b.Name, b => b),
                oldBones.ToDictionary(b => b.Name, b => b)
            );

            List<SlotData> newSlots = newData.Slots;
            List<SlotData> oldSlots = ConstantsClass.currentProject.generateSlotsJSONData();
            Dictionary<string, SlotData> updatedSlots = SpinejsonModel.regenerateSlots(
                newSlots.ToDictionary(b => b.Name, b => b),
                oldSlots.ToDictionary(b => b.Name, b => b)
            );

            List<SkinData> newSkins = newData.Skins;
            List<SkinData> oldSkins = ConstantsClass.currentProject.generateSkinsJSONData();
            Dictionary<string, SkinData> updatedSkins = SpinejsonModel.regenerateSkins(
                newSkins.ToDictionary(b => b.Name, b => b),
                oldSkins.ToDictionary(b => b.Name, b => b)
            );

            Dictionary<string, AnimationData> newAnimations = newData.Animations;
            Dictionary<string, AnimationData> oldAnimations =
                ConstantsClass.currentProject.generateAnimationsJSONData();
            Dictionary<string, AnimationData> updatedAnimations =
                SpinejsonModel.regenerateAnimations(newAnimations, oldAnimations);

            ConstantsClass.currentProject.regenrateProject(
                updatedBones,
                updatedSlots,
                updatedSkins,
                updatedAnimations
            );
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
