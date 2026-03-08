using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Skeleton;

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
        public Engine MainEngine = new Engine();
        public Dictionary<char, char> pairedSymbols = new Dictionary<char, char>()
        {
            { '{', '}' },
            { '[', ']' },
            { '"', '"' },
            { '<', '>' },
        };
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
    }
}
