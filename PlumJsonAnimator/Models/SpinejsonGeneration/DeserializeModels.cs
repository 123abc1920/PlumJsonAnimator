using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeserializeModels
{
    public class SpineBoneJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parent")]
        public string Parent { get; set; }

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
        public List<SpineBoneJson> Bones { get; set; }

        [JsonPropertyName("skins")]
        public List<SpineSkinJson> Skins { get; set; }
    }

    public class SpineSlotJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bone")]
        public string BoneName { get; set; }
    }

    public class SpineSkinJson
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
}
