using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.Services;

public class AutoSaver
{
    private PeriodicTimer? _autoSaveTimer;
    private CancellationTokenSource? _autoSaveCts;

    private readonly GlobalState GlobalState;
    private readonly JsonCode jsonCode;
    private readonly ProjectSettings projectSettings;

    public AutoSaver(GlobalState globalState, JsonCode jsonCode, ProjectSettings projectSettings)
    {
        this.GlobalState = globalState;
        this.jsonCode = jsonCode;
        this.projectSettings = projectSettings;
    }

    public async Task StartAutoSaveAsync(long seconds)
    {
        _autoSaveCts = new CancellationTokenSource();
        _autoSaveTimer = new PeriodicTimer(TimeSpan.FromSeconds(seconds));

        while (await _autoSaveTimer.WaitForNextTickAsync(_autoSaveCts.Token))
        {
            if (GlobalState.isAutoSave)
            {
                string project = JsonConvert.SerializeObject(
                    this.jsonCode.generateJSONData(GlobalState.CurrentProject),
                    GlobalState.jsonSettings
                );
                projectSettings.WriteAutoSave(project);
                GlobalState.LastSaveTime = DateTime.Now;
            }
        }
    }

    public void StopAutoSave()
    {
        _autoSaveCts?.Cancel();
        _autoSaveTimer?.Dispose();
    }
}
