using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    /// <summary>
    /// Bone data
    /// </summary>
    public class Bone : INotifyable, IRenamable
    {
        public int id = 0;
        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public virtual bool IsBone
        {
            get { return true; }
        }

        private double _baseX = 0;
        private double _baseY = 0;
        private double _baseA = 0;

        public virtual double BaseX
        {
            get => _baseX;
            set
            {
                if (Math.Abs(_baseX - value) > double.Epsilon)
                {
                    _baseX = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        public virtual double BaseY
        {
            get => _baseY;
            set
            {
                if (Math.Abs(_baseY - value) > double.Epsilon)
                {
                    _baseY = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public virtual double BaseA
        {
            get => _baseA;
            set
            {
                if (Math.Abs(_baseA - value) > double.Epsilon)
                {
                    _baseA = value;
                    OnPropertyChanged(nameof(A));
                }
            }
        }

        private double _animX = 0;
        private double _animY = 0;
        private double _animA = 0;

        public virtual double AnimX
        {
            get => _animX;
            set
            {
                if (Math.Abs(_animX - value) > double.Epsilon)
                {
                    _animX = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        public virtual double AnimY
        {
            get => _animY;
            set
            {
                if (Math.Abs(_animY - value) > double.Epsilon)
                {
                    _animY = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public virtual double AnimA
        {
            get => _animA;
            set
            {
                if (Math.Abs(_animA - value) > double.Epsilon)
                {
                    Rotate(value);
                    _animA = value;
                    OnPropertyChanged(nameof(AnimA));
                }
            }
        }

        public virtual double X
        {
            get
            {
                if (this._globalState == null)
                {
                    return 0;
                }

                if (this._globalState.setBasePos == true)
                {
                    return this.BaseX;
                }
                else
                {
                    return this.BaseX + this.AnimX;
                }
            }
            set
            {
                if (this._globalState.setBasePos == true)
                {
                    this.BaseX = value;
                }
                else
                {
                    this.AnimX = value - this.BaseX;
                }
            }
        }

        public virtual double Y
        {
            get
            {
                if (this._globalState == null)
                {
                    return 0;
                }

                if (this._globalState.setBasePos == true)
                {
                    return this.BaseY;
                }
                else
                {
                    return this.BaseY + this.AnimY;
                }
            }
            set
            {
                if (this._globalState.setBasePos == true)
                {
                    this.BaseY = value;
                }
                else
                {
                    this.AnimY = value - this.BaseY;
                }
            }
        }

        public virtual double A
        {
            get
            {
                if (this._globalState == null)
                {
                    return 0;
                }

                if (this._globalState.setBasePos == true)
                {
                    return this.BaseA;
                }
                else
                {
                    return this.AnimA;
                }
            }
            set
            {
                if (this._globalState.setBasePos == true)
                {
                    this.BaseA = value;
                }
                else
                {
                    this.AnimA = value;
                }
            }
        }

        public virtual double GlobalX
        {
            get
            {
                double globalX = this.BaseX + (this._globalState.setBasePos ? 0 : this.AnimX);
                if (this.Parent != null)
                {
                    globalX += this.Parent.GlobalX;
                }
                return globalX;
            }
        }

        public virtual double GlobalY
        {
            get
            {
                double globalY = this.BaseY + (this._globalState.setBasePos ? 0 : this.AnimY);
                if (this.Parent != null)
                {
                    globalY += this.Parent.GlobalY;
                }
                return globalY;
            }
        }

        private ObservableCollection<Slot> _slots = new ObservableCollection<Slot>();
        public ObservableCollection<Slot> Slots => _slots;

        public void UpdateSlots()
        {
            _slots.Clear();

            var newSlots = this._globalState.CurrentProject?.CurrentSkin?.GetSlots(this);
            if (newSlots != null)
            {
                foreach (var slot in newSlots)
                {
                    _slots.Add(slot);
                }
            }
        }

        public ObservableCollection<Bone> Children { get; set; } = new ObservableCollection<Bone>();
        public Bone? Parent { get; set; } = null;

        /// <summary>
        /// Returns name of the IRenamable object
        /// </summary>
        public string GetName
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

        private double lengthX = 10;
        public virtual double LengthX
        {
            get => lengthX;
            set
            {
                if (lengthX != value && value > 0)
                {
                    lengthX = value;
                    OnPropertyChanged(nameof(LengthX));
                }
            }
        }

        public virtual double LengthY { get; set; } = 0;

        protected GlobalState _globalState;
        protected LocalizationService _localizationService;

        protected Bone() { }

        public Bone(GlobalState globalState, LocalizationService localizationService)
        {
            this.Name = "root";

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(
            GlobalState globalState,
            Bone parent,
            int _id,
            LocalizationService localizationService
        )
        {
            this.id = _id;
            string name = "bone";
            this.Name = $"{name}{Counter.GenerateNamePostfix()}";

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(GlobalState globalState, Bone parent, LocalizationService localizationService)
        {
            string name = "bone";
            this.Name = $"{name}{Counter.GenerateNamePostfix()}";

            this.Parent = parent;

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(GlobalState globalState, string name, LocalizationService localizationService)
        {
            this._name = name;

            this.id = 100;

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        private Point SetupEndXEndY()
        {
            double angleRad = this.A * Math.PI / 180;
            var endX = this.GlobalX + lengthX * Math.Cos(angleRad);
            var endY = this.GlobalY + lengthX * Math.Sin(angleRad);

            return new Point(endX, endY);
        }

        public void AddChildren(Bone bone)
        {
            this.Children.Add(bone);
            bone.Parent = this;
        }

        private bool _isMoving = false;

        /// <summary>
        /// Moves bone and all its children and slots to new position
        /// </summary>
        /// <param name="x">Target x coordinate</param>
        /// <param name="y">Target y coordinate</param>
        public virtual void Move(double x, double y)
        {
            if (_isMoving)
                return;
            _isMoving = true;

            double localX = x;
            double localY = y;

            if (this.Parent != null)
            {
                localX = x - this.Parent.GlobalX;
                localY = y - this.Parent.GlobalY;
            }

            this.X = localX;
            this.Y = localY;

            _isMoving = false;
        }

        private bool _isRotating = false;

        /// <summary>
        /// Rotates bone to new angle
        /// </summary>
        /// <param name="a">Target angle</param>
        public virtual void Rotate(double a)
        {
            if (_isRotating)
            {
                return;
            }

            _isRotating = true;

            double oldA = this.A;
            this.A = a;

            /*foreach (Bone child in this.Children)
            {
                double dx = child.X - this.X;
                double dy = child.Y - this.Y;

                double angleDiff = (a - oldA) * Math.PI / 180;
                double newDx = dx * Math.Cos(angleDiff) - dy * Math.Sin(angleDiff);
                double newDy = dx * Math.Sin(angleDiff) + dy * Math.Cos(angleDiff);

                child.X = this.X + newDx;
                child.Y = this.Y + newDy;

                child.Rotate(child.A + (a - oldA));
            }*/

            List<Slot> slots = this._globalState.CurrentProject!.CurrentSkin.GetSlots(this);
            var options = this._globalState.GetParallelOptions();
            Parallel.ForEach(
                slots,
                options,
                slot =>
                {
                    double slotdx = slot.X - this.X;
                    double slotdy = slot.Y - this.Y;

                    double slotangleDiff = (a - oldA) * Math.PI / 180;
                    double slotnewDx =
                        slotdx * Math.Cos(slotangleDiff) - slotdy * Math.Sin(slotangleDiff);
                    double slotnewDy =
                        slotdx * Math.Sin(slotangleDiff) + slotdy * Math.Cos(slotangleDiff);

                    slot.Move(this.X + slotnewDx, this.Y + slotnewDy);
                }
            );

            _isRotating = false;
        }

        /// <summary>
        /// Changes bone`s length
        /// </summary>
        /// <param name="x">Click x coordinate</param>
        /// <param name="y">Click y coordinate</param>
        public virtual void Scale(double x, double y)
        {
            this.LengthX = Math.Sqrt((x - this.X) * (x - this.X) + (y - this.Y) * (y - this.Y));
        }

        /// <summary>
        /// Draws bone
        /// </summary>
        /// <param name="canvas">Target canvas</param>
        public void DrawBone(Canvas canvas)
        {
            Point start = new Point(
                canvas.Width / 2 + this.GlobalX,
                canvas.Height / 2 + this.GlobalY
            );
            var endPoints = SetupEndXEndY();
            Point end = new Point(canvas.Width / 2 + endPoints.X, canvas.Height / 2 + endPoints.Y);

            var line = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = this._globalState.GetLineBoneColor(this),
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = this._globalState.GetDotBoneColor(this),
            };

            Canvas.SetLeft(joint, start.X - 4);
            Canvas.SetTop(joint, start.Y - 4);

            canvas.Children.Add(line);
            canvas.Children.Add(joint);
        }

        /// <summary>
        /// Returns JSON data
        /// </summary>
        public BoneData GenerateJSONData()
        {
            return new BoneData
            {
                Name = this.Name,
                Parent = this.Parent?.Name,
                X = this.X,
                Y = this.Y,
            };
        }

        /// <summary>
        /// Returns JSON string
        /// </summary>
        public string GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }

        /// <summary>
        /// Sets new name to IRenamable object
        /// </summary>
        /// <param name="name">New name</param>
        public void SetName(string? name)
        {
            if (name != null)
            {
                this.Name = name;
            }
        }
    }

    /// <summary>
    /// Jsonifyed bone data
    /// </summary>
    public class BoneData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string? Parent { get; set; }

        [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double X { get; set; }

        [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double Y { get; set; }
    }
}
