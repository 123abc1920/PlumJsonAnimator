using Newtonsoft.Json;

namespace PlumJsonAnimator.Models.SkeletonNameSpace;

/// <summary>
/// Jsonifyed attachment data
/// </summary>
public class AttachmentData
{
    [JsonProperty("type")]
    public required string Type { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
    public int? Width { get; set; }

    [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
    public int? Height { get; set; }

    [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
    public double X { get; set; }

    [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
    public double Y { get; set; }

    [JsonProperty("a", NullValueHandling = NullValueHandling.Ignore)]
    public double A { get; set; }
}
