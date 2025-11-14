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
    }
}
