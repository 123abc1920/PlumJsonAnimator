using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Common.Constants;
using Common.Constants.CommonModels;

namespace Common.TimeLine
{
    public class TimelineControl : Control
    {
        private double timeStep;
        private bool _isDraggingPlayhead = false;
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
            const int minZoom = 1;
            const int maxZoom = 10;
            return Math.Clamp(value, minZoom, maxZoom);
        }

        public int Zoom
        {
            get => GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ZoomProperty || change.Property == CurrentTimeProperty)
            {
                InvalidateVisual();
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
            set
            {
                ConstantsClass.currentProject.CurrentAnimation.currentTime = value;
                ConstantsClass.currentProject.CurrentAnimation.SetupBones();
                SetValue(CurrentTimeProperty, value);
            }
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
            // Расчет ширины
            double desiredWidth = TotalDuration * PixelsPerSecond * Zoom;

            // Высота: если Height не установлен (т.е. NaN), используем availableSize.Height.
            double desiredHeight = availableSize.Height;

            return new Size(desiredWidth, desiredHeight);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _refreshTimer?.Stop();
        }

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

            // ... (Отрисовка фона) ...

            // --- 2. Константы высоты ---
            // Увеличим высоту для меток времени, чтобы они не накладывались на дорожки
            double timelineHeight = 25;
            double midlineY = timelineHeight; // Сдвинем основную линию шкалы вниз

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
            double availableTrackHeight = height - timelineHeight;

            // Высота одной дорожки (делим на количество дорожек)
            double trackRowHeight = availableTrackHeight / Math.Max(1, trackCount);

            for (int i = 0; i < trackCount; i++)
            {
                // Y-позиция центра дорожки:
                // Начинаем с конца шкалы (timelineHeight) + смещение на текущий ряд + половина высоты ряда
                double yPosition = timelineHeight + (i * trackRowHeight) + (trackRowHeight / 2.0);

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

            int numIntervals = (int)Math.Ceiling(duration); // Используем Ceil, чтобы не обрезать последнюю метку

            double step = 1.0 / (double)ConstantsClass.FPS;
            this.timeStep = step;

            for (double t = 0; t <= duration; t += step)
            {
                // Используем desiredWidth
                double xPosition = PixelsPerSecond * t * Zoom;

                // Высота метки: 8px для основных (каждые 5 сек), 5px для промежуточных
                double tickHeight = 5;

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
            Dictionary<double, Dictionary<KeyFrameTypes, bool>> keyframesMarks =
                ConstantsClass.currentProject?.CurrentAnimation?.GetKeyFramesMarks(
                    ConstantsClass.currentBone
                );

            if (keyframesMarks == null)
            {
                keyframesMarks = new Dictionary<double, Dictionary<KeyFrameTypes, bool>>();
            }

            const double KeyframeWidth = 6;
            const double KeyframeHeight = 18;
            SolidColorBrush fillBrush = new SolidColorBrush(Colors.Red);

            foreach (double time in keyframesMarks.Keys)
            {
                double xPosition = PixelsPerSecond * time * Zoom;

                if (keyframesMarks[time].ContainsKey(KeyFrameTypes.TRANSLATE))
                {
                    double yPosition = timelineHeight + (0 * trackRowHeight);

                    // Создаем прямоугольник
                    var rect = new Rect(
                        xPosition - KeyframeWidth / 2, // Центрируем по X
                        yPosition - KeyframeHeight / 2, // Центрируем по Y
                        KeyframeWidth,
                        KeyframeHeight
                    );

                    context.FillRectangle(fillBrush, rect);
                    context.DrawRectangle(redPen, rect);
                }

                if (keyframesMarks[time].ContainsKey(KeyFrameTypes.ROTATE))
                {
                    double yPosition = timelineHeight + (1 * trackRowHeight);

                    var rect = new Rect(
                        xPosition - KeyframeWidth / 2,
                        yPosition - KeyframeHeight / 2,
                        KeyframeWidth,
                        KeyframeHeight
                    );

                    context.FillRectangle(fillBrush, rect);
                    context.DrawRectangle(redPen, rect);
                }

                if (keyframesMarks[time].ContainsKey(KeyFrameTypes.SCALE))
                {
                    double yPosition = timelineHeight + (2 * trackRowHeight);

                    var rect = new Rect(
                        xPosition - KeyframeWidth / 2,
                        yPosition - KeyframeHeight / 2,
                        KeyframeWidth,
                        KeyframeHeight
                    );

                    context.FillRectangle(fillBrush, rect);
                    context.DrawRectangle(redPen, rect);
                }
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pos = e.GetCurrentPoint(this).Position;

            // ИСПОЛЬЗУЕМ desiredWidth (Полная ширина шкалы)
            double desiredWidth = TotalDuration * PixelsPerSecond * Zoom;

            // Расчет X-позиции бегунка на полной шкале
            double playheadX = CurrentTime * PixelsPerSecond * Zoom;

            // Если указатель находится в пределах 10px от бегунка
            if (Math.Abs(pos.X - playheadX) < 10)
            {
                _isDraggingPlayhead = true;
                e.Handled = true;
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (_isDraggingPlayhead)
            {
                var pos = e.GetCurrentPoint(this).Position;
                double newX = pos.X;

                // ИСПОЛЬЗУЕМ desiredWidth (Полная ширина шкалы)
                double calculatedWidth = TotalDuration * PixelsPerSecond * Zoom;

                // Ограничиваем X-координату в пределах полной шкалы
                newX = Math.Clamp(newX, 0, calculatedWidth);

                // Расчет нового времени (должен использовать calculatedWidth)
                double newTime = (newX / calculatedWidth) * TotalDuration;

                // ПРИВЯЗЫВАЕМ К КАДРАМ
                if (ConstantsClass.FPS > 0)
                {
                    double frameDuration = 1.0 / ConstantsClass.FPS;
                    int frameNumber = (int)Math.Round(newTime / frameDuration);
                    newTime = frameNumber * frameDuration;
                }

                // Еще раз ограничиваем после привязки
                newTime = Math.Clamp(newTime, 0, TotalDuration);

                // УСТАНАВЛИВАЕМ CurrentTime - бегунок подтянется сам при отрисовке
                newTime = Math.Round(newTime / this.timeStep) * timeStep;
                CurrentTime = newTime;

                e.Handled = true;

                // Обновляем отображение
                InvalidateVisual();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_isDraggingPlayhead)
            {
                _isDraggingPlayhead = false;
                e.Handled = true;

                if (ConstantsClass.FPS > 0)
                {
                    double frameDuration = 1.0 / ConstantsClass.FPS;
                    int frameNumber = (int)Math.Round(CurrentTime / frameDuration);
                    CurrentTime = frameNumber * frameDuration;
                    CurrentTime = Math.Round(CurrentTime / this.timeStep) * this.timeStep;

                    InvalidateVisual();
                }
            }
        }
    }
}
