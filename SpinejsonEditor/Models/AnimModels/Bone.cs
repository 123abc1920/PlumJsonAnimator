using System;
using System.Collections.Generic;
using System.Collections.Generic;
using AnimEngine;
using AnimTransformations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnimModels
{
    public class Bone : IBone
    {
        public string name = "root";
        public List<Bone> children = new List<Bone>();
        public Bone? parent = null;
        public Slot? slot = null;
        public double endX = 110;
        public double endY = 110;
        public double length = 10;
        public BoneInAnimation boneInAnimation;

        public Bone()
        {
            this.a = -100;
            this.id = 0;
            this.x = 100;
            this.y = 100;

            double angleRad = this.a * Math.PI / 180;
            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);
        }

        public Bone(int _id)
        {
            this.id = _id;
            this.a = -100;
            this.x = 100;
            this.y = 100;
            this.name = "name" + this.id.ToString();
        }

        public void addChildren(Bone bone)
        {
            this.children.Add(bone);
            bone.parent = this;
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

            foreach (Bone c in this.children)
            {
                c.move(c.x - deltaX, c.y - deltaY);
            }

            if (this.slot != null)
            {
                this.slot.move(this.slot.x - deltaX, this.slot.y - deltaY);
            }

            if (!ConstantsClass.currentProject.GetAnimation().isRun)
            {
                if (this.boneInAnimation == null)
                {
                    this.boneInAnimation = new BoneInAnimation(this);
                    ConstantsClass
                        .currentProject.GetAnimation()
                        .skeletonInAnimation.bones.Add(this.boneInAnimation);
                }

                this.boneInAnimation.setTranslateKeyFrame(this.x, this.y);
            }
        }

        public override void rotate(double a)
        {
            double oldA = this.a;
            this.a = a;
            double angleRad = this.a * Math.PI / 180;

            this.endX = this.x + length * Math.Cos(angleRad);
            this.endY = this.y + length * Math.Sin(angleRad);

            foreach (Bone child in this.children)
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

            if (this.slot != null)
            {
                double slotdx = this.slot.x - this.x;
                double slotdy = this.slot.y - this.y;

                double slotangleDiff = (a - oldA) * Math.PI / 180;
                double slotnewDx =
                    slotdx * Math.Cos(slotangleDiff) - slotdy * Math.Sin(slotangleDiff);
                double slotnewDy =
                    slotdx * Math.Sin(slotangleDiff) + slotdy * Math.Cos(slotangleDiff);

                this.slot.x = this.x + slotnewDx;
                this.slot.y = this.y + slotnewDy;

                this.slot.a = this.slot.a + (a - oldA);
            }

            if (!ConstantsClass.currentProject.GetAnimation().isRun)
            {
                if (this.boneInAnimation == null)
                {
                    this.boneInAnimation = new BoneInAnimation(this);
                    ConstantsClass
                        .currentProject.GetAnimation()
                        .skeletonInAnimation.bones.Add(this.boneInAnimation);
                }
                this.boneInAnimation.setRotateKeyFrame(this.a);
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
                Stroke = Constants.Color.getLineBoneColor(this.id),
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Constants.Color.getDotBoneColor(this.id),
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
                Name = this.name,
                Parent = this.parent?.name,
                X = this.x,
                Y = this.y,
                Rotation = this.a
            };
        }

        public string generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
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

    [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double Rotation { get; set; }
}
