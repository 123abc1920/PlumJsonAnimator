using System;
using System.Collections.Generic;
using System.ComponentModel;
using EngineModels;
using Newtonsoft.Json;
using Prettify;

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
            List<AnimationData> animations = new List<AnimationData>();
            for (int i = 0; i < project.animations.Count; i++)
            {
                animations.Add(project.animations[i].generateJSONData());
            }

            return new CodeData
            {
                Skeleton = new MetaData { Spine = "4.2.22" },
                SkeletonData = project.mainSkeleton.generateJSONData(),
                Animations = animations,
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
    public List<BoneData> Bones => SkeletonData?.Bones;

    [JsonProperty("slots")]
    public List<SlotData> Slots => SkeletonData?.Slots;

    [JsonProperty("animations", NullValueHandling = NullValueHandling.Ignore)]
    public List<AnimationData> Animations { get; set; }
}

public class MetaData
{
    [JsonProperty("spine")]
    public string Spine { get; set; }
}
