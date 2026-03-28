using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlumJsonAnimator.Services
{
    public class JsonValidator
    {
        /*private static String schemaJson =
            @"{
    '$schema': 'http://json-schema.org/draft-07/schema#',
    'type': 'object',
    'required': ['skeleton', 'bones'],
    'properties': {
        'skeleton': {
            'type': 'object',
            'required': ['spine']
        },
        'bones': {
            'type': 'array',
            'items': {
                'type': 'object',
                'required': ['name']
            }
        },
        'slots': {
            'type': 'array',
            'items': {
                'type': 'object'
            }
        },
        'skins': {
            'type': 'array',
            'items': {
                'type': 'object'
            }
        },
        'animations': {
            'type': 'object'
        }
    }
}";*/
        private LocalizationService localizationService;

        public JsonValidator(LocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        public string validate(String text)
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
