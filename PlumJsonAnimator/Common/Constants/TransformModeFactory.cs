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
        private readonly Dictionary<TransformModeTypes, Mode> _modes;

        public TransformModeFactory(GlobalState globalState)
        {
            this._globalState = globalState;

            _modes = new Dictionary<TransformModeTypes, Mode>
            {
                [TransformModeTypes.NO] = new NoMode(globalState),
                [TransformModeTypes.TRANSLATE] = new TransformMode(globalState),
                [TransformModeTypes.ROTATE] = new RotateMode(globalState),
                [TransformModeTypes.SCALE] = new ScaleMode(globalState),
                [TransformModeTypes.SHEAR] = new ShearMode(globalState),
            };
        }

        public Mode CreateMode(Mode old, TransformModeTypes type)
        {
            if (old.Type == type)
            {
                return _modes[TransformModeTypes.NO];
            }

            return _modes.TryGetValue(type, out var mode) ? mode : _modes[TransformModeTypes.NO];
        }
    }
}
