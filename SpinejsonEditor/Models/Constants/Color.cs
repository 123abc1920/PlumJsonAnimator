using System;
using System.Collections.Generic;
using AnimModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using SpinejsonEditor.ViewModels;
using transformModes;

namespace Constants
{
    public class Color
    {
        public static IImmutableBrush Red = Brushes.Red;
        public static IImmutableBrush Green = Brushes.Green;
        public static IImmutableBrush Blue = Brushes.Blue;
        public static IImmutableBrush Aqua = Brushes.Aqua;

        public static IImmutableBrush getDotBoneColor(int id)
        {
            if (ConstantsClass.currentProject.seletedBoneId == id)
            {
                return Red;
            }
            else
            {
                return Green;
            }
        }

        public static IImmutableBrush getLineBoneColor(int id)
        {
            if (ConstantsClass.currentProject.seletedBoneId == id)
            {
                return Blue;
            }
            else
            {
                return Aqua;
            }
        }
    }
}
