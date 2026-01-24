using AnimModels;

namespace AnimEngine
{
    class Engine
    {
        public static void runAnimation(Animation animation)
        {
            if (animation.isRun)
            {
                animation.step();
            }
        }
    }
}
