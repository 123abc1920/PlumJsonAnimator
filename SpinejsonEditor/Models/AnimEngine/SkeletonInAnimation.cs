using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnimEngine
{
    public class SkeletonInAnimation
    {
        public List<BoneInAnimation> bones = new List<BoneInAnimation>();

        public void animationStep(double time)
        {
            foreach (BoneInAnimation b in bones)
            {
                b.animationStep();
            }
        }

        public SkeletonInAnimationData generateJSONData()
        {
            List<BoneInAnimationData> bones = new List<BoneInAnimationData>();
            for (int i = 0; i < this.bones.Count; i++)
            {
                bones.Add(this.bones[i].generateJSONData());
            }

            return new SkeletonInAnimationData { Bones = bones };
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

public class SkeletonInAnimationData
{
    [JsonProperty("bones")]
    public List<BoneInAnimationData> Bones { get; set; } = new List<BoneInAnimationData>();
}
