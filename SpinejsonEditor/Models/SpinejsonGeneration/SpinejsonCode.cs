using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AnimModels;
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

            var currBones = Constants.ConstantsClass.currentProject.MainSkeleton.Bones;

            var currentBonesDict = currBones.ToDictionary(b => b.Name, b => b);
            var newBonesDict = newData.Bones.ToDictionary(b => b.Name, b => b);

            for (int i = currBones.Count - 1; i >= 0; i--)
            {
                var bone = currBones[i];
                if (newBonesDict.ContainsKey(bone.Name))
                {
                    var oldjsonObj = bone.generateJSONData();
                    if (newBonesDict.TryGetValue(bone.Name, out var newjsonObj))
                    {
                        if (oldjsonObj.ToString() != newjsonObj.ToString())
                        {
                            currBones.RemoveAt(i);
                            /*currBones.Add(
                                new Bone(
                                    currBones.Count,
                                    currBones[0],
                                    newjsonObj.Name,
                                    newjsonObj.X,
                                    newjsonObj.Y
                                )
                            );*/
                        }
                    }
                }
                else
                {
                    currBones.RemoveAt(i);
                }
            }
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
