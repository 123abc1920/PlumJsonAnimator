using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    /// <summary>
    /// Bone data
    /// </summary>
    public class Bone : ReactiveObject, IRenamable
    {
        public int id = 0;

        [Reactive]
        public string Name { get; set; } = "";
        public virtual bool IsBone
        {
            get { return true; }
        }

        [Reactive]
        public virtual double BaseX { get; set; }

        [Reactive]
        public virtual double BaseY { get; set; }

        [Reactive]
        public virtual double BaseA { get; set; }

        [Reactive]
        public virtual double AnimX { get; set; }

        [Reactive]
        public virtual double AnimY { get; set; }

        private double _animA;
        public virtual double AnimA
        {
            get => _animA;
            set
            {
                if (Math.Abs(_animA - value) > double.Epsilon)
                {
                    Rotate(value);
                    this.RaiseAndSetIfChanged(ref _animA, value);
                }
            }
        }

        public virtual double X
        {
            get
            {
                if (_globalState == null)
                    return 0;
                return _globalState.setBasePos ? BaseX : BaseX + AnimX;
            }
            set
            {
                if (_globalState.setBasePos)
                {
                    BaseX = value;
                }
                else
                {
                    AnimX = value - BaseX;
                }
            }
        }

        public virtual double Y
        {
            get
            {
                if (_globalState == null)
                    return 0;
                return _globalState.setBasePos ? BaseY : BaseY + AnimY;
            }
            set
            {
                if (_globalState.setBasePos)
                {
                    BaseY = value;
                }
                else
                {
                    AnimY = value - BaseY;
                }
            }
        }

        public virtual double A
        {
            get
            {
                if (_globalState == null)
                    return 0;
                return _globalState.setBasePos ? BaseA : (AnimA == 0 ? BaseA : AnimA);
            }
            set
            {
                if (_globalState.setBasePos)
                {
                    BaseA = value;
                }
                else
                {
                    AnimA = value;
                }
            }
        }

        private double _shearX = 0;
        public double ShearX
        {
            get => _shearX;
            set { this.RaiseAndSetIfChanged(ref _shearX, value); }
        }

        private double _shearY = 0;
        public double ShearY
        {
            get => _shearY;
            set { this.RaiseAndSetIfChanged(ref _shearY, value); }
        }

        public double GlobalX
        {
            get
            {
                double localX = BaseX + (_globalState.setBasePos ? 0 : AnimX);
                double localY = BaseY + (_globalState.setBasePos ? 0 : AnimY);

                if (Parent != null)
                {
                    return Parent.GlobalX + (localX * Parent.G11 + localY * Parent.G21);
                }

                return localX;
            }
            private set { }
        }

        public double GlobalY
        {
            get
            {
                double localY = BaseY + (_globalState.setBasePos ? 0 : AnimY);
                double localX = BaseX + (_globalState.setBasePos ? 0 : AnimX);

                if (Parent != null)
                {
                    return Parent.GlobalY + (localX * Parent.G12 + localY * Parent.G22);
                }

                return localY;
            }
            private set { }
        }

        public virtual double GlobalA
        {
            get
            {
                double angle = A;
                Bone? current = Parent;

                while (current != null)
                {
                    angle += current.A;
                    current = current.Parent;
                }

                return angle;
            }
        }

        private ObservableCollection<Slot> _slots = new ObservableCollection<Slot>();
        public ObservableCollection<Slot> Slots => _slots;

        public void UpdateSlots()
        {
            _slots.Clear();

            var newSlots = _globalState.CurrentProject?.CurrentSkin?.GetSlots(this);
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
            get => Name;
            set
            {
                if (Name != value)
                {
                    Name = value;
                }
            }
        }

        private const double LengthX = 10;
        private const double LengthY = 0;

        public double G11 { get; private set; } = 1;
        public double G12 { get; private set; } = 0;
        public double G21 { get; private set; } = 0;
        public double G22 { get; private set; } = 1;

        [Reactive]
        public SolidColorBrush BoneColor { get; set; } = new SolidColorBrush(Colors.Black);

        protected GlobalState _globalState;
        protected LocalizationService _localizationService;

        protected Bone()
        {
            this.WhenAnyValue(x => x.BaseX, x => x.AnimX)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(X)));

            this.WhenAnyValue(x => x.BaseY, x => x.AnimY)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(Y)));

            this.WhenAnyValue(x => x.BaseA, x => x.AnimA)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(A)));

            BoneColor = GenerateRandomColor();
        }

        public Bone(GlobalState globalState, LocalizationService localizationService)
            : this()
        {
            Name = "root";

            _globalState = globalState;
            _localizationService = localizationService;
        }

        public Bone(
            GlobalState globalState,
            Bone parent,
            int _id,
            LocalizationService localizationService
        )
            : this()
        {
            id = _id;
            string name = "bone";
            Name = $"{name}{Counter.GenerateNamePostfix()}";

            _globalState = globalState;
            _localizationService = localizationService;
        }

        public Bone(GlobalState globalState, Bone parent, LocalizationService localizationService)
            : this()
        {
            string name = "bone";
            Name = $"{name}{Counter.GenerateNamePostfix()}";

            Parent = parent;

            _globalState = globalState;
            _localizationService = localizationService;
        }

        public Bone(GlobalState globalState, string name, LocalizationService localizationService)
            : this()
        {
            Name = name;

            id = 100;

            _globalState = globalState;
            _localizationService = localizationService;
        }

        public void AddChildren(Bone bone)
        {
            Children.Add(bone);
            bone.Parent = this;
        }

        private bool _isMoving = false;

        /// <summary>
        /// Moves bone and all its children and slots to new position
        /// </summary>
        /// <param name="x">Target x coordinate (global)</param>
        /// <param name="y">Target y coordinate (global)</param>
        public virtual void Move(double x, double y)
        {
            if (_isMoving)
                return;
            _isMoving = true;

            if (Parent != null)
            {
                double dx = x - Parent.GlobalX;
                double dy = y - Parent.GlobalY;

                double parentAngleRad = -Parent.GlobalA * Math.PI / 180;
                double localX = dx * Math.Cos(parentAngleRad) - dy * Math.Sin(parentAngleRad);
                double localY = dx * Math.Sin(parentAngleRad) + dy * Math.Cos(parentAngleRad);

                if (_globalState.setBasePos)
                {
                    BaseX = localX;
                    BaseY = localY;
                }
                else
                {
                    AnimX = localX - BaseX;
                    AnimY = localY - BaseY;
                }

                X = localX;
                Y = localY;
            }
            else
            {
                if (_globalState.setBasePos)
                {
                    BaseX = x;
                    BaseY = y;
                }
                else
                {
                    AnimX = x - BaseX;
                    AnimY = y - BaseY;
                }

                X = x;
                Y = y;
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
                return;

            _isRotating = true;

            A = a;

            _isRotating = false;
        }

        /// <summary>
        /// Changes bone`s length
        /// </summary>
        /// <param name="x">Click x coordinate</param>
        /// <param name="y">Click y coordinate</param>
        public virtual void Scale(double x, double y)
        {
            ScaleX = x;
            ScaleY = y;
        }

        private double _scaleX = 1;
        public virtual double ScaleX
        {
            get => _scaleX;
            set
            {
                if (Math.Abs(_scaleX - value) > double.Epsilon)
                {
                    this.RaiseAndSetIfChanged(ref _scaleX, value);
                }
            }
        }

        private double _scaleY = 1;
        public virtual double ScaleY
        {
            get => _scaleY;
            set
            {
                if (Math.Abs(_scaleY - value) > double.Epsilon)
                {
                    this.RaiseAndSetIfChanged(ref _scaleY, value);
                }
            }
        }

        public virtual void Shear(double x, double y)
        {
            ShearX = x;
            ShearY = y;
        }

        /// <summary>
        /// Draws bone with matrix logic matching Spine 2D
        /// </summary>
        public void DrawBone(Canvas canvas)
        {
            double endX = GlobalX + (LengthX * G11);
            double endY = GlobalY + (LengthX * G12);

            Point start = new Point(canvas.Width / 2 + GlobalX, canvas.Height / 2 + GlobalY);
            Point end = new Point(canvas.Width / 2 + endX, canvas.Height / 2 + endY);

            var line = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = _globalState.GetLineBoneColor(this),
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = _globalState.GetDotBoneColor(this),
            };

            Canvas.SetLeft(joint, start.X - 4);
            Canvas.SetTop(joint, start.Y - 4);

            canvas.Children.Add(line);
            canvas.Children.Add(joint);

            foreach (var childBone in Children)
            {
                childBone.DrawBone(canvas);
            }
        }

        /// <summary>
        /// Computes G11..G22, GlobalX, GlobalY recursively without drawing.
        /// </summary>
        public void UpdateTransform(
            double m11 = 1,
            double m12 = 0,
            double m21 = 0,
            double m22 = 1,
            double parentX = 0,
            double parentY = 0
        )
        {
            double rotationRad = A * Math.PI / 180;
            double shearXRad = ShearX * Math.PI / 180;
            double shearYRad = ShearY * Math.PI / 180;

            double angleX = rotationRad + shearXRad;
            double angleY = rotationRad + Math.PI / 2 + shearYRad;

            double cosX = Math.Cos(angleX) * ScaleX;
            double sinX = Math.Sin(angleX) * ScaleX;
            double cosY = Math.Cos(angleY) * ScaleY;
            double sinY = Math.Sin(angleY) * ScaleY;

            double local11 = cosX;
            double local12 = sinX;
            double local21 = cosY;
            double local22 = sinY;

            double g11 = m11 * local11 + m21 * local12;
            double g12 = m12 * local11 + m22 * local12;
            double g21 = m11 * local21 + m21 * local22;
            double g22 = m12 * local21 + m22 * local22;

            double globalX = parentX + (X * m11 + Y * m21);
            double globalY = parentY + (X * m12 + Y * m22);

            G11 = g11;
            G12 = g12;
            G21 = g21;
            G22 = g22;
            GlobalX = globalX;
            GlobalY = globalY;

            foreach (var childBone in Children)
            {
                childBone.UpdateTransform(g11, g12, g21, g22, globalX, globalY);
            }
        }

        private SolidColorBrush GenerateRandomColor()
        {
            var random = new Random();
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);
            return new SolidColorBrush(new Color(255, r, g, b));
        }

        /// <summary>
        /// Returns JSON data
        /// </summary>
        public BoneData GenerateJSONData()
        {
            return new BoneData
            {
                Name = Name,
                Parent = Parent?.Name,
                X = BaseX,
                Y = BaseY,
                Rotation = BaseA,
                ShearX = ShearX,
                ShearY = ShearY,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
            };
        }

        /// <summary>
        /// Returns JSON string
        /// </summary>
        public string GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), _globalState.jsonSettings);
        }

        /// <summary>
        /// Sets new name to IRenamable object
        /// </summary>
        /// <param name="name">New name</param>
        public void SetName(string? name)
        {
            if (_globalState.CurrentProject.IsUniqBone(name) == true)
            {
                if (name != null)
                {
                    Name = name;
                }
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

        [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double Rotation { get; set; }

        [JsonProperty("shearX", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double ShearX { get; set; }

        [JsonProperty("shearY", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double ShearY { get; set; }

        [JsonProperty("scaleX", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double ScaleX { get; set; }

        [JsonProperty("scaleY", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double ScaleY { get; set; }
    }
}
