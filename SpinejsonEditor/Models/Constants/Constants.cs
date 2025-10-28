using System.Collections.Generic;
using AnimModels;
using Avalonia.Controls;
using SlotsView;
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
        public static List<SlotImage> SlotImages { get; set; } = new List<SlotImage>();

        public static void drawSlots(Canvas c)
        {
            foreach (SlotImage i in SlotImages)
            {
                var image = new Image
                {
                    Source = new Avalonia.Media.Imaging.Bitmap(i.Path),
                    Width = 100,
                    Height = 100,
                };

                Canvas.SetLeft(image, c.Height / 2 + 10);
                Canvas.SetTop(image, c.Width / 2 + 10);

                c.Children.Add(image);
            }
        }
    }
}
