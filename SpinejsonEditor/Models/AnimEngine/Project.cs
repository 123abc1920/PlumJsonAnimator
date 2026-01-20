using System.Collections.ObjectModel;
using AnimModels;
using Avalonia.Controls;
using Newtonsoft.Json;
using SpinejsonGeneration;
using TransformModes;

namespace EngineModels
{
    public partial class Project
    {
        public string ProjectPath { get; set; } = "C:/Users/Документы/";
        public string Name { get; set; } = "NewProject";

        private MetaData metaData = new MetaData { Spine = "4.2.22" };

        public Skeleton? MainSkeleton { get; set; } = null;
        public Mode currentMode = new NoMode();
        public int seletedBoneId = -1;
        public ObservableCollection<Slot> Slots { get; set; } = new ObservableCollection<Slot>();
        public SpinejsonCode SpinejsonCode { get; set; } = new SpinejsonCode();

        public ObservableCollection<Animation> animations = new ObservableCollection<Animation>();
        public int currentAnimation = 0;

        public Project()
        {
            MainSkeleton = new Skeleton();
            animations.Add(new Animation());
        }

        public Animation GetAnimation()
        {
            return animations[currentAnimation];
        }

        public void addAnimation()
        {
            this.animations.Add(new Animation("anim" + animations.Count.ToString()));
            currentAnimation = this.animations.Count - 1;
        }

        public IBone? GetSlot(int id)
        {
            foreach (Slot s in Slots)
            {
                if (id == s.id)
                {
                    return s;
                }
            }

            return null;
        }

        public void drawSlots(Canvas c)
        {
            foreach (Slot s in Slots)
            {
                s.drawSlot(c);
            }
        }

        public MetaData gemerateMetaData()
        {
            return this.metaData;
        }
    }
}

public class MetaData
{
    [JsonProperty("spine")]
    public string Spine { get; set; }
}
