using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnimEngine.Models;
using AnimEngine.Project;
using AnimModels;
using Newtonsoft.Json;
using PlumJsonAnimator.ViewModels;
using SpinejsonGeneration.JsonValidator;

namespace Common.Constants
{
    public class ConstantsClass
    {
        public static MainWindowViewModel? viewModel = null;
        public static Project? currentProject = null;
        public static JsonError jsonError = new JsonError();
        public static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        };
        public static int FPS = 60;
        public static Bone? currentBone = null;
        public static Engine MainEngine = new Engine();
        public static Dictionary<char, char> pairedSymbols = new Dictionary<char, char>()
        {
            { '{', '}' },
            { '[', ']' },
            { '"', '"' },
            { '<', '>' },
        };
        public static string theme = "light";
        public static bool drawBones = true;
        public static string workspace = "PlumJsonAnimatorWorkspace";
        public static string programExt = ".plmjsn";

        public static ParallelOptions GetParallelOptions()
        {
            int processorCount = Environment.ProcessorCount;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = processorCount };
            return parallelOptions;
        }
    }
}
