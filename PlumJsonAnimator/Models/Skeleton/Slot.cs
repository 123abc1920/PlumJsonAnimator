using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    /// <summary>
    /// Provides methods for work with slots
    /// </summary>
    public class Slot : Bone, IRenamable
    {
        public override bool IsBone
        {
            get { return false; }
        }

        // Приватные поля для локальных значений
        private double _localX = 0;
        private double _localY = 0;
        private double _localA = 0;

        [Reactive]
        public Attachment? CurrentAttachment { get; set; }

        public override double X
        {
            get => BoundedBone != null ? BoundedBone.X + _localX : _localX;
            set
            {
                _localX = BoundedBone != null ? value - BoundedBone.X : value;
                this.RaisePropertyChanged(nameof(X));
            }
        }

        public override double Y
        {
            get => BoundedBone != null ? BoundedBone.Y + _localY : _localY;
            set
            {
                _localY = BoundedBone != null ? value - BoundedBone.Y : value;
                this.RaisePropertyChanged(nameof(Y));
            }
        }

        public override double A
        {
            get => BoundedBone != null ? BoundedBone.A + _localA : _localA;
            set
            {
                _localA = BoundedBone != null ? value - BoundedBone.A : value;
                this.RaisePropertyChanged(nameof(A));
            }
        }

        /// <summary>
        /// Sets actual attachment to slot according current skin
        /// </summary>
        public void UpdateAttachment()
        {
            CurrentAttachment = _globalState.CurrentProject!.CurrentSkin.GetAttachment(this);
            if (CurrentAttachment != null && BoundedBone != null)
            {
                _localX = CurrentAttachment.x;
                _localY = CurrentAttachment.y;
                _localA = CurrentAttachment.a;

                var size = CurrentAttachment.GetSize();
                LengthX = size["width"] ?? LengthX;
                LengthY = size["height"] ?? LengthY;

                // Уведомляем об изменениях
                this.RaisePropertyChanged(nameof(X));
                this.RaisePropertyChanged(nameof(Y));
                this.RaisePropertyChanged(nameof(A));
            }
        }

        public SortedDictionary<double, DrawOrderOffset> drawOrders =
            new SortedDictionary<double, DrawOrderOffset>();

        public bool isUpdatingFromCode;

        [Reactive]
        public int CurrentDrawOrderOffset { get; set; }

        /// <summary>
        /// Updates draw order offset according current animation time
        /// </summary>
        public void UpdateDrawOrderOffset()
        {
            double currTime = this._globalState.CurrentProject.CurrentAnimation.currentTime;

            double? foundKey = null;
            foreach (var key in drawOrders.Keys)
            {
                if (key <= currTime)
                    foundKey = key;
                else
                    break;
            }

            var value = foundKey.HasValue ? drawOrders[foundKey.Value] : null;
            if (value != null)
            {
                isUpdatingFromCode = true;
                this.CurrentDrawOrderOffset = value.Offset;
                isUpdatingFromCode = false;
            }
            else
            {
                isUpdatingFromCode = true;
                this.CurrentDrawOrderOffset = 0;
                isUpdatingFromCode = false;
            }
        }

        private double _lengthX = 100;
        public override double LengthX
        {
            get => _lengthX;
            set
            {
                if (_lengthX != value && value > 0)
                {
                    this.RaiseAndSetIfChanged(ref _lengthX, value);
                }
            }
        }

        private double _lengthY = 100;
        public override double LengthY
        {
            get => _lengthY;
            set
            {
                if (_lengthY != value && value > 0)
                {
                    this.RaiseAndSetIfChanged(ref _lengthY, value);
                }
            }
        }

        private Bone? _boundedBone;
        public Bone? BoundedBone
        {
            get => _boundedBone;
            set
            {
                if (_boundedBone != value)
                {
                    _boundedBone = value;
                    if (value != null)
                    {
                        Move(value.GlobalX + this.X, value.GlobalY + this.Y);
                    }
                    this.RaisePropertyChanged();
                }
            }
        }

        public double GlobalX
        {
            get
            {
                double globalX = _localX;

                if (BoundedBone != null)
                {
                    double rad = BoundedBone.GlobalA * Math.PI / 180;
                    double rotatedX = _localX * Math.Cos(rad) - _localY * Math.Sin(rad);
                    globalX = BoundedBone.GlobalX + rotatedX;
                }

                return globalX;
            }
        }

        public double GlobalY
        {
            get
            {
                double globalY = _localY;

                if (BoundedBone != null)
                {
                    double rad = BoundedBone.GlobalA * Math.PI / 180;
                    double rotatedY = _localX * Math.Sin(rad) + _localY * Math.Cos(rad);
                    globalY = BoundedBone.GlobalY + rotatedY;
                }

                return globalY;
            }
        }

        public double GlobalA
        {
            get
            {
                double globalAngle = _localA;

                if (BoundedBone != null)
                {
                    globalAngle = BoundedBone.GlobalA + _localA;
                }

                return globalAngle;
            }
        }

        private Slot(GlobalState _globalState)
        {
            this.WhenAnyValue(x => x.CurrentDrawOrderOffset)
                .Where(_ => !isUpdatingFromCode)
                .Subscribe(value =>
                {
                    if (_globalState?.CurrentProject?.CurrentAnimation == null)
                        return;

                    double currTime = _globalState.CurrentProject.CurrentAnimation.currentTime;
                    if (drawOrders.ContainsKey(currTime))
                    {
                        drawOrders[currTime].Offset = value;
                    }
                    else
                    {
                        drawOrders.Add(
                            currTime,
                            new DrawOrderOffset() { Slot = Name, Offset = value }
                        );
                    }
                });

            this.WhenAnyValue(x => x.BoundedBone)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(X));
                    this.RaisePropertyChanged(nameof(Y));
                    this.RaisePropertyChanged(nameof(A));
                });
        }

        public Slot(GlobalState globalState, int id, string path)
            : this(globalState)
        {
            this.id = id;
            this.A = 0;
            this.X = 0;
            this.Y = 0;

            this.Name = $"{Path.GetFileNameWithoutExtension(path)}{Counter.GenerateNamePostfix()}";

            this._globalState = globalState;
            UpdateAttachment();
        }

        public Slot(GlobalState globalState, string name, Bone b)
            : this(globalState)
        {
            this.Name = name;
            this.BoundedBone = b;
            this._globalState = globalState;
            UpdateAttachment();
        }

        public Slot(GlobalState globalState, Bone b)
            : this(globalState)
        {
            this.Name = $"tesr{Counter.GenerateNamePostfix()}";
            this.BoundedBone = b;
            this._globalState = globalState;
            UpdateAttachment();
        }

        /// <summary>
        /// Moves slot to target position
        /// </summary>
        /// <param name="x">Target x coordinate</param>
        /// <param name="y">Target y coordinate</param>
        public override void Move(double x, double y)
        {
            if (BoundedBone != null)
            {
                double dx = x - BoundedBone.GlobalX;
                double dy = y - BoundedBone.GlobalY;
                double rad = -BoundedBone.GlobalA * Math.PI / 180;
                _localX = dx * Math.Cos(rad) - dy * Math.Sin(rad);
                _localY = dx * Math.Sin(rad) + dy * Math.Cos(rad);
            }
            else
            {
                _localX = x;
                _localY = y;
            }
            CurrentAttachment?.SetPos(_localX, _localY, _localA);
        }

        /// <summary>
        /// Changes slot size
        /// </summary>
        /// <param name="x">X click coordinate</param>
        /// <param name="y">Y click coordinate</param>
        public override void Scale(double x, double y)
        {
            if (this.CurrentAttachment != null)
            {
                this.LengthX = Math.Abs(x - this.X) * 5;
                this.LengthY = Math.Abs(y - this.Y) * 5;

                this.CurrentAttachment.SetSize(this.LengthX, this.LengthY);
            }
        }

        /// <summary>
        /// Rotates slot
        /// </summary>
        /// <param name="a">Target angle</param>
        public override void Rotate(double a)
        {
            if (BoundedBone != null)
            {
                _localA = a - BoundedBone.A;
            }
            else
            {
                _localA = a;
            }
            CurrentAttachment?.SetPos(_localX, _localY, _localA);
        }

        public void DrawSlotSelection(Canvas canvas)
        {
            if (_globalState.IsSlotSelected(this))
            {
                var border = new Border
                {
                    Width = 10,
                    Height = 10,
                    BorderBrush = AppColors.Red,
                    BorderThickness = new Thickness(2),
                };
                Canvas.SetLeft(border, canvas.Width / 2 + this.GlobalX - 5);
                Canvas.SetTop(border, canvas.Height / 2 + this.GlobalY - 5);
                canvas.Children.Add(border);
            }
        }

        public new SlotData GenerateJSONData()
        {
            return new SlotData
            {
                Name = this.Name,
                Bone = this.BoundedBone?.Name,
                Attachment = this.CurrentAttachment?.Name,
            };
        }

        public new string GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }

        /// <summary>
        /// Sets name to IRenamble object
        /// </summary>
        /// <param name="name">New name</param>
        public new void SetName(string? name)
        {
            if (this._globalState.CurrentProject.IsUniqSlot(name))
            {
                if (name != null)
                {
                    this.Name = name;
                }
            }
        }

        public new string GetName
        {
            get => this.Name;
            set
            {
                if (this.Name != value)
                {
                    this.Name = value;
                }
            }
        }
    }

    /// <summary>
    /// Slot JSON data
    /// </summary>
    public class SlotData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("bone", NullValueHandling = NullValueHandling.Ignore)]
        public string? Bone { get; set; }

        [JsonProperty("attachment", NullValueHandling = NullValueHandling.Ignore)]
        public string? Attachment { get; set; }
    }
}
