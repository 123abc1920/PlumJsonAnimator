using System;
using System.Collections.Generic;

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

        public String generateCode()
        {
            String code = "";

            for (int i = 0; i < this.bones.Count; i++)
            {
                code += this.bones[i].generateCode();
                if (i != this.bones.Count - 1)
                {
                    code += ",";
                }
            }

            return code;
        }
    }
}
