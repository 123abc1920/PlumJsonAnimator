using System.Collections.ObjectModel;
using AnimModels;
using Avalonia.Controls;
using transformModes;

namespace EngineModels
{
    public partial class Project
    {
        public string ProjectPath { get; set; } = "C:/Users/Документы/";
        public string Name { get; set; } = "NewProject";

        public Skeleton? mainSkeleton = null;
        public Mode currentMode = new NoMode();
        public int seletedBoneId = -1;
        public ObservableCollection<Slot> Slots { get; set; } = new ObservableCollection<Slot>();

        public Project()
        {
            mainSkeleton = new Skeleton();
        }

        public void drawSlots(Canvas c)
        {
            foreach (Slot s in Slots)
            {
                s.drawSlot(c);
            }
        }
    }
}
