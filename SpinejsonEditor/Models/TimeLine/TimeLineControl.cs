using System;
using System.Collections.ObjectModel;
using AnimEngine;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.VisualTree;
using Constants;

namespace TimeLine
{
    public class TimelineControl : Control
    {
        private bool _isDraggingPlayhead = false;

        public static readonly StyledProperty<double> PixelsPerSecondProperty =
            AvaloniaProperty.Register<TimelineControl, double>(nameof(PixelsPerSecond), 50.0);

        public double PixelsPerSecond
        {
            get => GetValue(PixelsPerSecondProperty);
            set => SetValue(PixelsPerSecondProperty, value);
        }

        public static readonly StyledProperty<ObservableCollection<TimelineTrack>> TracksProperty =
            AvaloniaProperty.Register<TimelineControl, ObservableCollection<TimelineTrack>>(
                nameof(Tracks),
                new ObservableCollection<TimelineTrack>() // Инициализация пустой коллекцией
            );

        public ObservableCollection<TimelineTrack> Tracks
        {
            get => GetValue(TracksProperty);
            set => SetValue(TracksProperty, value);
        }

        // Свойство для текущей позиции бегунка (от 0 до TotalDuration)
        public static readonly StyledProperty<double> CurrentTimeProperty =
            AvaloniaProperty.Register<TimelineControl, double>(nameof(CurrentTime), 0.0);

        public double CurrentTime
        {
            get => GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
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
                (sender, args) => sender.InvalidateMeasure() // Важно: Affects Measure
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
            double desiredWidth = TotalDuration * PixelsPerSecond;

            // Высота: если Height не установлен (т.е. NaN), используем availableSize.Height.
            double desiredHeight = double.IsNaN(Height) ? availableSize.Height : Height;

            // Важно: Avalonia не любит, когда MeasureOverride возвращает Infinity или NaN.
            // Ограничиваем высоту, если она бесконечна (хотя Grid должен это предотвратить).
            if (double.IsInfinity(desiredHeight))
            {
                // Предоставляем разумное значение по умолчанию, если высота не ограничена.
                desiredHeight = 150;
            }

            return new Size(desiredWidth, desiredHeight);
        }

        public override void Render(DrawingContext context)
        {
            // --- 1. Основные переменные ---
            // desiredWidth: Полная ширина (6000px, если 60s * 100px/s)
            double desiredWidth = TotalDuration * PixelsPerSecond;
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
                double playheadX = (CurrentTime / duration) * desiredWidth;

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

            // Высота, доступная для всех дорожек (ВСЕ, ЧТО НИЖЕ timelineHeight)
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

                // Тут можно добавить отрисовку имени дорожки
            }

            // ----------------------------------------------------------------------------------
            // 6. Расчет и отрисовка Меток Времени
            // ----------------------------------------------------------------------------------

            int numIntervals = (int)Math.Ceiling(duration); // Используем Ceil, чтобы не обрезать последнюю метку

            // Шаг отрисовки: 1 секунда (для мелких меток)
            double step = 1.0;

            for (double t = 0; t <= duration; t += step)
            {
                // Используем desiredWidth
                double xPosition = PixelsPerSecond * t;

                // Высота метки: 8px для основных (каждые 5 сек), 5px для промежуточных
                double tickHeight = (t % 5 == 0) ? 8 : 5;

                // Рисуем вертикальную метку: от midlineY вверх/вниз
                context.DrawLine(
                    linePen,
                    new Point(xPosition, 0),
                    new Point(xPosition, tickHeight)
                );
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pos = e.GetCurrentPoint(this).Position;

            // ✅ ИСПОЛЬЗУЕМ desiredWidth (Полная ширина шкалы)
            double desiredWidth = TotalDuration * PixelsPerSecond;

            // Расчет X-позиции бегунка на полной шкале
            double playheadX = (CurrentTime / TotalDuration) * desiredWidth;

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

                // ✅ ИСПОЛЬЗУЕМ desiredWidth (Полная ширина шкалы)
                double calculatedWidth = TotalDuration * PixelsPerSecond;

                // Ограничиваем X-координату в пределах полной шкалы
                newX = Math.Clamp(newX, 0, calculatedWidth);

                // Расчет нового времени (должен использовать calculatedWidth)
                double newTime = (newX / calculatedWidth) * TotalDuration;

                CurrentTime = newTime;
                e.Handled = true;
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_isDraggingPlayhead)
            {
                _isDraggingPlayhead = false;
                e.Handled = true;
            }

            ConstantsClass.currentProject.GetAnimation().currentTime = CurrentTime;
        }
    }
}
