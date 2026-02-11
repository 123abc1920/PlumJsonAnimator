using System.Collections.Generic;
using AnimEngine;
using AnimModels;
using EngineModels;
using JsonValidator;
using Newtonsoft.Json;
using SpinejsonEditor.ViewModels;

namespace Constants
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
        public static Bone currentBone = null;
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
    }
}
