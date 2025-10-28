using AnimModels;
using Avalonia.Controls;
using SpinejsonEditor.ViewModels;
using transformModes;

namespace Constants
{
    public class ConstantsClass
    {
        public static Skeleton? mainSkeleton = null;
        public static MainWindowViewModel? viewModel = null;
        public static Mode currentMode = new NoMode();
        public static int seletedBoneId = -1;
    }
}
