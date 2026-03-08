using System.IO;
using Common.Constants;
using Newtonsoft.Json;
using PlumJsonAnimator.Models;

namespace PlumJsonAnimator.Services
{
    public class JsonExport
    {
        private JsonValidator jsonValidator;
        private JsonCode jsonCode;

        public JsonExport(JsonValidator jsonValidator, JsonCode jsonCode)
        {
            this.jsonValidator = jsonValidator;
            this.jsonCode = jsonCode;
        }

        public ExportResult exportSpineJson(string outFolder)
        {
            string output = JsonConvert.SerializeObject(
                this.jsonCode.generateJSONData(ConstantsClass.currentProject),
                ConstantsClass.jsonSettings
            );

            if (Directory.Exists(outFolder))
            {
                var filePath = Path.Combine(
                    outFolder,
                    $"{ConstantsClass.currentProject.Name}.json"
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
                if (result == "JSON is valid")
                {
                    this.jsonCode.Text = text;
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
