using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models
{
    public partial class Project : INotifyable
    {
        public string ProjectPath { get; set; }
        private string _name = "NewProject";
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public MetaData MetaData { get; set; } = new MetaData();

        public Mode currentMode;

        public Skeleton? MainSkeleton { get; set; } = null;
        public ObservableCollection<Slot> Slots { get; set; } = new ObservableCollection<Slot>();
        public ObservableCollection<Res> Resources { get; } = new ObservableCollection<Res>();
        public ObservableCollection<Animation> Animations { get; } =
            new ObservableCollection<Animation>();

        public ObservableCollection<Skin> Skins { get; } = new ObservableCollection<Skin>();
        private Skin _currentSkin;
        private Animation? _currentAnimation;

        private GlobalState _globalState;
        private Interpolation _interpolation;

        public Skin CurrentSkin
        {
            get => _currentSkin;
            set
            {
                if (_currentSkin != value)
                {
                    _currentSkin = value;
                    OnPropertyChanged(nameof(CurrentSkin));
                    foreach (Bone b in MainSkeleton!.Bones)
                    {
                        b.UpdateSlots();
                    }
                }
            }
        }

        public Animation? CurrentAnimation
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

        private string _code = "";
        public string Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        public Project(GlobalState globalState, Interpolation interpolation)
        {
            MainSkeleton = new Skeleton(globalState);
            Animations.Add(new Animation(globalState, interpolation));
            CurrentAnimation = Animations[0];
            Skins.Add(new Skin(globalState));
            CurrentSkin = Skins[0];

            this.currentMode = new NoMode(globalState);

            this._globalState = globalState;
            this._interpolation = interpolation;
        }

        public Project(
            string name,
            string path,
            GlobalState globalState,
            Interpolation interpolation
        )
            : this(globalState, interpolation)
        {
            this.Name = name;
            this.ProjectPath = path;
        }

        public void SetupProjectSettings(SettingsData settingsData)
        {
            this.ProjectPath = settingsData.Path;
            this.Name = settingsData.Name;
            this.MetaData.Spine = settingsData.Spine;
            this.Code = settingsData.Anim;
        }

        public Animation? GetCurrentAnimation()
        {
            return CurrentAnimation;
        }

        public void AddAnimation()
        {
            this.Animations.Add(
                new Animation(
                    this._globalState,
                    this._interpolation,
                    "anim" + Animations.Count.ToString()
                )
            );
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
            this.Skins.Add(new Skin("skin" + Skins.Count.ToString(), this._globalState));
        }

        public void DeleteSkin()
        {
            if (this.Skins.Count > 1)
            {
                this.Skins.Remove(CurrentSkin);
                CurrentSkin = this.Skins[0];
            }
        }

        public Slot? GetSlotByName(string name)
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

        public void DrawSlots(Canvas c)
        {
            CurrentSkin.DrawSkin(c);
        }

        public string GetProjectPath()
        {
            return Path.Combine(this.ProjectPath, this.Name);
        }

        /// <summary>
        /// Generates JSON object of metadata
        /// </summary>
        public MetaData GenerateMetaData()
        {
            return this.MetaData;
        }

        /// <summary>
        /// Returns list of JSON objects of skins
        /// </summary>
        public List<SkinData> GenerateSkinsJSONData()
        {
            List<SkinData> skinData = new List<SkinData>();
            foreach (Skin s in Skins)
            {
                skinData.Add(s.GenerateJSONData());
            }
            return skinData;
        }

        /// <summary>
        /// Returns list of JSON objects of slots
        /// </summary>
        public List<SlotData> GenerateSlotsJSONData()
        {
            List<SlotData> slotData = new List<SlotData>();
            foreach (Slot s in Slots)
            {
                slotData.Add(s.GenerateJSONData());
            }
            return slotData;
        }

        /// <summary>
        /// Returns list of JSON objects of animations
        /// </summary>
        public Dictionary<string, AnimationData> GenerateAnimationsJSONData()
        {
            Dictionary<string, AnimationData> animData = new Dictionary<string, AnimationData>();

            foreach (Animation a in Animations)
            {
                animData.Add(a.Name, a.GenerateJSONData());
            }

            return animData;
        }

        public Res? GetResByName(string name)
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

        /// <summary>
        /// Regenerates project from JSON objects
        /// </summary>
        public void RegenerateProject(
            Dictionary<string, BoneData> bones,
            Dictionary<string, SlotData> slots,
            Dictionary<string, SkinData> skins,
            Dictionary<string, AnimationData> animations
        )
        {
            // recreate bones
            List<Bone> bonesToRemove = new List<Bone>();

            foreach (Bone b in this.MainSkeleton!.Bones)
            {
                if (bones.TryGetValue(b.Name, out BoneData? boneData))
                {
                    if (b.GenerateJSONData() != boneData)
                    {
                        b.x = boneData.X;
                        b.y = boneData.Y;
                        b.Parent = this.MainSkeleton.GetBoneByName(boneData.Parent);
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
                Bone b = new Bone(this._globalState, bone.Key, null);
                this.MainSkeleton.AddBone(b);
            }

            foreach (var bone in bones)
            {
                if (!string.IsNullOrEmpty(bone.Value.Parent))
                {
                    var childBone = MainSkeleton.GetBoneByName(bone.Key);
                    var parentBone = MainSkeleton.GetBoneByName(bone.Value.Parent);

                    if (childBone != null && parentBone != null)
                    {
                        childBone.Parent = parentBone;
                        parentBone.AddChildren(childBone);
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Warning: Cannot set parent {bone.Value.Parent} for bone {bone.Key}"
                        );
                    }
                }
            }

            foreach (var bone in bonesToRemove)
            {
                this.MainSkeleton.Bones.Remove(bone);
            }

            if (this.MainSkeleton.Bones.Count <= 0)
            {
                this.MainSkeleton.Bones.Add(new Bone(this._globalState));
                this.MainSkeleton.RootBones = new ObservableCollection<Bone>()
                {
                    this.MainSkeleton.Bones[0],
                };
            }

            // recreate slots
            List<Slot> slotsToRemove = new List<Slot>();

            foreach (Slot b in this.Slots)
            {
                if (slots.TryGetValue(b.Name, out SlotData slotData))
                {
                    if (b.GenerateJSONData() != slotData)
                    {
                        b.BoundedBone = this.MainSkeleton.GetBoneByName(slotData.Bone);
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
                Slot s = new Slot(
                    this._globalState,
                    slot.Key,
                    this.MainSkeleton.GetBoneByName(slot.Value.Bone)
                );
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
                    if (b.GenerateJSONData() != skinData)
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
                                    (ImageRes)this.GetResByName(attach.Name),
                                    attach
                                );
                                b.BindSlotAttachment(this.GetSlotByName(slotName), a);
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
                Skin s = new Skin(skin.Key, this._globalState);
                var skinData = skin.Value;
                s.SlotAttachmentBinding = new Dictionary<Slot, Attachment>();
                foreach (string slotName in skinData.Attachments.Keys)
                {
                    Dictionary<string, AttachmentData> attachs = skinData.Attachments[slotName];
                    foreach (string attachName in attachs.Keys)
                    {
                        var attach = attachs[attachName];
                        ImageAttachment a = new ImageAttachment(
                            (ImageRes)this.GetResByName(attach.Name),
                            attach
                        );
                        s.BindSlotAttachment(this.GetSlotByName(slotName), a);
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
                    if (b.GenerateJSONData() != animationData)
                    {
                        b.BoneAnimationBinding = new Dictionary<Bone, BoneAnimation>();
                        foreach (string name in animationData.Bones.Keys)
                        {
                            var boneAnimation = animationData.Bones[name];
                            Bone bone = this.MainSkeleton.GetBoneByName(name);
                            foreach (IKeyframeTypeData keyframe in boneAnimation.rotate)
                            {
                                b.RotateBone(bone, keyframe.Value, keyframe.Time);
                            }
                            foreach (IKeyframeTypeData keyframe in boneAnimation.translate)
                            {
                                b.TranslateBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                            }
                        }
                        if (animationData.DrawOrder != null && animationData != null)
                        {
                            foreach (DrawOrderItem item in animationData.DrawOrder)
                            {
                                foreach (DrawOrderOffset drawOrderOffset in item.Offsets)
                                {
                                    Slot s = this!.GetSlotByName(drawOrderOffset.Slot);
                                    if (s != null)
                                    {
                                        if (s.drawOrders.ContainsKey((double)item.Time))
                                        {
                                            s.drawOrders[(double)item.Time] = drawOrderOffset;
                                        }
                                        else
                                        {
                                            s.drawOrders.Add((double)item.Time, drawOrderOffset);
                                        }

                                        if (item.Time == 0)
                                        {
                                            s.isUpdatingFromCode = true;
                                            s.CurrentDrawOrderOffset = drawOrderOffset.Offset;
                                            s.isUpdatingFromCode = false;
                                        }
                                    }
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
                Animation a = new Animation(this._globalState, this._interpolation, animation.Key);
                a.BoneAnimationBinding = new Dictionary<Bone, BoneAnimation>();
                var animationData = animation.Value;

                foreach (string name in animationData.Bones.Keys)
                {
                    var boneAnimation = animationData.Bones[name];
                    Bone bone = this.MainSkeleton.GetBoneByName(name);
                    foreach (IKeyframeTypeData keyframe in boneAnimation.rotate)
                    {
                        a.RotateBone(bone, keyframe.Value, keyframe.Time);
                    }
                    foreach (IKeyframeTypeData keyframe in boneAnimation.translate)
                    {
                        a.TranslateBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                    }
                }
                foreach (DrawOrderItem item in animationData.DrawOrder)
                {
                    foreach (DrawOrderOffset drawOrderOffset in item.Offsets)
                    {
                        Slot s = this!.GetSlotByName(drawOrderOffset.Slot);
                        if (s != null)
                        {
                            if (s.drawOrders.ContainsKey((double)item.Time))
                            {
                                s.drawOrders[(double)item.Time] = drawOrderOffset;
                            }
                            else
                            {
                                s.drawOrders.Add((double)item.Time, drawOrderOffset);
                            }

                            if (item.Time == 0)
                            {
                                s.isUpdatingFromCode = true;
                                s.CurrentDrawOrderOffset = drawOrderOffset.Offset;
                                s.isUpdatingFromCode = false;
                            }
                        }
                    }
                }
                this.Animations.Add(a);
            }

            foreach (var animation in animationsToRemove)
            {
                this.Animations.Remove(animation);
            }

            if (Skins.Count <= 0)
            {
                Skins.Add(new Skin(this._globalState));
            }
            if (Animations.Count <= 0)
            {
                Animations.Add(new Animation(this._globalState, this._interpolation));
            }

            this.CurrentSkin = Skins[0];
            this.CurrentAnimation = Animations[0];
        }
    }

    /// <summary>
    /// Jsonifyed project metadata
    /// </summary>
    public class MetaData
    {
        [JsonProperty("spine")]
        public string? Spine { get; set; }
    }
}
