using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.SkeletonNameSpace;

// TODO: localizate valid results
// TODO: replace repetitions
// TODO: Logging
namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Provides methods for converting project into json code
    /// </summary>
    public class JsonCode
    {
        /// <summary>
        /// Contains information about json validation and provides updated validated data
        /// </summary>
        public class ValidResult
        {
            public required string Message { get; set; }
            public bool IsOk { get; set; }
            public object? UpdatedArray { get; set; }
        }

        private Prettify prettify;

        private GlobalState globalState;

        public JsonCode(GlobalState globalState, Prettify prettify)
        {
            this.globalState = globalState;
            this.prettify = prettify;
        }

        /// <summary>
        /// Replaces irrelevant bones with relevant
        /// </summary>
        /// <param name="newBones">Dictionary obtained from the json code</param>
        /// <param name="oldBones">Dictionary obtained from the project</param>
        /// <returns>A dictionary containing only relevant data. The keys are names, the values are BoneData</returns>
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

        /// <summary>
        /// Replaces irrelevant slots with relevant
        /// </summary>
        /// <param name="newSlots">Dictionary obtained from the json code</param>
        /// <param name="oldSlots">Dictionary obtained from the project</param>
        /// <returns>A dictionary containing only relevant data. The keys are names, the values are SlotData</returns>
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

        /// <summary>
        /// Replaces irrelevant skins with relevant
        /// </summary>
        /// <param name="newSkins">Dictionary obtained from the json code</param>
        /// <param name="oldSkins">Dictionary obtained from the project</param>
        /// <returns>A dictionary containing only relevant data. The keys are names, the values are SkinData</returns>
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

        /// <summary>
        /// Replaces irrelevant animations with relevant
        /// </summary>
        /// <param name="newAnimations">Dictionary obtained from the json code</param>
        /// <param name="oldAnimations">Dictionary obtained from the project</param>
        /// <returns>A dictionary containing only relevant data. The keys are names, the values are AnimayionData</returns>
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

        /// <summary>
        /// Generates project json data
        /// </summary>
        /// <param name="project">The project which have to be converted into json</param>
        /// <returns>CodeData</returns>
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
            foreach (Slot s in this.globalState.CurrentProject!.Slots)
            {
                slots.Add(s.generateJSONData());
            }

            return new CodeData
            {
                Skeleton = this.globalState.CurrentProject.generateMetaData(),
                Bones = project.MainSkeleton!.generateJSONData(),
                Slots = slots,
                Animations = animations,
                Skins = this.globalState.CurrentProject.generateSkinsJSONData(),
            };
        }

        /// <summary>
        /// Sets the project json data to project
        /// </summary>
        /// <param name="project">Project that has to get data</param>
        public void generateCode(Project project)
        {
            var _text = JsonConvert.SerializeObject(
                generateJSONData(project),
                this.globalState.jsonSettings
            );

            _text = this.prettify.prettify(_text);
            project.Code = _text;
        }

        /// <summary>
        /// Regenerates bones from json
        /// </summary>
        /// <param name="bones">Dictionary with bones data. The keys are bones names, values are bones jsonifyed data.</param>
        /// ValidResult result=regenerateBones(bones);
        /// if (!result.IsOk)
        /// {
        ///     //DO SOMETHING
        /// }
        /// </example>
        /// <returns>ValoidResult, where UpdatedArray is selected bones</returns>
        private ValidResult regenerateBones(List<BoneData> bones)
        {
            List<BoneData> newBones = bones;
            List<BoneData> oldBones =
                this.globalState.CurrentProject!.MainSkeleton!.generateJSONData();
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
                    bool parent = updatedBones.ContainsKey(b.Parent);
                    if (parent == false)
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

        /// <summary>
        /// Regenerates slots from json
        /// </summary>
        /// <param name="slots">Dictionary with slots data. The keys are slots names, values are slots jsonifyed data.</param>
        /// ValidResult result=regenerateSlots(slots);
        /// if (!result.IsOk)
        /// {
        ///     //DO SOMETHING
        /// }
        /// </example>
        /// <returns>ValoidResult, where UpdatedArray is selected slots</returns>
        private ValidResult regenerateSlots(List<SlotData> slots)
        {
            List<SlotData> newSlots = slots;
            List<SlotData> oldSlots = this.globalState.CurrentProject!.generateSlotsJSONData();
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

        /// <summary>
        /// Regenerates skins from json
        /// </summary>
        /// <param name="skins">Dictionary with skins data. The keys are skins names, values are skins jsonifyed data.</param>
        /// ValidResult result=regenerateSkins(skins);
        /// if (!result.IsOk)
        /// {
        ///     //DO SOMETHING
        /// }
        /// </example>
        /// <returns>ValoidResult, where UpdatedArray is selected skins</returns>
        private ValidResult regenerateSkins(List<SkinData> skins)
        {
            List<SkinData> newSkins = skins;
            List<SkinData> oldSkins = this.globalState.CurrentProject!.generateSkinsJSONData();
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

        /// <summary>
        /// Regenerates animations from json
        /// </summary>
        /// <param name="animations">Dictionary with animations data. The keys are animations names, values are animations jsonifyed data.</param>
        /// <example>
        /// ValidResult result=regenerateAnimations(animations);
        /// if (!result.IsOk)
        /// {
        ///     //DO SOMETHING
        /// }
        /// </example>
        /// <returns>ValoidResult, where UpdatedArray is selected animations</returns>
        private ValidResult regenerateAnimations(Dictionary<string, AnimationData> animations)
        {
            Dictionary<string, AnimationData> newAnimations = animations;
            Dictionary<string, AnimationData> oldAnimations =
                this.globalState.CurrentProject!.generateAnimationsJSONData();
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

        /// <summary>
        /// Regenerates project from its json code
        /// </summary>
        /// <param name="project">Project</param>
        /// <example>
        /// ValidResult result=regenerate(project);
        /// if (!result.IsOk)
        /// {
        ///     //DO SOMETHING
        /// }
        /// </example>
        /// <returns>ValoidResult, where UpdatedArray is empty</returns>
        public ValidResult regenerate(Project project)
        {
            if (project.Code == null || project.Code == "")
            {
                return new ValidResult { Message = "Empty json code", IsOk = false };
            }

            var codeData = JsonConvert.DeserializeObject<CodeData>(project.Code);

            if (codeData == null)
            {
                return new ValidResult { Message = "", IsOk = false };
            }

            var boneResult = regenerateBones(codeData.Bones);
            if (!boneResult.IsOk)
            {
                return new ValidResult { Message = boneResult.Message, IsOk = false };
            }

            var slotResult = regenerateSlots(codeData.Slots);
            if (!slotResult.IsOk)
            {
                return new ValidResult { Message = slotResult.Message, IsOk = false };
            }

            var skinResult = regenerateSkins(codeData.Skins);
            if (!skinResult.IsOk)
            {
                return new ValidResult { Message = skinResult.Message, IsOk = false };
            }

            var animationResult = regenerateAnimations(codeData.Animations);
            if (!animationResult.IsOk)
            {
                return new ValidResult { Message = animationResult.Message, IsOk = false };
            }

            this.globalState.CurrentProject?.regenerateProject(
                (Dictionary<string, BoneData>)boneResult.UpdatedArray,
                (Dictionary<string, SlotData>)slotResult.UpdatedArray,
                (Dictionary<string, SkinData>)skinResult.UpdatedArray,
                (Dictionary<string, AnimationData>)animationResult.UpdatedArray
            );

            return new ValidResult { Message = "", IsOk = true };
        }
    }

    /// <summary>
    /// Jsonifyed project
    /// </summary>
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
