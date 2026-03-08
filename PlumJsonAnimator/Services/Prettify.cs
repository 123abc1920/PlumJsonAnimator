using System;
using Common.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlumJsonAnimator.Services
{
    public class Prettify
    {
        public Prettify() { }

        public String prettify(String text)
        {
            try
            {
                var parsed = JToken.Parse(text);
                return parsed.ToString(Formatting.Indented);
            }
            catch (JsonReaderException ex)
            {
                ConstantsClass.jsonError.ErrorText = GetErrorWithContext(
                    text,
                    ex.LineNumber,
                    ex.LinePosition,
                    ex.Message
                );
                return text;
            }
        }

        private string GetErrorWithContext(
            string json,
            int errorLine,
            int errorPos,
            string errorMessage
        )
        {
            var lines = json.Split('\n');

            if (errorLine > lines.Length)
                return $"Ошибка: {errorMessage}";

            string errorLineText = lines[errorLine - 1];

            string pointer = new string(' ', Math.Max(0, errorPos - 1)) + "↑";

            string context = "";

            if (errorLine > 1)
                context += $"{errorLine - 1}: {lines[errorLine - 2]}\n";

            context += $"{errorLine}: {errorLineText}\n";
            context += $"    {pointer}\n";

            if (errorLine < lines.Length)
                context += $"{errorLine + 1}: {lines[errorLine]}\n";

            return $"Ошибка на строке {errorLine}, позиция {errorPos}:\n"
                + $"{errorMessage}\n\n"
                + $"Контекст:\n{context}";
        }
    }
}
