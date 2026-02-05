using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Constants;
using Newtonsoft.Json;
using Renameble;

namespace AnimModels
{
    public class Bone : IBone, INotifyPropertyChanged, IRenamable
    {
        private double _x = 0;
        private double _y = 0;
        private double _a = 0;

        public override double x
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

        public override double y
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

        public override double a
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

        private string _name = "";
        public override string Name
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

        private ObservableCollection<Slot> _slots = new ObservableCollection<Slot>();
        public ObservableCollection<Slot> Slots => _slots;

        public void UpdateSlots()
        {
            _slots.Clear();

            var newSlots = ConstantsClass.currentProject?.CurrentSkin?.GetSlots(this);
            if (newSlots != null)
            {
                foreach (var slot in newSlots)
                {
                    _slots.Add(slot);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<IBone> Children { get; set; } =
            new ObservableCollection<IBone>();
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

        public double endX = 110;
        public double endY = 110;
        public double length = 10;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Bone()
        {
            this.Name = "root";

            this._a = -100;
            this.id = 0;
            this._x = 100;
            this._y = 100;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this._x + length * Math.Cos(angleRad);
            this.endY = this._y + length * Math.Sin(angleRad);

            this.isBone = true;
        }

        public Bone(int _id)
        {
            this.id = _id;
            this._a = -100;
            this._x = 100;
            this._y = 100;
            this.Name = "name" + this.id.ToString();

            this.isBone = true;
        }

        public Bone(int _id, Bone parent, String name, double x, double y, double a)
        {
            this.id = _id;
            this._a = a;
            this._x = x;
            this._y = y;
            this.Name = name;
            this.Parent = parent;

            this.isBone = true;
        }

        public Bone(Bone parent)
        {
            this._a = -100;
            this.id = 0;
            this._x = 100;
            this._y = 100;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);

            this.Parent = parent;
            this.id = 100;

            this.isBone = true;
        }

        public void addChildren(Bone bone)
        {
            this.Children.Add(bone);
            bone.Parent = this;
        }

        public override void move(double x, double y)
        {
            double deltaX = this.x - x;
            double deltaY = this.y - y;

            this.x = x;
            this.y = y;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);

            foreach (Bone c in this.Children)
            {
                c.move(c.x - deltaX, c.y - deltaY);
            }

            List<Slot> slots = ConstantsClass.currentProject.CurrentSkin.GetSlots(this);
            foreach (Slot slot in slots)
            {
                slot.move(slot.x - deltaX, slot.y - deltaY);
            }

            var animation = ConstantsClass.currentProject?.GetAnimation();
            if (animation != null && !animation.isRun)
            {
                animation.TranslateBone(this, this.x, this.y);
            }
        }

        public override void rotate(double a)
        {
            double oldA = this.a;
            this.a = a;
            double angleRad = this.a * Math.PI / 180;

            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);

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

            List<Slot> slots = ConstantsClass.currentProject.CurrentSkin.GetSlots(this);
            foreach (Slot slot in slots)
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

            var animation = ConstantsClass.currentProject?.GetAnimation();
            if (animation != null && !animation.isRun)
            {
                animation.RotateBone(this, this.a);
            }
        }

        public void drawBone(Canvas canvas)
        {
            Point start = new Point(canvas.Width / 2 + this.x, canvas.Height / 2 + this.y);
            Point end = new Point(canvas.Width / 2 + this.endX, canvas.Height / 2 + this.endY);

            var line = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = Color.getLineBoneColor(this.id),
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Color.getDotBoneColor(this.id),
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
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
        }

        public void SetName(string? name)
        {
            if (name != null)
            {
                this.Name = name;
            }
        }
    }
}

public class BoneData
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
    public string Parent { get; set; }

    [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double X { get; set; }

    [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double Y { get; set; }
}
