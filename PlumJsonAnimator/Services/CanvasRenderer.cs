using System;
using Avalonia.Controls;
using Avalonia.Threading;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.Services;

public class CanvasRenderer
{
    private DispatcherTimer _canvasLoop = new DispatcherTimer();

    public GlobalState globalState;
    public JsonCode jsonCode;
    public JsonValidator jsonValidator;

    public CanvasRenderer(GlobalState globalState, JsonCode jsonCode, JsonValidator jsonValidator)
    {
        this.globalState = globalState;
        this.jsonCode = jsonCode;
        this.jsonValidator = jsonValidator;

        _canvasLoop.Interval = TimeSpan.FromMilliseconds(1000 / this.globalState.FPS);
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
        if (this.globalState.currentTab == 0)
        {
            RedrawCanvas(
                this.globalState.canvas,
                this.globalState.drawBones,
                this.globalState.captureMode
            );
        }
        else if (this.globalState.currentTab == 1)
        {
            CanGenerateProject();
        }
    }

    public void RedrawCanvas(Canvas canvas, bool isDrawBone, bool isDrawCapture)
    {
        canvas?.Children.Clear();
        this.globalState.CurrentProject?.DrawSlots(canvas);
        if (isDrawBone)
        {
            this.globalState.CurrentProject?.MainSkeleton?.DrawSkeleton(canvas);
            this.jsonCode.generateCode(this.globalState.CurrentProject);
        }
        if (isDrawCapture)
        {
            this.globalState.captureArea?.DrawCaptureArea(canvas);
        }
    }

    public bool CanGenerateProject()
    {
        this.globalState.jsonError.ErrorText = this.jsonValidator.Validate(
            this.globalState.CurrentProject.Code
        );
        if (this.globalState.jsonError.IsOk)
        {
            ValidResult validateResult = this.jsonCode.Regenerate(
                this.globalState.CurrentProject,
                false
            );
            if (!validateResult.IsOk)
            {
                this.globalState.jsonError.ErrorText = validateResult.Message;
                return false;
            }
        }
        return true;
    }
}
