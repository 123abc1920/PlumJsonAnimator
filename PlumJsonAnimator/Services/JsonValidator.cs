using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Validates json code
    /// </summary>
    public class JsonValidator
    {
        private LocalizationService localizationService;

        public JsonValidator(LocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        /// <summary>
        /// Validates text. Finds error line and position
        /// </summary>
        /// <param name="text">Text which has to be validated</param>
        /// <example>
        /// new Interpolation().linearInterpolation(20, 40, 0.5);
        /// </example>
        /// <returns>Information about errors</returns>
        public string Validate(String text)
        {
            try
            {
                JToken.Parse(text);
                return this.localizationService.GetMessage(LocalizationConsts.JSON_VALID);
            }
            catch (JsonReaderException ex)
            {
                return GetErrorWithContext(text, ex.LineNumber, ex.LinePosition, ex.Message);
            }
        }

        /// <summary>
        /// Linear interpolation between two values
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="errorLine">Error line</param>
        /// <param name="errorPos">Error position</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Text with info about errors</returns>
        private string GetErrorWithContext(
            string json,
            int errorLine,
            int errorPos,
            string errorMessage
        )
        {
            if (json == "" || json == null)
            {
                return $"{this.localizationService.GetMessage(LocalizationConsts.ERROR)}: {errorMessage}";
            }

            var lines = json.Split('\n');

            if (errorLine > lines.Length)
            {
                return $"{this.localizationService.GetMessage(LocalizationConsts.ERROR)}: {errorMessage}";
            }

            if (lines.Length <= errorLine - 1)
            {
                return $"{this.localizationService.GetMessage(LocalizationConsts.ERROR)}: {errorMessage}";
            }

            string errorLineText = lines[errorLine - 1];

            string pointer = new string(' ', Math.Max(0, errorPos - 1)) + "↑";

            string context = "";

            if (errorLine > 1)
                context += $"{errorLine - 1}: {lines[errorLine - 2]}\n";

            context += $"{errorLine}: {errorLineText}\n";
            context += $"    {pointer}\n";

            if (errorLine < lines.Length)
                context += $"{errorLine + 1}: {lines[errorLine]}\n";

            return $"{this.localizationService.GetMessage(LocalizationConsts.JSON_ERROR_STR)} {errorLine}, {this.localizationService.GetMessage(LocalizationConsts.JSON_ERROR_POS)} {errorPos}:\n"
                + $"{errorMessage}\n\n"
                + $"{this.localizationService.GetMessage(LocalizationConsts.JSON_CONTEXT)}:\n{context}";
        }
    }
}
