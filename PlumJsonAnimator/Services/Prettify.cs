using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlumJsonAnimator.Common.Constants;

// TODO: remove repitition with get error context
namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Provides mwthods for prettifying json
    /// </summary>
    public class Prettify
    {
        private GlobalState _globalState;
        private LocalizationService _localizationService;

        public Prettify(GlobalState globalState, LocalizationService localizationService)
        {
            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Prettifyes json code
        /// </summary>
        /// <param name="text">Json string</param>
        /// <returns>Prettifyed text</returns>
        public String prettify(String text)
        {
            try
            {
                var parsed = JToken.Parse(text);
                return parsed.ToString(Formatting.Indented);
            }
            catch (JsonReaderException ex)
            {
                this._globalState.jsonError.ErrorText = GetErrorWithContext(
                    text,
                    ex.LineNumber,
                    ex.LinePosition,
                    ex.Message
                );
                return text;
            }
        }

        /// <summary>
        /// Returns info about errors
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
            var lines = json.Split('\n');

            if (errorLine > lines.Length)
                return $"{this._localizationService.GetMessage(LocalizationConsts.ERROR)}: {errorMessage}";

            string errorLineText = lines[errorLine - 1];

            string pointer = new string(' ', Math.Max(0, errorPos - 1)) + "↑";

            string context = "";

            if (errorLine > 1)
                context += $"{errorLine - 1}: {lines[errorLine - 2]}\n";

            context += $"{errorLine}: {errorLineText}\n";
            context += $"    {pointer}\n";

            if (errorLine < lines.Length)
                context += $"{errorLine + 1}: {lines[errorLine]}\n";

            return $"{this._localizationService.GetMessage(LocalizationConsts.JSON_ERROR_STR)} {errorLine}, {this._localizationService.GetMessage(LocalizationConsts.JSON_ERROR_POS)} {errorPos}:\n"
                + $"{errorMessage}\n\n"
                + $"{this._localizationService.GetMessage(LocalizationConsts.JSON_CONTEXT)}:\n{context}";
        }
    }
}
