using System.Collections.Generic;
using System.Linq;

namespace SpinejsonGeneration
{
    public class SpinejsonModel
    {
        public static void LoadProjectAnim()
        {
            
        }

        public static Dictionary<string, BoneData> regenerateBones(
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

        public static Dictionary<string, SlotData> regenerateSlots(
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

        public static Dictionary<string, SkinData> regenerateSkins(
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

        public static Dictionary<string, AnimationData> regenerateAnimations(
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
    }
}
