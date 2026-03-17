using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Common;

namespace PlumJsonAnimator.Common.Constants
{
    public class TransformModeFactory
    {
        private GlobalState globalState;
        private Mode[] modes;

        public TransformModeFactory(GlobalState globalState)
        {
            this.globalState = globalState;

            modes = new Mode[]
            {
                new NoMode(globalState),
                new TransformMode(globalState),
                new RotateMode(globalState),
                new ScaleMode(globalState),
            };
        }

        public Mode createMode(Mode old, TransformModesTypes type)
        {
            if (old.type == type)
            {
                return new NoMode(this.globalState);
            }
            else
            {
                if (type == TransformModesTypes.TRANSLATE)
                {
                    return new TransformMode(this.globalState);
                }
                else if (type == TransformModesTypes.ROTATE)
                {
                    return new RotateMode(this.globalState);
                }
                else if (type == TransformModesTypes.SCALE)
                {
                    return new ScaleMode(this.globalState);
                }
            }
            return new NoMode(this.globalState);
        }
    }
}
