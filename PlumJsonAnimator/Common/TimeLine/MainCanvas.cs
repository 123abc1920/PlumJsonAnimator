using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.Common.Timeline;

public class MainCanvas : Control
{
    public static readonly StyledProperty<GlobalState?> GlobalStateProperty =
        AvaloniaProperty.Register<MainCanvas, GlobalState?>(nameof(GlobalState));

    public GlobalState? GlobalState
    {
        get => GetValue(GlobalStateProperty);
        set => SetValue(GlobalStateProperty, value);
    }

    public static readonly StyledProperty<JsonValidator?> JsoValidatorProperty =
        AvaloniaProperty.Register<MainCanvas, JsonValidator?>(nameof(JsonValidator));

    public JsonValidator? JsonValidator
    {
        get => GetValue(JsoValidatorProperty);
        set => SetValue(JsoValidatorProperty, value);
    }

    public static readonly StyledProperty<JsonCode?> JsonCodeProperty = AvaloniaProperty.Register<
        MainCanvas,
        JsonCode?
    >(nameof(JsonCode));

    public JsonCode? JsonCode
    {
        get => GetValue(JsonCodeProperty);
        set => SetValue(JsonCodeProperty, value);
    }

    static MainCanvas()
    {
        AffectsRender<MainCanvas>(GlobalStateProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.FillRectangle(Brushes.Transparent, new Rect(0, 0, Bounds.Width, Bounds.Height));

        if (GlobalState == null)
            return;

        using (
            context.PushTransform(Matrix.CreateTranslation(Bounds.Width / 2.0, Bounds.Height / 2.0))
        )
        {
            RedrawCanvas(context, GlobalState.DrawBones, GlobalState.CaptureMode);
            GlobalState.SelectedItem?.Draw(context);
        }
    }

    public void RedrawCanvas(DrawingContext context, bool isDrawBone, bool isDrawCapture)
    {
        GlobalState?.CurrentProject?.DrawSlots(context);
        GlobalState?.CurrentProject?.MainSkeleton?.RecalculateBones();
        if (isDrawBone)
        {
            GlobalState?.CurrentProject?.MainSkeleton?.DrawSkeleton(context);
        }
        if (isDrawCapture)
        {
            GlobalState?.CaptureArea?.DrawCaptureArea(context);
        }
    }
}
