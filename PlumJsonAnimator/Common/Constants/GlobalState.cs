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
        public Bone? currentBone = null;
        public string theme = "light";
        public bool drawBones = true;
        public string workspace = "PlumJsonAnimatorWorkspace";
        public string programExt = ".plmjsn";

        public ParallelOptions GetParallelOptions()
        {
            int processorCount = Environment.ProcessorCount;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = processorCount };
            return parallelOptions;
        }

        public IImmutableBrush getDotBoneColor(int id)
        {
            if (this.currentBone?.id == id)
            {
                return Color.Red;
            }
            else
            {
                return Color.Green;
            }
        }

        public IImmutableBrush getLineBoneColor(int id)
        {
            if (this.currentBone?.id == id)
            {
                return Color.Blue;
            }
            else
            {
                return Color.Aqua;
            }
        }

        public GlobalState() { }
    }
}

// zip PT62
