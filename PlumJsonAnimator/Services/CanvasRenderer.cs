using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using PlumJsonAnimator.Common.Constants;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.Services;

public class CanvasRenderer
{
    private DispatcherTimer _canvasLoop = new DispatcherTimer();

    public GlobalState _globalState;
    public JsonCode _jsonCode;
    public JsonValidator _jsonValidator;

    public CanvasRenderer(GlobalState globalState, JsonCode jsonCode, JsonValidator jsonValidator)
    {
        _globalState = globalState;
        _jsonCode = jsonCode;
        _jsonValidator = jsonValidator;

        _canvasLoop.Interval = TimeSpan.FromMilliseconds(1000 / _globalState.FPS);
        _canvasLoop.Tick += UpdateCanvas;
        _canvasLoop.Start();
    }

    public void LoopStart()
    {
        _canvasLoop.Start();
    }

    public void LoopStop()
    {
        _canvasLoop.Stop();
    }

    private void UpdateCanvas(object? sender, EventArgs e)
    {
        RedrawCanvas(_globalState.Canvas);
    }

    public void RedrawCanvas(Control canvas, bool? b = null, bool? a = null)
    {
        if (_globalState.CurrentTab == 0)
        {
            canvas?.InvalidateVisual();
            _jsonCode.GenerateCode(_globalState.CurrentProject);
        }
        else if (_globalState.CurrentTab == 1)
        {
            CanGenerateProject();
        }
    }

    public bool CanGenerateProject()
    {
        _globalState.JsonError.ErrorText = _jsonValidator.Validate(
            _globalState.CurrentProject.Code
        );
        if (_globalState.JsonError.IsOk)
        {
            RegenerateResult validateResult = _jsonCode.Regenerate(
                _globalState.CurrentProject,
                false
            );
            if (!validateResult.IsOk)
            {
                _globalState.JsonError.ErrorText = validateResult.Message;
                return false;
            }
        }
        return true;
    }
}
