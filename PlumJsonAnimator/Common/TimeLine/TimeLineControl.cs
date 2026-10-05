using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Threading;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Easing;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Common.Timeline
{
    public class TimelineControl : TemplatedControl, ICustomHitTest
    {
        private const int TickHeight = 5;
        private readonly Dictionary<TransformModeTypes, int> trackTypes = new()
        {
            [TransformModeTypes.TRANSLATE] = 0,
            [TransformModeTypes.ROTATE] = 1,
            [TransformModeTypes.SCALE] = 2,
            [TransformModeTypes.SHEAR] = 3,
        };
        private const double TimelineHeight = 25;
        private const double KeyframeWidth = 6;
        private const double KeyframeHeight = 18;
        private const double BaseBezierDelta = 0.1;
        private const int MouseMoveThreshold = 5;
        private const int MinZoom = 1;
        private const int MaxZoom = 10;

        private double _timeStep;
        private bool _isDraggingPlayhead = false;
        private bool _isSettingBezier = false;
        private double _oldX = 0;
        private double _oldY = 0;
        private bool _isLeft = true;
        private IKeyframeType? _keyframe = null;

        private DispatcherTimer _refreshTimer;

        public TimelineControl()
        {
            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _refreshTimer.Tick += (s, e) => InvalidateVisual();
            _refreshTimer.Start();
        }

        public static readonly StyledProperty<double> PixelsPerSecondProperty =
            AvaloniaProperty.Register<TimelineControl, double>(nameof(PixelsPerSecond), 50.0);

        public double PixelsPerSecond
        {
            get => GetValue(PixelsPerSecondProperty);
            set => SetValue(PixelsPerSecondProperty, value);
        }

        public static readonly StyledProperty<int> ZoomProperty = AvaloniaProperty.Register<
            TimelineControl,
            int
        >(nameof(Zoom), 1, coerce: CoerceZoom);

        private static int CoerceZoom(AvaloniaObject obj, int value)
        {
            return Math.Clamp(value, MinZoom, MaxZoom);
        }

        public static readonly StyledProperty<int> FPSProperty = AvaloniaProperty.Register<
            TimelineControl,
            int
        >(nameof(FPS), 1);

        public static readonly StyledProperty<Bone?> CurrentBoneProperty =
            AvaloniaProperty.Register<TimelineControl, Bone?>(nameof(CurrentBone), null);

        public static readonly StyledProperty<Animation?> CurrentAnimationProperty =
            AvaloniaProperty.Register<TimelineControl, Animation?>(nameof(CurrentAnimation), null);

        public static readonly StyledProperty<Mode?> CurrentModeProperty =
            AvaloniaProperty.Register<TimelineControl, Mode?>(nameof(CurrentMode), null);
        public static readonly StyledProperty<EasingTypes?> CurrentEasingModeProperty =
            AvaloniaProperty.Register<TimelineControl, EasingTypes?>(
                nameof(CurrentEasingMode),
                null
            );

        public int Zoom
        {
            get => GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public int FPS
        {
            get => GetValue(FPSProperty);
            set => SetValue(FPSProperty, value);
        }

        public Bone? CurrentBone
        {
            get => GetValue(CurrentBoneProperty);
            set => SetValue(CurrentBoneProperty, value);
        }

        public Mode? CurrentMode
        {
            get => GetValue(CurrentModeProperty);
            set => SetValue(CurrentModeProperty, value);
        }

        public Animation? CurrentAnimation
        {
            get => GetValue(CurrentAnimationProperty);
            set => SetValue(CurrentAnimationProperty, value);
        }

        public EasingTypes? CurrentEasingMode
        {
            get => GetValue(CurrentEasingModeProperty);
            set => SetValue(CurrentEasingModeProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (
                change.Property == ZoomProperty
                || change.Property == CurrentTimeProperty
                || change.Property == CurrentTimeProperty
            )
            {
                InvalidateVisual();
            }

            if (
                change.Property == CurrentBoneProperty
                || change.Property == CurrentAnimationProperty
                || change.Property == CurrentModeProperty
                || change.Property == CurrentTimeProperty
            )
            {
                if (CurrentMode is null || CurrentBone is null)
                    return;

                var keyFrame = CurrentAnimation?.FindKeyFrameByTime(
                    CurrentBone,
                    CurrentMode.Type,
                    CurrentTime
                );

                CurrentEasingMode = keyFrame?.Curve?.EasingType;
            }
        }

        public static readonly StyledProperty<ObservableCollection<TimelineTrack>> TracksProperty =
            AvaloniaProperty.Register<TimelineControl, ObservableCollection<TimelineTrack>>(
                nameof(Tracks),
                new ObservableCollection<TimelineTrack>()
            );

        public ObservableCollection<TimelineTrack> Tracks
        {
            get => GetValue(TracksProperty);
            set => SetValue(TracksProperty, value);
        }

        public static readonly StyledProperty<double> CurrentTimeProperty =
            AvaloniaProperty.Register<TimelineControl, double>(nameof(CurrentTime), 0.0);

        public double CurrentTime
        {
            get => GetValue(CurrentTimeProperty);
            set { SetValue(CurrentTimeProperty, value); }
        }

        static TimelineControl()
        {
            CurrentTimeProperty.Changed.AddClassHandler<TimelineControl>(
                (sender, args) => sender.InvalidateVisual()
            );
            TracksProperty.Changed.AddClassHandler<TimelineControl>(
                (sender, args) => sender.InvalidateVisual()
            );
            PixelsPerSecondProperty.Changed.AddClassHandler<TimelineControl>(
                (sender, args) => sender.InvalidateMeasure()
            );
        }

        public static readonly StyledProperty<double> TotalDurationProperty =
            AvaloniaProperty.Register<TimelineControl, double>(nameof(TotalDuration), 10.0);

        public double TotalDuration
        {
            get => GetValue(TotalDurationProperty);
            set => SetValue(TotalDurationProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double desiredWidth = TotalDuration * PixelsPerSecond * Zoom;

            double desiredHeight = availableSize.Height;

            return new Size(desiredWidth, desiredHeight);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _refreshTimer?.Stop();
        }

        bool ICustomHitTest.HitTest(Point point)
        {
            return new Rect(0, 0, Bounds.Width, Bounds.Height).Contains(point);
        }

        // TODO: fix drawing keyframe lines
        public override void Render(DrawingContext context)
        {
            // --- 1. Основные переменные ---
            // desiredWidth: Полная ширина (6000px, если 60s * 100px/s)
            double desiredWidth = TotalDuration * PixelsPerSecond * Zoom;
            // height: Фактическая высота в окне (например, 150px)
            double height = Bounds.Height;
            double duration = TotalDuration;
            int trackCount = Tracks.Count;

            var linePen = new Pen(Brushes.Gray, 1);
            var redPen = new Pen(Brushes.Red, 2);

            // --- 3. Отрисовка главной горизонтальной линии ---
            // Используем desiredWidth

            // ----------------------------------------------------------------------------------
            // 4. Отрисовка Бегунка (Playhead) - ДО отрисовки дорожек
            // ----------------------------------------------------------------------------------
            if (duration > 0)
            {
                // ИСПОЛЬЗУЕМ desiredWidth для расчета X-позиции
                double playheadX = CurrentTime * PixelsPerSecond * Zoom;

                // Рисуем вертикальную линию бегунка (на всю высоту height)
                context.DrawLine(redPen, new Point(playheadX, 0), new Point(playheadX, height));

                // Отрисовка треугольника бегунка
                var triangleGeometry = new PolylineGeometry(
                    new[]
                    {
                        new Point(playheadX, 0),
                        new Point(playheadX - 5, 5),
                        new Point(playheadX + 5, 5),
                    },
                    true
                );
                context.DrawGeometry(Brushes.Red, null, triangleGeometry);
            }

            // ----------------------------------------------------------------------------------
            // 5. Расчет и отрисовка Дорожек
            // ----------------------------------------------------------------------------------

            // Высота, доступная для всех дорожек
            double availableTrackHeight = height - TimelineHeight;

            // Высота одной дорожки (делим на количество дорожек)
            double trackRowHeight = availableTrackHeight / Math.Max(1, trackCount) - 5;

            for (int i = 0; i < trackCount; i++)
            {
                // Y-позиция центра дорожки:
                // Начинаем с конца шкалы (timelineHeight) + смещение на текущий ряд + половина высоты ряда
                double yPosition = TimelineHeight + (i * trackRowHeight) + (trackRowHeight / 2.0);

                // Рисуем линию дорожки (используем desiredWidth)
                context.DrawLine(
                    linePen,
                    new Point(0, yPosition),
                    new Point(desiredWidth, yPosition)
                );
            }

            // ----------------------------------------------------------------------------------
            // 6. Расчет и отрисовка Меток Времени
            // ----------------------------------------------------------------------------------

            double step = 1.0 / FPS;
            this._timeStep = step;

            for (double t = 0; t <= duration; t += step)
            {
                // Используем desiredWidth
                double xPosition = PixelsPerSecond * t * Zoom;

                // Высота метки: 8px для основных (каждые 5 сек), 5px для промежуточных
                double tickHeight = TickHeight;

                // Рисуем вертикальную метку: от midlineY вверх/вниз
                context.DrawLine(
                    linePen,
                    new Point(xPosition, 0),
                    new Point(xPosition, tickHeight)
                );
            }

            // ----------------------------------------------------------------------------------
            // 7. Отрисовка ключкадров
            // ----------------------------------------------------------------------------------
            KeyValuePair<
                Bone,
                Dictionary<double, Dictionary<TransformModeTypes, bool>>
            >? currentBoneKeyFrames = null;

            if (CurrentAnimation is null)
                return;

            foreach (var boneKeyFrames in CurrentAnimation.GetAllKeyFrameMarks())
            {
                var opacity = 0.2;

                // TODO: сравнение не работает
                if (CurrentBone == boneKeyFrames.Key && currentBoneKeyFrames != null)
                {
                    opacity = 1.0;
                }

                DrawBoneKeyframes(context, boneKeyFrames, opacity, TimelineHeight, trackRowHeight);
            }

            if (
                CurrentBone != null
                && !(CurrentMode is null)
                && CurrentMode.Type != TransformModeTypes.NO
            )
            {
                var keyFrameLine = CurrentAnimation.GetKeyFrameLine(CurrentMode, CurrentBone);
                DrawKeyFrameLine(context, keyFrameLine);
            }
        }

        private void DrawKeyFrameLine(
            DrawingContext context,
            SortedDictionary<double, IKeyframeType>? keyFrameLine
        )
        {
            if (keyFrameLine is null)
                return;

            var keys = keyFrameLine.Keys.ToList();

            var rowIndex = trackTypes[CurrentMode!.Type];
            double availableTrackHeight = Bounds.Height - TimelineHeight;
            double trackRowHeight = availableTrackHeight / Math.Max(1, trackTypes.Count);
            int startY = (int)(
                TimelineHeight
                + (trackTypes[CurrentMode.Type] * trackRowHeight)
                + (trackRowHeight / 2.0)
            );

            int endY = (int)(
                TimelineHeight
                + (trackTypes[CurrentMode.Type] * trackRowHeight)
                - (trackRowHeight / 2.0)
            );

            for (int i = 0; i < keys.Count - 1; i++)
            {
                var currentKeyFrame = keyFrameLine[keys[i]];

                var startX = (int)(PixelsPerSecond * keys[i] * Zoom);
                var endX = (int)(PixelsPerSecond * keys[i + 1] * Zoom);

                var color = CurrentBone!.BoneColor;

                SolidColorBrush fillBrush = new SolidColorBrush(color.Color);
                currentKeyFrame.Curve.DrawLine(
                    context,
                    new PointModel(startX, startY),
                    new PointModel(endX, endY),
                    fillBrush
                );
            }
        }

        private void DrawBoneKeyframes(
            DrawingContext context,
            KeyValuePair<
                Bone,
                Dictionary<double, Dictionary<TransformModeTypes, bool>>
            > boneKeyFrames,
            double opacity,
            double timelineHeight,
            double trackRowHeight
        )
        {
            var color = boneKeyFrames.Key.BoneColor;
            var keyframesMarks =
                boneKeyFrames.Value
                ?? new Dictionary<double, Dictionary<TransformModeTypes, bool>>();

            SolidColorBrush fillBrush = new SolidColorBrush(color.Color) { Opacity = opacity };

            foreach (double time in keyframesMarks.Keys)
            {
                double xPosition = PixelsPerSecond * time * Zoom;

                foreach (var (type, rowIndex) in trackTypes)
                {
                    if (keyframesMarks[time].ContainsKey(type))
                    {
                        double yPosition = timelineHeight + (rowIndex * trackRowHeight);

                        var rect = new Rect(
                            xPosition - KeyframeWidth / 2,
                            yPosition - KeyframeHeight / 2,
                            KeyframeWidth,
                            KeyframeHeight
                        );

                        context.FillRectangle(fillBrush, rect);
                    }
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pos = e.GetCurrentPoint(this).Position;

            if (pos.Y > TickHeight)
            {
                if (CurrentEasingMode == EasingTypes.BEZIER)
                {
                    var time = pos.X / (PixelsPerSecond * Zoom);
                    _keyframe = CurrentAnimation.FindKeyFrameByTime(
                        CurrentBone,
                        CurrentMode.Type,
                        time
                    );
                    var nextTime = CurrentAnimation.FindNextTime(
                        time,
                        CurrentBone,
                        CurrentMode.Type
                    );
                    double? currTime = CurrentAnimation.FindKeyFrameTime(
                        time,
                        CurrentBone,
                        CurrentMode.Type
                    );
                    if (nextTime != null && currTime != null)
                    {
                        double currX = (double)currTime * PixelsPerSecond * Zoom;
                        double nextX = (double)nextTime * PixelsPerSecond * Zoom;

                        if (Math.Abs(pos.X - nextX) < Math.Abs(pos.X - currX))
                        {
                            _isLeft = false;
                        }
                    }
                    _isSettingBezier = true;
                    _oldX = pos.X;
                    _oldY = pos.Y;
                }
                e.Handled = true;
                return;
            }

            double playheadX = CurrentTime * PixelsPerSecond * Zoom;

            if (Math.Abs(pos.X - playheadX) < 10)
            {
                _isDraggingPlayhead = true;
                e.Handled = true;
            }
            else
            {
                SetCurrentTime(pos.X, pos.Y);
                e.Handled = true;
                InvalidateVisual();
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            var pos = e.GetCurrentPoint(this).Position;

            if (_isDraggingPlayhead)
            {
                SetCurrentTime(pos.X, pos.Y);
                e.Handled = true;
                InvalidateVisual();
            }

            if (_isSettingBezier)
            {
                var newY = pos.Y;
                var newX = pos.X;

                double deltaX = BaseBezierDelta;
                double deltaY = BaseBezierDelta;

                if (
                    Math.Abs(newX - _oldX) > MouseMoveThreshold
                    || Math.Abs(newY - _oldY) > MouseMoveThreshold
                )
                {
                    if (newY > _oldY)
                    {
                        deltaY = -deltaY;
                    }
                    if (newX < _oldX)
                    {
                        deltaX = -deltaX;
                    }

                    if (_keyframe?.Curve != null)
                    {
                        BezierEasing bezierEasing = (BezierEasing)_keyframe.Curve;
                        bezierEasing.UpdateKeys(deltaX, deltaY, _isLeft);
                    }

                    _oldX = newX;
                    _oldY = newY;
                }
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e.Pointer.Captured == this)
            {
                e.Pointer.Capture(null);
            }

            if (_isDraggingPlayhead)
            {
                _isDraggingPlayhead = false;
                e.Handled = true;

                if (FPS > 0)
                {
                    double frameDuration = 1.0 / FPS;
                    int frameNumber = (int)Math.Round(CurrentTime / frameDuration);
                    CurrentTime = frameNumber * frameDuration;
                    CurrentTime = Math.Round(CurrentTime / this._timeStep) * this._timeStep;

                    InvalidateVisual();
                }
            }

            if (_isSettingBezier)
            {
                _isSettingBezier = false;
                _oldX = 0;
                _oldY = 0;
                _keyframe = null;
                _isLeft = true;
                e.Handled = true;
            }
        }

        private void SetCurrentTime(double x, double y)
        {
            double newX = x;
            double newY = y;

            if (newY > TickHeight)
            {
                return;
            }

            double calculatedWidth = TotalDuration * PixelsPerSecond * Zoom;
            newX = Math.Clamp(newX, 0, calculatedWidth);
            double newTime = (newX / calculatedWidth) * TotalDuration;

            if (FPS > 0)
            {
                double frameDuration = 1.0 / FPS;
                int frameNumber = (int)Math.Round(newTime / frameDuration);
                newTime = frameNumber * frameDuration;
            }
            newTime = Math.Clamp(newTime, 0, TotalDuration);
            newTime = Math.Round(newTime / _timeStep) * _timeStep;
            CurrentTime = newTime;
        }
    }
}
