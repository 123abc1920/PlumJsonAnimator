using System.IO;
using Constants;
using Newtonsoft.Json;

namespace AnimExport
{
    namespace JsonExport
    {
        public enum ExportResult
        {
            SUCCESS = 0,
            NO_FOLDER,
            INCORRECT_JSON,
        }

        public class SpineJsonExport
        {
            public static ExportResult exportSpineJson(string outFolder)
            {
                string output = JsonConvert.SerializeObject(
                    ConstantsClass.currentProject.SpinejsonCode.generateJSONData(
                        ConstantsClass.currentProject
                    ),
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

            public static ExportResult importSpineJson(string inputFile)
            {
                if (File.Exists(inputFile))
                {
                    string text = File.ReadAllText(inputFile);
                    string result = JsonValidator.JsonValidator.validate(text);
                    if (result == "JSON is valid")
                    {
                        ConstantsClass.currentProject.SpinejsonCode.Text = text;
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
}
