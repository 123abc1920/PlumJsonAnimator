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

        public double LocalX = 0;
        public double LocalY = 0;
        public double LocalA = 0;

        [Reactive]
        public Attachment? CurrentAttachment { get; set; }

        public override double X
        {
            get => BoundedBone != null ? BoundedBone.X + LocalX : LocalX;
            set
            {
                LocalX = BoundedBone != null ? value - BoundedBone.X : value;
                this.RaisePropertyChanged(nameof(X));
            }
        }

        public override double Y
        {
            get => BoundedBone != null ? BoundedBone.Y + LocalY : LocalY;
            set
            {
                LocalY = BoundedBone != null ? value - BoundedBone.Y : value;
                this.RaisePropertyChanged(nameof(Y));
            }
        }

        public override double A
        {
            get => BoundedBone != null ? BoundedBone.A + LocalA : LocalA;
            set
            {
                LocalA = BoundedBone != null ? value - BoundedBone.A : value;
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
                LocalX = CurrentAttachment.x;
                LocalY = CurrentAttachment.y;
                LocalA = CurrentAttachment.a;

                var size = CurrentAttachment.GetSize();
                LengthX = size["width"] ?? LengthX;
                LengthY = size["height"] ?? LengthY;

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

        public double LengthX { get; set; } = 100;
        public double LengthY { get; set; } = 100;

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

        public double ScaleX { get; set; } = 1;
        public double ScaleY { get; set; } = 1;

        public double GlobalX
        {
            get
            {
                double globalX = LocalX;

                if (BoundedBone != null)
                {
                    double rad = BoundedBone.GlobalA * Math.PI / 180;
                    double rotatedX = LocalX * Math.Cos(rad) - LocalY * Math.Sin(rad);
                    globalX = BoundedBone.GlobalX + rotatedX;
                }

                return globalX;
            }
        }

        public double GlobalY
        {
            get
            {
                double globalY = LocalY;

                if (BoundedBone != null)
                {
                    double rad = BoundedBone.GlobalA * Math.PI / 180;
                    double rotatedY = LocalX * Math.Sin(rad) + LocalY * Math.Cos(rad);
                    globalY = BoundedBone.GlobalY + rotatedY;
                }

                return globalY;
            }
        }

        public double GlobalA
        {
            get
            {
                double globalAngle = LocalA;

                if (BoundedBone != null)
                {
                    globalAngle = BoundedBone.GlobalA + LocalA;
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
                LocalX = dx * Math.Cos(rad) - dy * Math.Sin(rad);
                LocalY = dx * Math.Sin(rad) + dy * Math.Cos(rad);
            }
            else
            {
                LocalX = x;
                LocalY = y;
            }
            CurrentAttachment?.SetPos(LocalX, LocalY, LocalA);
        }

        /// <summary>
        /// Changes slot size
        /// </summary>
        /// <param name="x">X click coordinate</param>
        /// <param name="y">Y click coordinate</param>
        public override void Scale(double x, double y)
        {
            if (CurrentAttachment != null)
            {
                LengthX = Math.Abs(x - this.X) * 5;
                LengthY = Math.Abs(y - this.Y) * 5;

                CurrentAttachment.SetSize(this.LengthX, this.LengthY);
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
                LocalA = a - BoundedBone.A;
            }
            else
            {
                LocalA = a;
            }
            CurrentAttachment?.SetPos(LocalX, LocalY, LocalA);
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
