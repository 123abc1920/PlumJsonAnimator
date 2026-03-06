using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpinejsonGeneration
{
    public class SpineBoneJson
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("parent")]
        public required string Parent { get; set; }

        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }

        [JsonPropertyName("rotation")]
        public float Rotation { get; set; }

        [JsonPropertyName("scaleX")]
        public float ScaleX { get; set; } = 1;

        [JsonPropertyName("scaleY")]
        public float ScaleY { get; set; } = 1;
    }

    public class SpineSkeletonJson
    {
        [JsonPropertyName("bones")]
        public required List<SpineBoneJson> Bones { get; set; }

        [JsonPropertyName("skins")]
        public required List<SpineSkinJson> Skins { get; set; }
    }

    public class SpineSlotJson
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("bone")]
        public required string BoneName { get; set; }
    }

    public class SpineSkinJson
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
