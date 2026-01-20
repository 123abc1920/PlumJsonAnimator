using System;
using System.Collections.Generic;
using System.Linq;
using AnimModels;
using AnimTransformations;
using Constants;
using Newtonsoft.Json;

namespace AnimEngine
{
    public class BoneInAnimation
    {
        public Bone? bone;
        public List<IKeyframeType> rotateKeyframes = new List<IKeyframeType>();
        public List<IKeyframeType> translateKeyframes = new List<IKeyframeType>();
        public List<IKeyframeType> shearKeyframes = new List<IKeyframeType>();
        public List<IKeyframeType> scaleKeyframes = new List<IKeyframeType>();
        public Translate currentTranslateFrame = null,
            nextTranslateFrame = null;
        public Rotate currentRotateFrame = null,
            nextRotateFrame = null;
        public double tTranslate = 0;
        public double tRotate = 0;

        public BoneInAnimation(Bone _bone)
        {
            this.bone = _bone;
        }

        public void setRotateKeyFrame(double value)
        {
            foreach (Rotate frame in rotateKeyframes)
            {
                if (
                    Math.Abs(frame.time - ConstantsClass.currentProject.GetAnimation().currentTime)
                    < 0.01f
                )
                {
                    frame.value = value;
                    return;
                }
            }

            rotateKeyframes.Add(
                new Rotate(ConstantsClass.currentProject.GetAnimation().currentTime, value)
            );
            rotateKeyframes = rotateKeyframes.OrderBy(k => k.time).ToList();
        }

        public void setTranslateKeyFrame(double x, double y)
        {
            foreach (Translate frame in translateKeyframes)
            {
                if (
                    Math.Abs(frame.time - ConstantsClass.currentProject.GetAnimation().currentTime)
                    < 0.01f
                )
                {
                    frame.x = x;
                    frame.y = y;
                    return;
                }
            }

            translateKeyframes.Add(
                new Translate(ConstantsClass.currentProject.GetAnimation().currentTime, x, y)
            );
            translateKeyframes = translateKeyframes.OrderBy(k => k.time).ToList();
        }

        public void setShearKeyFrame(double x, double y)
        {
            foreach (Shear frame in shearKeyframes)
            {
                if (frame.time == ConstantsClass.currentProject.GetAnimation().currentTime)
                {
                    frame.x = x;
                    frame.y = y;
                    return;
                }
            }

            shearKeyframes.Add(
                new Shear(ConstantsClass.currentProject.GetAnimation().currentTime, x, y)
            );
            shearKeyframes = shearKeyframes.OrderBy(k => k.time).ToList();
        }

        public void setScaleKeyFrame(double x, double y)
        {
            foreach (Scale frame in scaleKeyframes)
            {
                if (frame.time == ConstantsClass.currentProject.GetAnimation().currentTime)
                {
                    frame.x = x;
                    frame.y = y;
                    return;
                }
            }

            scaleKeyframes.Add(
                new Scale(ConstantsClass.currentProject.GetAnimation().currentTime, x, y)
            );
            scaleKeyframes = scaleKeyframes.OrderBy(k => k.time).ToList();
        }

        private void findCurrentSegment()
        {
            double currentTime = ConstantsClass.currentProject.GetAnimation().currentTime;

            // Ищем сегмент, где currentTime находится между frame.time и nextFrame.time
            for (int i = 0; i < translateKeyframes.Count - 1; i++)
            {
                Translate frame = (Translate)translateKeyframes[i];
                Translate nextFrame = (Translate)translateKeyframes[i + 1];

                if (currentTime >= frame.time && currentTime < nextFrame.time)
                {
                    currentTranslateFrame = frame;
                    nextTranslateFrame = nextFrame;
                    return; // Нашли сегмент, выходим
                }
            }

            // Если время больше последнего кадра, просто остаемся на последнем кадре
            if (
                translateKeyframes.Count > 0
                && currentTime >= ((Translate)translateKeyframes[translateKeyframes.Count - 1]).time
            )
            {
                currentTranslateFrame = (Translate)translateKeyframes[translateKeyframes.Count - 1];
                nextTranslateFrame = null;
            }
            // Если время меньше первого кадра, остаемся на первом
            else if (translateKeyframes.Count > 0)
            {
                currentTranslateFrame = (Translate)translateKeyframes[0];
                nextTranslateFrame = (Translate)translateKeyframes[1];
            }
        }

        public void translateStep()
        {
            findCurrentSegment();

            if (currentTranslateFrame == null)
            {
                return;
            }

            if (nextTranslateFrame == null)
            {
                this.bone?.move(currentTranslateFrame.x, currentTranslateFrame.y);
                return;
            }

            double currentTime = ConstantsClass.currentProject.GetAnimation().currentTime;

            double segmentDuration = nextTranslateFrame.time - currentTranslateFrame.time;

            double timeElapsed = currentTime - currentTranslateFrame.time;

            double t;
            if (segmentDuration > 0)
            {
                t = timeElapsed / segmentDuration;
            }
            else
            {
                t = 1.0;
            }

            t = Math.Clamp(t, 0.0, 1.0);

            double interpolatedX = Interpolations.Interpolation.linearInterpolation(
                currentTranslateFrame.x,
                nextTranslateFrame.x,
                t
            );
            double interpolatedY = Interpolations.Interpolation.linearInterpolation(
                currentTranslateFrame.y,
                nextTranslateFrame.y,
                t
            );

            this.bone?.move(interpolatedX, interpolatedY);
        }

        private void findCurrentRotateSegment(double currentTime)
        {
            // Если кадров меньше двух, сегмент для интерполяции не существует
            if (rotateKeyframes.Count < 2)
            {
                currentRotateFrame = rotateKeyframes.Count > 0 ? (Rotate)rotateKeyframes[0] : null;
                nextRotateFrame = null;
                return;
            }

            // Ищем сегмент, где currentTime находится между frame.time и nextFrame.time
            for (int i = 0; i < rotateKeyframes.Count - 1; i++)
            {
                Rotate frame = (Rotate)rotateKeyframes[i];
                Rotate nextFrame = (Rotate)rotateKeyframes[i + 1];

                if (currentTime >= frame.time && currentTime < nextFrame.time)
                {
                    currentRotateFrame = frame;
                    nextRotateFrame = nextFrame;
                    return; // Нашли сегмент, выходим
                }
            }

            // Если время больше времени последнего кадра, остаемся на последнем кадре
            Rotate lastFrame = (Rotate)rotateKeyframes[rotateKeyframes.Count - 1];
            if (currentTime >= lastFrame.time)
            {
                currentRotateFrame = lastFrame;
                nextRotateFrame = null;
            }
        }

        public void rotateStep()
        {
            double currentTime = ConstantsClass.currentProject.GetAnimation().currentTime;

            // 1. Находим текущий сегмент на основе фактического времени
            findCurrentRotateSegment(currentTime);

            if (currentRotateFrame == null)
            {
                return;
            }

            // 2. Если только один кадр или конец анимации (nextRotateFrame == null)
            if (nextRotateFrame == null)
            {
                // Устанавливаем значение последнего/единственного кадра
                this.bone?.rotate(currentRotateFrame.value);
                return;
            }

            // 3. Вычисление пропорционального фактора t

            double segmentDuration = nextRotateFrame.time - currentRotateFrame.time;
            double timeElapsed = currentTime - currentRotateFrame.time;

            double t;
            if (segmentDuration > 0)
            {
                // Пропорциональный фактор t находится в диапазоне [0, 1]
                t = timeElapsed / segmentDuration;
            }
            else
            {
                // Если сегмент длится 0 времени, t = 1 (мгновенный переход)
                t = 1.0;
            }

            // Ограничиваем t, чтобы избежать проблем с округлением
            t = Math.Clamp(t, 0.0, 1.0);

            // 4. Интерполяция с использованием t

            // Используем angleInterpolation (это важно для корректного вращения,
            // чтобы избежать длинного пути между, например, 350° и 10°)
            double interpolatedA = Interpolations.Interpolation.angleInterpolation(
                currentRotateFrame.value,
                nextRotateFrame.value,
                t // Используем t, основанное на времени
            );

            this.bone?.rotate(interpolatedA);
        }

        public void animationStep()
        {
            translateStep();
            rotateStep();
        }

        public BoneInAnimationData generateJSONData()
        {
            List<IKeyframeTypeData> translate = new List<IKeyframeTypeData>();
            for (int i = 0; i < this.translateKeyframes.Count; i++)
            {
                translate.Add(this.translateKeyframes[i].generateJSONData());
            }

            List<IKeyframeTypeData> rotate = new List<IKeyframeTypeData>();
            /*for (int i = 0; i < this.translateKeyframes.Count; i++)
            {
                rotate.Add(this.rotateKeyframes[i].generateJSONData());
            }*/

            List<IKeyframeTypeData> shear = new List<IKeyframeTypeData>();

            return new BoneInAnimationData
            {
                Name = this.bone.Name,
                Translate = translate,
                Rotate = rotate,
                Shear = shear,
            };
        }

        public String generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
        }
    }
}

public class BoneInAnimationData
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("translate", NullValueHandling = NullValueHandling.Ignore)]
    public List<IKeyframeTypeData> Translate { get; set; }

    [JsonProperty("rotate", NullValueHandling = NullValueHandling.Ignore)]
    public List<IKeyframeTypeData> Rotate { get; set; }

    [JsonProperty("shear", NullValueHandling = NullValueHandling.Ignore)]
    public List<IKeyframeTypeData> Shear { get; set; }
}
