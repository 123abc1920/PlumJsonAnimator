using EngineModels;
using JsonValidator;
using SpinejsonEditor.ViewModels;

namespace Constants
{
    public class ConstantsClass
    {
        public static MainWindowViewModel? viewModel = null;
        public static Project? currentProject = null;
        public static JsonError jsonError = new JsonError();
    }
}
