using System.IO;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;

namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Provides methods for exporting and importing json code from file
    /// </summary>
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

        /// <summary>
        /// Exports current project into json code
        /// </summary>
        /// <param name="outFolder">The path to output folder</param>
        /// <param name="project">Project that has to be exported</param>
        public ExportResult exportSpineJson(string outFolder, Project? project)
        {
            if (project == null)
            {
                return ExportResult.PROJECT_IS_NULL;
            }

            string output = JsonConvert.SerializeObject(
                this.jsonCode.generateJSONData(project),
                this.globalState.jsonSettings
            );

            if (Directory.Exists(outFolder))
            {
                var filePath = Path.Combine(outFolder, $"{project.Name}.json");
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }
                File.WriteAllText(filePath, output);
                return ExportResult.SUCCESS;
            }

            return ExportResult.NO_FOLDER;
        }

        /// <summary>
        /// Import json code from file
        /// </summary>
        /// <param name="inputFile">Project</param>
        /// <param name="project"></param>
        public ExportResult importSpineJson(string inputFile, Project project)
        {
            if (File.Exists(inputFile))
            {
                string text = File.ReadAllText(inputFile);
                string result = this.jsonValidator.Validate(text);
                if (result == this.localizationService.GetMessage(LocalizationConsts.JSON_VALID))
                {
                    project!.Code = text;
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
