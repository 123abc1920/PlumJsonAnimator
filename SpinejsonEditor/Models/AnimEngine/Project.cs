using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AnimModels;
using Avalonia.Controls;
using Newtonsoft.Json;
using SpinejsonGeneration;
using TransformModes;

namespace EngineModels
{
    public partial class Project : INotifyPropertyChanged
    {
        public string ProjectPath { get; set; } = "C:/Users/Документы/";
        public string Name { get; set; } = "NewProject";

        private MetaData metaData = new MetaData { Spine = "4.2.22" };

        public Skeleton? MainSkeleton { get; set; } = null;
        public Mode currentMode = new NoMode();
        public int seletedBoneId = -1;
        public ObservableCollection<Slot> Slots { get; set; } = new ObservableCollection<Slot>();
        public SpinejsonCode SpinejsonCode { get; set; } = new SpinejsonCode();

        public ObservableCollection<Animation> Animations = new ObservableCollection<Animation>();
        public int currentAnimation = 0;

        public ObservableCollection<Skin> Skins { get; } = new ObservableCollection<Skin>();
        private Skin _currentSkin;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Skin CurrentSkin
        {
            get => _currentSkin;
            set
            {
                if (_currentSkin != value)
                {
                    _currentSkin = value;
                    OnPropertyChanged(nameof(CurrentSkin));
                    RestartBindings();
                }
            }
        }
        public int CurrentSkinIndex { get; set; } = 0;

        public Project()
        {
            MainSkeleton = new Skeleton();
            Animations.Add(new Animation());
            Skins.Add(new Skin());
            CurrentSkin = Skins[0];
        }

        public Animation GetAnimation()
        {
            return Animations[currentAnimation];
        }

        public void addAnimation()
        {
            this.Animations.Add(new Animation("anim" + Animations.Count.ToString()));
            currentAnimation = this.Animations.Count - 1;
        }

        public void AddSkin()
        {
            this.Skins.Add(new Skin("skin" + Skins.Count.ToString()));
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

        /// <summary>
        /// Rebind all bones with new skin
        /// </summary>
        public void RestartBindings()
        {
            this.MainSkeleton.RestartBindings(CurrentSkin);
        }

        public void drawSlots(Canvas c)
        {
            CurrentSkin.DrawSkin(c);
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
