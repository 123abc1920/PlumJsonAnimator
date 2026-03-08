using Avalonia.Media;

namespace PlumJsonAnimator.Common.Constants
{
    public class Color
    {
        private GlobalState globalState;

        public Color(GlobalState globalState)
        {
            this.globalState = globalState;
        }

        public IImmutableBrush Red = Brushes.Red;
        public IImmutableBrush Green = Brushes.Green;
        public IImmutableBrush Blue = Brushes.Blue;
        public IImmutableBrush Aqua = Brushes.Aqua;

        public IImmutableBrush getDotBoneColor(int id)
        {
            if (this.globalState.currentProject?.seletedBoneId == id)
            {
                return Red;
            }
            else
            {
                return Green;
            }
        }

        public IImmutableBrush getLineBoneColor(int id)
        {
            if (this.globalState.currentProject?.seletedBoneId == id)
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
