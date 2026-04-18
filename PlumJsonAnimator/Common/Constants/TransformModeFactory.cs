using System.Collections.Generic;
using PlumJsonAnimator.Models.Common;

namespace PlumJsonAnimator.Common.Constants
{
    /// <summary>
    /// Creates transform modes
    /// </summary>
    public class TransformModeFactory
    {
        private GlobalState _globalState;
        private readonly Dictionary<TransformModesTypes, Mode> _modes;

        public TransformModeFactory(GlobalState globalState)
        {
            this._globalState = globalState;

            _modes = new Dictionary<TransformModesTypes, Mode>
            {
                [TransformModesTypes.NO] = new NoMode(globalState),
                [TransformModesTypes.TRANSLATE] = new TransformMode(globalState),
                [TransformModesTypes.ROTATE] = new RotateMode(globalState),
                [TransformModesTypes.SCALE] = new ScaleMode(globalState),
            };
        }

        public Mode CreateMode(Mode old, TransformModesTypes type)
        {
            if (old.type == type)
            {
                return _modes[TransformModesTypes.NO];
            }

            return _modes.TryGetValue(type, out var mode) ? mode : _modes[TransformModesTypes.NO];
        }
    }
}
