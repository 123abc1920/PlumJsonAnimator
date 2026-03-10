using Avalonia.Media;

namespace PlumJsonAnimator.Common.Constants
{
    public class Color
    {
        public Color() { }

        public IImmutableBrush Red = Brushes.Red;
        public IImmutableBrush Green = Brushes.Green;
        public IImmutableBrush Blue = Brushes.Blue;
        public IImmutableBrush Aqua = Brushes.Aqua;

        public IImmutableBrush getDotBoneColor(int id)
        {
            /*if (this.globalState.currentProject?.seletedBoneId == id)
            {
                return Red;
            }
            else
            {
                return Green;
            }*/
            return Green;
        }

        public IImmutableBrush getLineBoneColor(int id)
        {
            /*if (this.globalState.currentProject?.seletedBoneId == id)
            {
                return Blue;
            }
            else
            {
                return Aqua;
            }*/
            return Green;
        }
    }
}
