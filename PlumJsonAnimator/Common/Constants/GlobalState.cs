using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Newtonsoft.Json;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Common.Constants
{
    public class GlobalState
    {
        public Project? currentProject = null;
        public JsonError jsonError = new JsonError();
        public JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        };
        public int FPS = 60;
        public event Action TimeUpdated;

        public void OnTimeUpdated()
        {
            TimeUpdated?.Invoke();
        }

        public Bone? currentBone = null;
        public string theme = "light";
        public bool drawBones = true;
        public bool captureMode = false;
        public string workspace = "PlumJsonAnimatorWorkspace";
        public string programExt = ".plmjsn";

        public CaptureArea? captureArea;

        public ParallelOptions GetParallelOptions()
        {
            int processorCount = Environment.ProcessorCount;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = processorCount };
            return parallelOptions;
        }

        public IImmutableBrush getDotBoneColor(int id)
        {
            if (this.currentBone?.id == id && this.currentBone.IsBone == true)
            {
                return AppColors.Red;
            }
            else
            {
                return AppColors.Green;
            }
        }

        public IImmutableBrush getLineBoneColor(int id)
        {
            if (this.currentBone?.id == id && this.currentBone.IsBone == true)
            {
                return AppColors.Blue;
            }
            else
            {
                return AppColors.Aqua;
            }
        }

        public bool IsSlotSelected(Slot slot)
        {
            if (this.currentBone != null)
            {
                if (!this.currentBone.IsBone && this.currentBone == slot)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
