using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

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

    [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
    public string? End { get; set; }

    [JsonProperty("vertexCount", NullValueHandling = NullValueHandling.Ignore)]
    public int VertexCount { get; set; }

    [JsonProperty("vertices", NullValueHandling = NullValueHandling.Ignore)]
    public List<double>? Vertices { get; set; }
}
