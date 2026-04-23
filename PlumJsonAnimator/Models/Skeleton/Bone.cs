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

        private double _x = 0;
        private double _y = 0;
        private double _a = 0;

        public virtual double X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > double.Epsilon)
                {
                    Move(value, _y);
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        public virtual double Y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) > double.Epsilon)
                {
                    Move(_x, value);
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public virtual double A
        {
            get => _a;
            set
            {
                if (Math.Abs(_a - value) > double.Epsilon)
                {
                    Rotate(value);
                    _a = value;
                    OnPropertyChanged(nameof(A));
                }
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

        private double endX = 10;
        private double endY = 10;
        private double lengthX = 10;
        public virtual double LengthX
        {
            get => lengthX;
            set
            {
                if (lengthX != value && value > 0)
                {
                    lengthX = value;
                    double angleRad = this.A * Math.PI / 180;
                    this.endX = this.X + lengthX * Math.Cos(angleRad);
                    this.endY = this.Y + lengthX * Math.Sin(angleRad);
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

            SetupEndXEndY();

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

            this._x = parent.X;
            this._y = parent.Y;

            SetupEndXEndY();

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(GlobalState globalState, Bone parent, LocalizationService localizationService)
        {
            string name = "bone";
            this.Name = $"{name}{Counter.GenerateNamePostfix()}";

            this._x = parent.X;
            this._y = parent.Y;

            SetupEndXEndY();

            this.Parent = parent;

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(
            GlobalState globalState,
            string name,
            Bone parent,
            LocalizationService localizationService
        )
        {
            this._name = name;

            this._x = parent.X;
            this._y = parent.Y;

            SetupEndXEndY();

            this.Parent = parent;
            this.id = 100;

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        private void SetupEndXEndY()
        {
            double angleRad = this.A * Math.PI / 180;
            this.endX = this._x + lengthX * Math.Cos(angleRad);
            this.endY = this._y + lengthX * Math.Sin(angleRad);
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
            {
                return;
            }

            _isMoving = true;

            double oldX = this.X;
            double oldY = this.Y;

            double deltaX = oldX - x;
            double deltaY = oldY - y;

            this.X = x;
            this.Y = y;

            double angleRad = this.A * Math.PI / 180;
            this.endX = this.X + lengthX * Math.Cos(angleRad);
            this.endY = this.Y + lengthX * Math.Sin(angleRad);

            foreach (Bone c in this.Children)
            {
                c.Move(c.X - deltaX, c.Y - deltaY);
            }

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
            double angleRad = this.A * Math.PI / 180;

            this.endX = this.X + lengthX * Math.Cos(angleRad);
            this.endY = this.Y + lengthX * Math.Sin(angleRad);

            foreach (Bone child in this.Children)
            {
                double dx = child.X - this.X;
                double dy = child.Y - this.Y;

                double angleDiff = (a - oldA) * Math.PI / 180;
                double newDx = dx * Math.Cos(angleDiff) - dy * Math.Sin(angleDiff);
                double newDy = dx * Math.Sin(angleDiff) + dy * Math.Cos(angleDiff);

                child.X = this.X + newDx;
                child.Y = this.Y + newDy;

                child.Rotate(child.A + (a - oldA));
            }

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
            Point start = new Point(canvas.Width / 2 + this.X, canvas.Height / 2 + this.Y);
            Point end = new Point(canvas.Width / 2 + this.endX, canvas.Height / 2 + this.endY);

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
