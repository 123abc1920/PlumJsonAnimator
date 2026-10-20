using System;
using System.Threading;
using System.Threading.Tasks;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Services;

public class AutoSaver
{
    private PeriodicTimer? _autoSaveTimer;
    private CancellationTokenSource? _autoSaveCts;

    private readonly GlobalState _globalState;
    private readonly JsonCode _jsonCode;

    public AutoSaver(GlobalState globalState, JsonCode jsonCode)
    {
        _globalState = globalState;
        _jsonCode = jsonCode;
    }

    public async Task StartAutoSaveAsync(long seconds)
    {
        _autoSaveCts = new CancellationTokenSource();
        _autoSaveTimer = new PeriodicTimer(TimeSpan.FromSeconds(seconds));

        while (await _autoSaveTimer.WaitForNextTickAsync(_autoSaveCts.Token))
        {
            if (_globalState.isAutoSave)
            {
                _globalState.CurrentProject?.AutoSaveProjectSettings(_jsonCode);
                _globalState.LastSaveTime = DateTime.Now;
            }
        }
    }

    public void StopAutoSave()
    {
        _autoSaveCts?.Cancel();
        _autoSaveTimer?.Dispose();
    }
}
