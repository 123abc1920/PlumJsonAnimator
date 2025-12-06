using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prettify
{
    public class Prettify
    {
        public static String prettify(String text)
        {
            var parsed = JToken.Parse(text);
            return parsed.ToString(Formatting.Indented);
        }
    }
}
