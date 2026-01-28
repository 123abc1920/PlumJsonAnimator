using EngineModels;
using JsonValidator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    }
}
