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

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    public class Bone : INotifyable, IRenamable
    {
        public int id;
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

        public virtual double x
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > double.Epsilon)
                {
                    _x = value;
                    move(_x, _y);
                    OnPropertyChanged(nameof(x));
                }
            }
        }

        public virtual double y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) > double.Epsilon)
                {
                    _y = value;
                    move(_x, _y);
                    OnPropertyChanged(nameof(y));
                }
            }
        }

        public virtual double a
        {
            get => _a;
            set
            {
                if (Math.Abs(_a - value) > double.Epsilon)
                {
                    _a = value;
                    rotate(_a);
                    OnPropertyChanged(nameof(a));
                }
            }
        }

        private ObservableCollection<Slot> _slots = new ObservableCollection<Slot>();
        public ObservableCollection<Slot> Slots => _slots;

        public void UpdateSlots()
        {
            _slots.Clear();

            var newSlots = this.globalState.currentProject?.CurrentSkin?.GetSlots(this);
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

        private double endX = 110;
        private double endY = 110;
        private double lengthX = 10;
        public virtual double LengthX
        {
            get => lengthX;
            set
            {
                if (lengthX != value && value > 0)
                {
                    lengthX = value;
                    double angleRad = this.a * Math.PI / 180;
                    this.endX = this.x + lengthX * Math.Cos(angleRad);
                    this.endY = this.y + lengthX * Math.Sin(angleRad);
                    OnPropertyChanged(nameof(LengthX));
                }
            }
        }

        public virtual double LengthY { get; set; } = 0;

        protected GlobalState globalState;

        protected Bone() { }

        public Bone(GlobalState globalState)
        {
            this.Name = "root";

            this._a = -100;
            this.id = 0;
            this._x = 100;
            this._y = 100;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this._x + lengthX * Math.Cos(angleRad);
            this.endY = this._y + lengthX * Math.Sin(angleRad);

            this.globalState = globalState;
        }

        public Bone(GlobalState globalState, int _id)
        {
            this.id = _id;
            this._a = -100;
            this._x = 100;
            this._y = 100;
            string name = "bone";
            this.Name = $"{name}{Counter.GenerateName()}";

            this.globalState = globalState;
        }

        public Bone(
            GlobalState globalState,
            int _id,
            Bone parent,
            String name,
            double x,
            double y,
            double a
        )
        {
            this.id = _id;
            this._a = a;
            this._x = x;
            this._y = y;
            this.Name = $"{name}{Counter.GenerateName()}";
            this.Parent = parent;

            this.globalState = globalState;
        }

        public Bone(GlobalState globalState, Bone parent)
        {
            this._a = -100;
            this.id = 0;
            this._x = 100;
            this._y = 100;

            string name = "bone";
            this.Name = $"{name}{Counter.GenerateName()}";

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + lengthX * Math.Cos(angleRad);
            this.endY = this.y + lengthX * Math.Sin(angleRad);

            this.Parent = parent;
            this.id = 100;

            this.globalState = globalState;
        }

        public Bone(GlobalState globalState, string name, Bone parent)
        {
            this._a = -100;
            this.id = 0;
            this._x = 100;
            this._y = 100;

            this._name = $"{name}{Counter.GenerateName()}";

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + lengthX * Math.Cos(angleRad);
            this.endY = this.y + lengthX * Math.Sin(angleRad);

            this.Parent = parent;
            this.id = 100;

            this.globalState = globalState;
        }

        public void addChildren(Bone bone)
        {
            this.Children.Add(bone);
            bone.Parent = this;
        }

        public virtual void move(double x, double y)
        {
            double deltaX = this.x - x;
            double deltaY = this.y - y;

            this.x = x;
            this.y = y;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + lengthX * Math.Cos(angleRad);
            this.endY = this.y + lengthX * Math.Sin(angleRad);

            foreach (Bone c in this.Children)
            {
                c.move(c.x - deltaX, c.y - deltaY);
            }

            List<Slot> slots = this.globalState.currentProject!.CurrentSkin.GetSlots(this);
            var options = this.globalState.GetParallelOptions();
            Parallel.ForEach(
                slots,
                options,
                slot =>
                {
                    slot.move(slot.x - deltaX, slot.y - deltaY);
                }
            );
        }

        public virtual void rotate(double a)
        {
            double oldA = this.a;
            this.a = a;
            double angleRad = this.a * Math.PI / 180;

            this.endX = this.x + lengthX * Math.Cos(angleRad);
            this.endY = this.y + lengthX * Math.Sin(angleRad);

            foreach (Bone child in this.Children)
            {
                double dx = child.x - this.x;
                double dy = child.y - this.y;

                double angleDiff = (a - oldA) * Math.PI / 180;
                double newDx = dx * Math.Cos(angleDiff) - dy * Math.Sin(angleDiff);
                double newDy = dx * Math.Sin(angleDiff) + dy * Math.Cos(angleDiff);

                child.x = this.x + newDx;
                child.y = this.y + newDy;

                child.rotate(child.a + (a - oldA));
            }

            List<Slot> slots = this.globalState.currentProject!.CurrentSkin.GetSlots(this);
            var options = this.globalState.GetParallelOptions();
            Parallel.ForEach(
                slots,
                options,
                slot =>
                {
                    double slotdx = slot.x - this.x;
                    double slotdy = slot.y - this.y;

                    double slotangleDiff = (a - oldA) * Math.PI / 180;
                    double slotnewDx =
                        slotdx * Math.Cos(slotangleDiff) - slotdy * Math.Sin(slotangleDiff);
                    double slotnewDy =
                        slotdx * Math.Sin(slotangleDiff) + slotdy * Math.Cos(slotangleDiff);

                    slot.x = this.x + slotnewDx;
                    slot.y = this.y + slotnewDy;

                    slot.parentA = slot.parentA + (a - oldA);
                }
            );
        }

        public virtual void scale(double x, double y)
        {
            this.LengthX = Math.Sqrt((x - this.x) * (x - this.x) + (y - this.y) * (y - this.y));
        }

        public void drawBone(Canvas canvas)
        {
            Point start = new Point(canvas.Width / 2 + this.x, canvas.Height / 2 + this.y);
            Point end = new Point(canvas.Width / 2 + this.endX, canvas.Height / 2 + this.endY);

            var line = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = this.globalState.getLineBoneColor(this.id),
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = this.globalState.getDotBoneColor(this.id),
            };

            Canvas.SetLeft(joint, start.X - 4);
            Canvas.SetTop(joint, start.Y - 4);

            canvas.Children.Add(line);
            canvas.Children.Add(joint);
        }

        public BoneData generateJSONData()
        {
            return new BoneData
            {
                Name = this.Name,
                Parent = this.Parent?.Name,
                X = this.x,
                Y = this.y,
            };
        }

        public string generateCode()
        {
            return JsonConvert.SerializeObject(generateJSONData(), this.globalState.jsonSettings);
        }

        public void SetName(string? name)
        {
            if (name != null)
            {
                this.Name = name;
            }
        }
    }

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
