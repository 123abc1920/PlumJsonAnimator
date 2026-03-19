using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Services
{
    public class ValidResult
    {
        public required string Message { get; set; }
        public bool IsOk { get; set; }
        public object? UpdatedArray { get; set; }
    }

    public class ProjectValidResult
    {
        public required string Message { get; set; }
        public bool IsOk { get; set; }
    }

    public class JsonCode
    {
        private Prettify prettify;

        private GlobalState globalState;

        public JsonCode(GlobalState globalState, Prettify prettify)
        {
            this.globalState = globalState;
            this.prettify = prettify;
        }

        public JsonCode(Prettify prettify)
        {
            this.prettify = prettify;
        }

        private Dictionary<string, BoneData> regenerateBones(
            Dictionary<string, BoneData> newBones,
            Dictionary<string, BoneData> oldBones
        )
        {
            foreach (var kvp in newBones)
            {
                oldBones[kvp.Key] = kvp.Value;
            }

            var keysToRemove = oldBones.Keys.Except(newBones.Keys).ToList();
            foreach (var key in keysToRemove)
            {
                oldBones.Remove(key);
            }

            return oldBones;
        }

        private Dictionary<string, SlotData> regenerateSlots(
            Dictionary<string, SlotData> newSlots,
            Dictionary<string, SlotData> oldSlots
        )
        {
            foreach (var kvp in newSlots)
            {
                oldSlots[kvp.Key] = kvp.Value;
            }

            var keysToRemove = oldSlots.Keys.Except(newSlots.Keys).ToList();
            foreach (var key in keysToRemove)
            {
                oldSlots.Remove(key);
            }
            return oldSlots;
        }

        private Dictionary<string, SkinData> regenerateSkins(
            Dictionary<string, SkinData> newSkins,
            Dictionary<string, SkinData> oldSkins
        )
        {
            foreach (var kvp in newSkins)
            {
                oldSkins[kvp.Key] = kvp.Value;
            }

            var keysToRemove = oldSkins.Keys.Except(newSkins.Keys).ToList();
            foreach (var key in keysToRemove)
            {
                oldSkins.Remove(key);
            }
            return oldSkins;
        }

        private Dictionary<string, AnimationData> regenerateAnimations(
            Dictionary<string, AnimationData> newAnimations,
            Dictionary<string, AnimationData> oldAnimations
        )
        {
            foreach (var kvp in newAnimations)
            {
                oldAnimations[kvp.Key] = kvp.Value;
            }

            var keysToRemove = oldAnimations.Keys.Except(newAnimations.Keys).ToList();
            foreach (var key in keysToRemove)
            {
                oldAnimations.Remove(key);
            }
            return oldAnimations;
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
            foreach (Slot s in this.globalState.currentProject!.Slots)
            {
                slots.Add(s.generateJSONData());
            }

            return new CodeData
            {
                Skeleton = this.globalState.currentProject.generateMetaData(),
                Bones = project.MainSkeleton!.generateJSONData(),
                Slots = slots,
                Animations = animations,
                Skins = this.globalState.currentProject.generateSkinsJSONData(),
            };
        }

        public void generateCode(Project project)
        {
            var _text = JsonConvert.SerializeObject(
                generateJSONData(project),
                this.globalState.jsonSettings
            );

            _text = this.prettify.prettify(_text);
            project.Code = _text;
        }

        public ValidResult regenerateBones(List<BoneData> bones)
        {
            List<BoneData> newBones = bones;
            List<BoneData> oldBones =
                this.globalState.currentProject!.MainSkeleton!.generateJSONData();
            Dictionary<string, BoneData> updatedBones = regenerateBones(
                newBones.ToDictionary(b => b.Name, b => b),
                oldBones.ToDictionary(b => b.Name, b => b)
            );

            int rootBonesCount = 0;
            foreach (BoneData b in updatedBones.Values)
            {
                if (b.Name == null)
                {
                    return new ValidResult
                    {
                        Message = "Ошибка: У одной из костей не установлено имя",
                        IsOk = false,
                        UpdatedArray = null,
                    };
                }
                if (b.Parent == null)
                {
                    rootBonesCount++;
                }
                else
                {
                    Bone? parent = this.globalState.currentProject.MainSkeleton.getBone(b.Parent);
                    if (parent == null)
                    {
                        return new ValidResult
                        {
                            Message = $"Ошибка: Кость {b.Name} имеет недействительного родителя",
                            IsOk = false,
                            UpdatedArray = null,
                        };
                    }
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
            List<SlotData> oldSlots = this.globalState.currentProject!.generateSlotsJSONData();
            Dictionary<string, SlotData> updatedSlots = regenerateSlots(
                newSlots.ToDictionary(b => b.Name, b => b),
                oldSlots.ToDictionary(b => b.Name, b => b)
            );

            foreach (SlotData s in updatedSlots.Values)
            {
                if (s.Name == null)
                {
                    return new ValidResult
                    {
                        Message = "У одного из слотов не установлено имя!",
                        IsOk = false,
                        UpdatedArray = null,
                    };
                }
            }

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
            List<SkinData> oldSkins = this.globalState.currentProject!.generateSkinsJSONData();
            Dictionary<string, SkinData> updatedSkins = regenerateSkins(
                newSkins.ToDictionary(b => b.Name, b => b),
                oldSkins.ToDictionary(b => b.Name, b => b)
            );

            foreach (SkinData s in updatedSkins.Values)
            {
                if (s.Name == null)
                {
                    return new ValidResult
                    {
                        Message = "У одного из скинов не установлено имя!",
                        IsOk = false,
                        UpdatedArray = null,
                    };
                }
            }

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
                this.globalState.currentProject!.generateAnimationsJSONData();
            Dictionary<string, AnimationData> updatedAnimations = regenerateAnimations(
                newAnimations,
                oldAnimations
            );

            return new ValidResult
            {
                Message = "",
                IsOk = true,
                UpdatedArray = updatedAnimations,
            };
        }

        public ProjectValidResult regenerate(Project project)
        {
            if (project.Code == null || project.Code == "")
            {
                return new ProjectValidResult { Message = "Empty json code", IsOk = false };
            }

            var codeData = JsonConvert.DeserializeObject<CodeData>(project.Code);

            if (codeData == null)
            {
                return new ProjectValidResult { Message = "", IsOk = false };
            }

            var boneResult = regenerateBones(codeData.Bones);
            if (!boneResult.IsOk)
            {
                return new ProjectValidResult { Message = boneResult.Message, IsOk = false };
            }

            var slotResult = regenerateSlots(codeData.Slots);
            if (!slotResult.IsOk)
            {
                return new ProjectValidResult { Message = slotResult.Message, IsOk = false };
            }

            var skinResult = regenerateSkins(codeData.Skins);
            if (!skinResult.IsOk)
            {
                return new ProjectValidResult { Message = skinResult.Message, IsOk = false };
            }

            var animationResult = regenerateAnimations(codeData.Animations);
            if (!animationResult.IsOk)
            {
                return new ProjectValidResult { Message = animationResult.Message, IsOk = false };
            }

            this.globalState.currentProject?.regenerateProject(
                (Dictionary<string, BoneData>)boneResult.UpdatedArray,
                (Dictionary<string, SlotData>)slotResult.UpdatedArray,
                (Dictionary<string, SkinData>)skinResult.UpdatedArray,
                (Dictionary<string, AnimationData>)animationResult.UpdatedArray
            );

            return new ProjectValidResult { Message = "", IsOk = true };
        }
    }

    public class CodeData
    {
        [JsonProperty("skeleton")]
        public required MetaData Skeleton { get; set; }

        [JsonProperty("bones")]
        public required List<BoneData> Bones { get; set; }

        [JsonProperty("slots")]
        public required List<SlotData> Slots { get; set; }

        [JsonProperty("skins")]
        public required List<SkinData> Skins { get; set; }

        [JsonProperty("animations")]
        public required Dictionary<string, AnimationData> Animations { get; set; }
    }
}
