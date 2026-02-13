using System.Collections.ObjectModel;

namespace TimeLine
{
    public class TimelineTrack
    {
        public string Name { get; set; } = "Track";
    }

    public class Keyframe
    {
        public double Time { get; set; }
        public bool IsSelected { get; set; }
    }
}
