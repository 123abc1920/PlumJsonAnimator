using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using AnimModels;
using AnimTransformations;
using Avalonia.Controls;
using Newtonsoft.Json;
using Resources;
using SpinejsonGeneration;
using Tmds.DBus.Protocol;
using TransformModes;

namespace EngineModels
{
    public partial class Project : INotifyPropertyChanged
    {
        public string ProjectPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SpinejsonWorkspace"
            );
        public string Name { get; set; } = "NewProject";

        public MetaData MetaData { get; set; } = new MetaData { Spine = "4.2.22" };

        public Skeleton? MainSkeleton { get; set; } = null;
        public Mode currentMode = new NoMode();
        public int seletedBoneId = -1;
        public ObservableCollection<Slot> Slots { get; set; } = new ObservableCollection<Slot>();
        public SpinejsonCode SpinejsonCode { get; set; } = new SpinejsonCode();

        public ObservableCollection<Res> Resources { get; } = new ObservableCollection<Res>();

        public ObservableCollection<Animation> Animations { get; } =
            new ObservableCollection<Animation>();

        public ObservableCollection<Skin> Skins { get; } = new ObservableCollection<Skin>();
        private Skin _currentSkin;
        private Animation _currentAnimation;

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
                    foreach (Bone b in MainSkeleton.Bones)
                    {
                        b.UpdateSlots();
                    }
                }
            }
        }

        public Animation CurrentAnimation
        {
            get => _currentAnimation;
            set
            {
                if (_currentAnimation != value)
                {
                    _currentAnimation = value;
                    OnPropertyChanged(nameof(CurrentAnimation));
                }
            }
        }

        public Project()
        {
            MainSkeleton = new Skeleton();
            Animations.Add(new Animation());
            CurrentAnimation = Animations[0];
            Skins.Add(new Skin());
            CurrentSkin = Skins[0];
        }

        public Project(string name, string path)
            : this()
        {
            this.Name = name;
            this.ProjectPath = path;
        }

        public Animation GetAnimation()
        {
            return CurrentAnimation;
        }

        public void AddAnimation()
        {
            this.Animations.Add(new Animation("anim" + Animations.Count.ToString()));
        }

        public void DeleteAnimation()
        {
            if (this.Animations.Count > 1)
            {
                this.Animations.Remove(CurrentAnimation);
                CurrentAnimation = this.Animations[0];
            }
        }

        public void AddSkin()
        {
            this.Skins.Add(new Skin("skin" + Skins.Count.ToString()));
        }

        public void DeleteSkin()
        {
            if (this.Skins.Count > 1)
            {
                this.Skins.Remove(CurrentSkin);
                CurrentSkin = this.Skins[0];
            }
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

        public Slot GetSlot(string name)
        {
            foreach (Slot s in Slots)
            {
                if (name == s.Name)
                {
                    return s;
                }
            }

            return null;
        }

        public void drawSlots(Canvas c)
        {
            CurrentSkin.DrawSkin(c);
        }

        public string GetProjectPath()
        {
            return Path.Combine(this.ProjectPath, this.Name);
        }

        public MetaData generateMetaData()
        {
            return this.MetaData;
        }

        public List<SkinData> generateSkinsJSONData()
        {
            List<SkinData> skinData = new List<SkinData>();
            foreach (Skin s in Skins)
            {
                skinData.Add(s.generateJSONData());
            }
            return skinData;
        }

        public List<SlotData> generateSlotsJSONData()
        {
            List<SlotData> slotData = new List<SlotData>();
            foreach (Slot s in Slots)
            {
                slotData.Add(s.generateJSONData());
            }
            return slotData;
        }

        public Dictionary<string, AnimationData> generateAnimationsJSONData()
        {
            Dictionary<string, AnimationData> animData = new Dictionary<string, AnimationData>();

            foreach (Animation a in Animations)
            {
                animData.Add(a.Name, a.generateJSONData());
            }

            return animData;
        }

        public Res? FindRes(string name)
        {
            foreach (Res res in this.Resources)
            {
                if (res.Name == name)
                {
                    return res;
                }
            }
            return null;
        }

        public void regenrateProject(
            Dictionary<string, BoneData> bones,
            Dictionary<string, SlotData> slots,
            Dictionary<string, SkinData> skins,
            Dictionary<string, AnimationData> animations
        )
        {
            // recreate bones
            List<Bone> bonesToRemove = new List<Bone>();

            foreach (Bone b in this.MainSkeleton.Bones)
            {
                if (bones.TryGetValue(b.Name, out BoneData boneData))
                {
                    if (b.generateJSONData() != boneData)
                    {
                        b.x = boneData.X;
                        b.y = boneData.Y;
                        b.Parent = this.MainSkeleton.getBone(boneData.Parent);
                    }
                    bones.Remove(b.Name);
                }
                else
                {
                    bonesToRemove.Add(b);
                }
            }

            foreach (var bone in bones)
            {
                Bone b = new Bone(bone.Key, this.MainSkeleton.getBone(bone.Value.Parent));
                this.MainSkeleton.Bones.Add(b);
            }

            foreach (var bone in bonesToRemove)
            {
                this.MainSkeleton.Bones.Remove(bone);
            }

            // recreate slots
            List<Slot> slotsToRemove = new List<Slot>();

            foreach (Slot b in this.Slots)
            {
                if (slots.TryGetValue(b.Name, out SlotData slotData))
                {
                    if (b.generateJSONData() != slotData)
                    {
                        b.BoundedBone = this.MainSkeleton.getBone(slotData.Bone);
                    }
                    slots.Remove(b.Name);
                }
                else
                {
                    slotsToRemove.Add(b);
                }
            }

            foreach (var slot in slots)
            {
                Slot s = new Slot(slot.Key, this.MainSkeleton.getBone(slot.Value.Bone));
                this.Slots.Add(s);
            }

            foreach (var slot in slotsToRemove)
            {
                this.Slots.Remove(slot);
            }

            // recreate skins and slot-bone bounding
            List<Skin> skinsToRemove = new List<Skin>();

            foreach (Skin b in this.Skins)
            {
                if (skins.TryGetValue(b.Name, out SkinData skinData))
                {
                    if (b.generateJSONData() != skinData)
                    {
                        b.SlotAttachmentBinding = new Dictionary<Slot, Attachment>();
                        foreach (string slotName in skinData.Attachments.Keys)
                        {
                            Dictionary<string, AttachmentData> attachs = skinData.Attachments[
                                slotName
                            ];
                            foreach (string attachName in attachs.Keys)
                            {
                                var attach = attachs[attachName];
                                ImageAttachment a = new ImageAttachment(
                                    (ImageRes)this.FindRes(attach.Name)
                                );
                                b.BindSlotAttachment(this.GetSlot(slotName), a);
                            }
                        }
                    }
                    skins.Remove(b.Name);
                }
                else
                {
                    skinsToRemove.Add(b);
                }
            }

            foreach (var skin in skins)
            {
                Skin s = new Skin(skin.Key);
                var skinData = skin.Value;
                s.SlotAttachmentBinding = new Dictionary<Slot, Attachment>();
                foreach (string slotName in skinData.Attachments.Keys)
                {
                    Dictionary<string, AttachmentData> attachs = skinData.Attachments[slotName];
                    foreach (string attachName in attachs.Keys)
                    {
                        var attach = attachs[attachName];
                        ImageAttachment a = new ImageAttachment(
                            (ImageRes)this.FindRes(attach.Name)
                        );
                        s.BindSlotAttachment(this.GetSlot(slotName), a);
                    }
                }
                this.Skins.Add(s);
            }

            foreach (var skin in skinsToRemove)
            {
                this.Skins.Remove(skin);
            }

            // recreate animations
            List<Animation> animationsToRemove = new List<Animation>();

            foreach (Animation b in this.Animations)
            {
                if (animations.TryGetValue(b.Name, out AnimationData animationData))
                {
                    if (b.generateJSONData() != animationData)
                    {
                        b.BoneAnimationBinding = new Dictionary<Bone, BoneAnimation>();
                        foreach (string name in animationData.Keys)
                        {
                            var boneDict = animationData[name];
                            foreach (string boneName in boneDict.Keys)
                            {
                                Bone bone = this.MainSkeleton.getBone(boneName);
                                foreach (IKeyframeTypeData keyframe in boneDict[boneName].rotate)
                                {
                                    b.RotateBone(bone, keyframe.Value, keyframe.Time);
                                }
                                foreach (IKeyframeTypeData keyframe in boneDict[boneName].translate)
                                {
                                    b.TranslateBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                                }
                            }
                        }
                    }
                    animations.Remove(b.Name);
                }
                else
                {
                    animationsToRemove.Add(b);
                }
            }

            foreach (var animation in animations)
            {
                Animation a = new Animation(animation.Key);
                a.BoneAnimationBinding = new Dictionary<Bone, BoneAnimation>();
                var animationData = animation.Value;
                foreach (string name in animationData.Keys)
                {
                    var boneDict = animationData[name];
                    foreach (string boneName in boneDict.Keys)
                    {
                        Bone bone = this.MainSkeleton.getBone(boneName);
                        foreach (IKeyframeTypeData keyframe in boneDict[boneName].rotate)
                        {
                            a.RotateBone(bone, keyframe.Value, keyframe.Time);
                        }
                        foreach (IKeyframeTypeData keyframe in boneDict[boneName].translate)
                        {
                            a.TranslateBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                        }
                    }
                }
                this.Animations.Add(a);
            }

            foreach (var animation in animationsToRemove)
            {
                this.Animations.Remove(animation);
            }

            this.CurrentSkin = Skins[0];
            this.CurrentAnimation = Animations[0];
        }
    }
}

public class MetaData
{
    [JsonProperty("spine")]
    public string Spine { get; set; }
}
