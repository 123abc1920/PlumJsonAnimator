using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Commands;
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
        public ObservableCollection<Animation> Animations { get; set; } =
            new ObservableCollection<Animation>();

        public ObservableCollection<Skin> Skins { get; set; } = new ObservableCollection<Skin>();
        private Skin _currentSkin;
        private Animation? _currentAnimation;

        private ProjectSettings _projectSettings;

        private GlobalState _globalState;
        private Interpolation _interpolation;
        private LocalizationService _localizationService;

        public Skin CurrentSkin
        {
            get => _currentSkin;
            set
            {
                if (_currentSkin != value)
                {
                    _currentSkin = value;
                    _currentAnimation?.UpdateAllDrawOrder();
                    OnPropertyChanged(nameof(CurrentSkin));
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
                    _currentAnimation?.UpdateAllDrawOrder();
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

        public Project(
            GlobalState globalState,
            Interpolation interpolation,
            LocalizationService localizationService
        )
        {
            MainSkeleton = new Skeleton(globalState, localizationService);
            Animations.Add(new Animation(globalState, interpolation));
            CurrentAnimation = Animations[0];
            Skins.Add(new Skin(globalState));
            CurrentSkin = Skins[0];

            currentMode = new NoMode(globalState);

            _globalState = globalState;
            _interpolation = interpolation;
            _localizationService = localizationService;
        }

        public Project(
            ProjectSettings projectSettings,
            GlobalState globalState,
            Interpolation interpolation,
            LocalizationService localizationService
        )
            : this(globalState, interpolation, localizationService)
        {
            _projectSettings = projectSettings;
            SetupProjectSettings(projectSettings.GetSettingsData());
        }

        public void SetupProjectSettings(SettingsData settingsData)
        {
            ProjectPath = settingsData.Path;
            Name = settingsData.Name;
            MetaData.Spine = settingsData.Spine;
            Code = settingsData.Anim;
        }

        public Animation? GetCurrentAnimation()
        {
            return CurrentAnimation;
        }

        public Animation AddAnimation()
        {
            Animation newAnimation = new Animation(
                _globalState,
                _interpolation,
                $"anim{Counter.GenerateNamePostfix()}"
            );
            Animations.Add(newAnimation);
            return newAnimation;
        }

        public void RestoreAnimation(Animation animation)
        {
            Animations.Add(animation);
        }

        public void DeleteAnimation(Animation animation)
        {
            if (Animations.Count > 1)
            {
                Animations.Remove(animation);
                CurrentAnimation = Animations[0];
            }
        }

        public Skin AddSkin()
        {
            Skin newSkin = new Skin($"skin{Counter.GenerateNamePostfix()}", _globalState);
            Skins.Add(newSkin);
            return newSkin;
        }

        public void RestoreSkin(Skin skin)
        {
            Skins.Add(skin);
        }

        public void DeleteSkin(Skin skin)
        {
            if (Skins.Count > 1)
            {
                Skins.Remove(skin);
                CurrentSkin = Skins[0];
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
            if (_globalState.CurrentBone?.IsBone == false)
            {
                ((Slot)_globalState.CurrentBone).DrawSlotSelection(c);
            }
        }

        public string GetProjectPath()
        {
            return Path.Combine(ProjectPath, Name);
        }

        /// <summary>
        /// Generates JSON object of metadata
        /// </summary>
        public MetaData GenerateMetaData()
        {
            return MetaData;
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

        /// <summary>
        /// Adds resource in resources list
        /// </summary>
        public void AddRes(Res res)
        {
            Resources.Add(res);
        }

        public Res? GetResByName(string name)
        {
            foreach (Res res in Resources)
            {
                if (res.Name == name)
                {
                    return res;
                }
            }
            return null;
        }

        public List<SlotAttach> DeleteSlotFromProject(Slot slot)
        {
            List<SlotAttach> result = new List<SlotAttach>();

            Slots.Remove(slot);
            foreach (Skin s in Skins)
            {
                if (s.ContainsSlot(slot) == true)
                {
                    Attachment a = s.GetAttachment(slot);
                    result.Add(new SlotAttach(slot, s, a));
                    s.DeleteSlot(slot);
                }
            }

            return result;
        }

        public List<BoneAnim> DeleteBoneFromProject(Bone? bone)
        {
            if (bone == null)
                return null;

            List<BoneAnim> result = new List<BoneAnim>();

            MainSkeleton?.Bones.Remove(bone);
            bone?.Parent?.Children.Remove(bone);
            foreach (Animation a in Animations)
            {
                if (a.ContainsBone(bone))
                {
                    result.Add(new BoneAnim(bone, a, a.GetBoneAnimation(bone)));
                }
                a.DeleteBoneFromAnimation(bone);
            }

            return result;
        }

        public void AddBoneToProject(Bone? bone, Bone? parent = null)
        {
            if (bone == null)
                return;

            MainSkeleton?.Bones.Add(bone);

            if (parent != null)
            {
                bone.Parent = parent;
                parent.Children.Add(bone);
            }
            else
            {
                bone.Parent = null;
            }
        }

        public void RestoreBone(Bone? bone)
        {
            if (bone == null)
                return;

            MainSkeleton.Bones.Add(bone);

            foreach (Slot s in bone.Slots)
            {
                Slots.Add(s);
            }
        }

        public void AddSlotToProject(Slot s, Bone b)
        {
            Slots.Add(s);
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

            foreach (Bone b in MainSkeleton!.Bones)
            {
                if (bones.TryGetValue(b.Name, out BoneData? boneData))
                {
                    if (b.GenerateJSONData() != boneData)
                    {
                        b.BaseX = boneData.X;
                        b.BaseY = boneData.Y;
                        b.BaseA = boneData.Rotation;
                        b.ShearX = boneData.ShearX;
                        b.ShearY = boneData.ShearY;
                        b.Parent = MainSkeleton.GetBoneByName(boneData.Parent);
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
                Bone b = new Bone(_globalState, bone.Key, _localizationService);
                b.BaseX = bone.Value.X;
                b.BaseY = bone.Value.Y;
                b.BaseA = bone.Value.Rotation;
                b.ShearX = bone.Value.ShearX;
                b.ShearY = bone.Value.ShearY;
                MainSkeleton.AddBone(b);
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
                MainSkeleton.Bones.Remove(bone);
            }

            if (MainSkeleton.Bones.Count <= 0)
            {
                MainSkeleton.Bones.Add(new Bone(_globalState, _localizationService));
                MainSkeleton.RootBones = new ObservableCollection<Bone>() { MainSkeleton.Bones[0] };
            }

            // recreate slots
            List<Slot> slotsToRemove = new List<Slot>();

            foreach (Slot slot in Slots.ToList())
            {
                if (slots.TryGetValue(slot.Name, out SlotData slotData))
                {
                    if (slot.GenerateJSONData() != slotData)
                    {
                        slot.BoundedBone?.Slots.Remove(slot);

                        slot.BoundedBone = MainSkeleton.GetBoneByName(slotData.Bone);
                        slot.BoundedBone?.Slots.Add(slot);
                    }
                    slots.Remove(slot.Name);
                }
                else
                {
                    slotsToRemove.Add(slot);
                }
            }

            foreach (var slot in slots)
            {
                Slot s = new Slot(
                    _globalState,
                    slot.Key,
                    MainSkeleton.GetBoneByName(slot.Value.Bone)
                );

                var targetBone = MainSkeleton.GetBoneByName(slot.Value.Bone);
                if (targetBone != null && !targetBone.Slots.Contains(s))
                {
                    targetBone.Slots.Add(s);
                }

                Slots.Add(s);
            }

            foreach (var slot in slotsToRemove)
            {
                slot.BoundedBone?.Slots.Remove(slot);
                Slots.Remove(slot);
            }

            _globalState.CurrentBone = null;

            // recreate animations
            foreach (Slot s in _globalState.CurrentProject.Slots)
            {
                s.drawOrders.Clear();
            }

            List<Animation> animationsToRemove = new List<Animation>();

            foreach (Animation animation in Animations)
            {
                if (animations.TryGetValue(animation.Name, out AnimationData animationData))
                {
                    if (animation.GenerateJSONData() != animationData)
                    {
                        animation.BoneAnimationBinding = new Dictionary<Bone, BoneAnimation>();
                        foreach (string name in animationData.Bones.Keys)
                        {
                            var boneAnimation = animationData.Bones[name];
                            Bone bone = MainSkeleton.GetBoneByName(name);
                            foreach (IKeyframeTypeData keyframe in boneAnimation.rotate)
                            {
                                animation.RotateBone(bone, keyframe.Value, keyframe.Time);
                            }
                            foreach (IKeyframeTypeData keyframe in boneAnimation.translate)
                            {
                                animation.TranslateBone(
                                    bone,
                                    keyframe.X,
                                    keyframe.Y,
                                    keyframe.Time
                                );
                            }
                            foreach (IKeyframeTypeData keyframe in boneAnimation.shear)
                            {
                                animation.ShearBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                            }
                            foreach (IKeyframeTypeData keyframe in boneAnimation.scale)
                            {
                                animation.ScaleBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                            }
                        }
                        if (animationData.DrawOrder != null && animationData != null)
                        {
                            foreach (DrawOrderItem item in animationData.DrawOrder)
                            {
                                foreach (DrawOrderOffset drawOrderOffset in item.Offsets)
                                {
                                    Slot s = GetSlotByName(drawOrderOffset.Slot);
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
                    animations.Remove(animation.Name);
                }
                else
                {
                    animationsToRemove.Add(animation);
                }
            }

            foreach (var animation in animations)
            {
                Animation a = new Animation(_globalState, _interpolation, animation.Key);
                a.BoneAnimationBinding = new Dictionary<Bone, BoneAnimation>();
                var animationData = animation.Value;

                foreach (string name in animationData.Bones.Keys)
                {
                    var boneAnimation = animationData.Bones[name];
                    Bone bone = MainSkeleton.GetBoneByName(name);
                    foreach (IKeyframeTypeData keyframe in boneAnimation.rotate)
                    {
                        a.RotateBone(bone, keyframe.Value, keyframe.Time);
                    }
                    foreach (IKeyframeTypeData keyframe in boneAnimation.translate)
                    {
                        a.TranslateBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                    }
                    foreach (IKeyframeTypeData keyframe in boneAnimation.shear)
                    {
                        a.ShearBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                    }
                    foreach (IKeyframeTypeData keyframe in boneAnimation.scale)
                    {
                        a.ScaleBone(bone, keyframe.X, keyframe.Y, keyframe.Time);
                    }
                }
                foreach (DrawOrderItem item in animationData.DrawOrder)
                {
                    foreach (DrawOrderOffset drawOrderOffset in item.Offsets)
                    {
                        Slot s = GetSlotByName(drawOrderOffset.Slot);
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
                Animations.Add(a);
            }

            foreach (var animation in animationsToRemove)
            {
                Animations.Remove(animation);
            }

            if (Animations.Count <= 0)
            {
                Animations.Add(new Animation(_globalState, _interpolation));
            }
            CurrentAnimation = Animations[0];

            // recreate skins and slot-bone bounding
            List<Skin> skinsToRemove = new List<Skin>();

            foreach (Skin skin in Skins)
            {
                if (skins.TryGetValue(skin.Name, out SkinData skinData))
                {
                    if (skin.GenerateJSONData() != skinData)
                    {
                        skin.SlotAttachmentBinding = new Dictionary<Slot, Attachment>();
                        foreach (string slotName in skinData.Attachments.Keys)
                        {
                            Dictionary<string, AttachmentData> attachs = skinData.Attachments[
                                slotName
                            ];
                            foreach (string attachName in attachs.Keys)
                            {
                                var attach = attachs[attachName];
                                ImageAttachment a = new ImageAttachment(
                                    (ImageRes)GetResByName(attach.Name),
                                    attach
                                );
                                skin.BindSlotAttachment(GetSlotByName(slotName), a);
                            }
                        }
                    }
                    skins.Remove(skin.Name);
                }
                else
                {
                    skinsToRemove.Add(skin);
                }
            }

            foreach (var skin in skins)
            {
                Skin s = new Skin(skin.Key, _globalState);
                var skinData = skin.Value;
                s.SlotAttachmentBinding = new Dictionary<Slot, Attachment>();
                foreach (string slotName in skinData.Attachments.Keys)
                {
                    Dictionary<string, AttachmentData> attachs = skinData.Attachments[slotName];
                    foreach (string attachName in attachs.Keys)
                    {
                        var attach = attachs[attachName];
                        ImageAttachment a = new ImageAttachment(
                            (ImageRes)GetResByName(attach.Name),
                            attach
                        );
                        s.BindSlotAttachment(GetSlotByName(slotName), a);
                    }
                }
                Skins.Add(s);
            }

            foreach (var skin in skinsToRemove)
            {
                Skins.Remove(skin);
            }

            if (Skins.Count <= 0)
            {
                Skins.Add(new Skin(_globalState));
            }
            CurrentSkin = Skins[0];
        }

        public bool IsUniqRes(string name)
        {
            foreach (Res r in Resources)
            {
                if (r.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsUniqBone(string name)
        {
            foreach (Bone bone in MainSkeleton.Bones)
            {
                if (bone.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsUniqSlot(string name)
        {
            foreach (Slot slot in Slots)
            {
                if (slot.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsUniqAttach(string name)
        {
            foreach (Skin skin in Skins)
            {
                if (skin.IsAttachUniq(name) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public void SaveProjectSettings()
        {
            _projectSettings.SaveSettings();
        }

        public void SaveProject(JsonCode jsonCode)
        {
            string project = JsonConvert.SerializeObject(
                jsonCode.generateJSONData(this),
                _globalState.jsonSettings
            );
            _projectSettings.WriteProjectJSON(project);
        }

        public void AutoSaveProjectSettings(JsonCode jsonCode)
        {
            string project = JsonConvert.SerializeObject(
                jsonCode.generateJSONData(this),
                _globalState.jsonSettings
            );

            _projectSettings.WriteAutoSave(project);
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
