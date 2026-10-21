using System;
using Avalonia.Controls;
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
        if (_globalState.CurrentTab == 0)
        {
            RedrawCanvas(
                _globalState.Canvas,
                _globalState.DrawBones,
                _globalState.CaptureMode
            );
        }
        else if (_globalState.CurrentTab == 1)
        {
            CanGenerateProject();
        }
    }

    public void RedrawCanvas(Canvas canvas, bool isDrawBone, bool isDrawCapture)
    {
        canvas?.Children.Clear();
        _globalState.CurrentProject?.DrawSlots(canvas);
        _globalState.CurrentProject?.MainSkeleton?.RecalculateBones();
        if (isDrawBone)
        {
            _globalState.CurrentProject?.MainSkeleton?.DrawSkeleton(canvas);
            _jsonCode.GenerateCode(_globalState.CurrentProject);
        }
        if (isDrawCapture)
        {
            _globalState.CaptureArea?.DrawCaptureArea(canvas);
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
