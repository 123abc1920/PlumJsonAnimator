using System.IO;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;

namespace PlumJsonAnimator.Services
{
    public class JsonExport
    {
        private JsonValidator jsonValidator;
        private JsonCode jsonCode;
        private GlobalState globalState;
        private LocalizationService localizationService;

        public JsonExport(
            JsonValidator jsonValidator,
            JsonCode jsonCode,
            GlobalState globalState,
            LocalizationService localizationService
        )
        {
            this.jsonValidator = jsonValidator;
            this.jsonCode = jsonCode;
            this.globalState = globalState;
            this.localizationService = localizationService;
        }

        public ExportResult exportSpineJson(string outFolder)
        {
            string output = JsonConvert.SerializeObject(
                this.jsonCode.generateJSONData(this.globalState.CurrentProject),
                this.globalState.jsonSettings
            );

            if (Directory.Exists(outFolder))
            {
                var filePath = Path.Combine(
                    outFolder,
                    $"{this.globalState.CurrentProject.Name}.json"
                );
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }
                File.WriteAllText(filePath, output);
                return ExportResult.SUCCESS;
            }

            return ExportResult.NO_FOLDER;
        }

        public ExportResult importSpineJson(string inputFile)
        {
            if (File.Exists(inputFile))
            {
                string text = File.ReadAllText(inputFile);
                string result = this.jsonValidator.validate(text);
                if (result == this.localizationService.GetMessage(LocalizationConsts.JSON_VALID))
                {
                    this.globalState.CurrentProject!.Code = text;
                    return ExportResult.SUCCESS;
                }
                else
                {
                    return ExportResult.INCORRECT_JSON;
                }
            }
            return ExportResult.NO_FOLDER;
        }
    }
}
