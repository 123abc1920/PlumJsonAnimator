using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlumJsonAnimator.Models.Easing;
using PlumJsonAnimator.Models.Interfaces;

namespace PlumJsonAnimator.Models.Common;

public class EasingConverter : JsonConverter<IEasing>
{
    public override IEasing ReadJson(
        JsonReader reader,
        Type objectType,
        IEasing? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        if (reader.TokenType == JsonToken.String)
        {
            var s = (string)reader.Value!;
            return s == "stepped" ? new SteppedEasing() : new LinearEasing();
        }

        if (reader.TokenType == JsonToken.StartArray)
        {
            var arr = JArray.Load(reader);
            return new BezierEasing(
                (double)arr[0],
                (double)arr[1],
                (double)arr[2],
                (double)arr[3]
            );
        }

        return new LinearEasing();
    }

    public override void WriteJson(JsonWriter writer, IEasing? value, JsonSerializer serializer)
    {
        switch (value)
        {
            case SteppedEasing:
                writer.WriteValue("stepped");
                break;
            case BezierEasing b:
                writer.WriteStartArray();
                foreach (var v in b.KeysMap) writer.WriteValue(v);
                writer.WriteEndArray();
                break;
            default:
                break;
        }
    }
}
