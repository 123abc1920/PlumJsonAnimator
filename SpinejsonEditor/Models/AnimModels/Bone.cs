using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AnimEngine;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Constants;
using Newtonsoft.Json;
using Resources;

namespace AnimModels
{
    public class Bone : IBone, INotifyPropertyChanged
    {
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
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<IBone> Children { get; set; } =
            new ObservableCollection<IBone>();
        public Bone? Parent { get; set; } = null;
        private Slot? _slot;
        public Slot? Slot
        {
            get => _slot;
            set
            {
                _slot = value;

                OnPropertyChanged(nameof(Slot));
                OnPropertyChanged(nameof(SlotName));
                OnPropertyChanged(nameof(HasSlot));
            }
        }
        public string SlotName
        {
            get { return Slot?.Name ?? ""; }
        }
        public bool HasSlot
        {
            get { return Slot != null; }
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

            this.a = -100;
            this.id = 0;
            this.x = 100;
            this.y = 100;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);

            this.isBone = true;
        }

        public Bone(int _id)
        {
            this.id = _id;
            this.a = -100;
            this.x = 100;
            this.y = 100;
            this.Name = "name" + this.id.ToString();

            this.isBone = true;
        }

        public Bone(int _id, Bone parent, String name, double x, double y, double a)
        {
            this.id = _id;
            this.a = a;
            this.x = x;
            this.y = y;
            this.Name = name;
            this.Parent = parent;

            this.isBone = true;
        }

        public Bone(Bone parent)
        {
            this.a = -100;
            this.id = 0;
            this.x = 100;
            this.y = 100;

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

            if (this.Slot != null)
            {
                this.Slot.move(this.Slot.x - deltaX, this.Slot.y - deltaY);
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

            if (this.Slot != null)
            {
                double slotdx = this.Slot.x - this.x;
                double slotdy = this.Slot.y - this.y;

                double slotangleDiff = (a - oldA) * Math.PI / 180;
                double slotnewDx =
                    slotdx * Math.Cos(slotangleDiff) - slotdy * Math.Sin(slotangleDiff);
                double slotnewDy =
                    slotdx * Math.Sin(slotangleDiff) + slotdy * Math.Cos(slotangleDiff);

                this.Slot.x = this.x + slotnewDx;
                this.Slot.y = this.y + slotnewDy;

                this.Slot.a = this.Slot.a + (a - oldA);
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

        public void AddSlot(Res res)
        {
            Slot s = new Slot("tesr", this, new ImageAttachment((ImageRes)res));
            this.Slot = s;
        }

        public string generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
        }

        public override IEnumerable<IBone> CombinedChildren
        {
            get
            {
                if (Slot != null)
                    yield return Slot;

                foreach (var child in Children)
                    yield return child;
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
