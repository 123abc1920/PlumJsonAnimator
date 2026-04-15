using System;
using System.IO;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;

namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Provides methods for work with project settings. It is in "{projectDir}/{projectName}/settings.plmjsn: ususally.
    /// </summary>
    public class ProjectSettings
    {
        private AppSettings appSettings;
        private GlobalState globalState;
        private JsonCode jsonCode;
        private string settingsName;

        private const string BASE_ANIM =
            "{'skeleton':{'spine':'4.3.2'},'bones':[{'name':'root','x':100.0,'y':100.0}],'slots':[],'skins':[{'name':'default','attachments':{}}],'animations':{'anim0':{'bones':{},'drawOrder':[]}}}";

        private SettingsData settingsData;

        public SettingsData GetSettingsData()
        {
            return this.settingsData;
        }

        /// <summary>
        /// Sets settings from this project
        /// </summary>
        /// <param name="project"></param>
        public void UpdateSettings(Project project)
        {
            this.settingsData.Name = project.Name;
            this.settingsData.Path = project.ProjectPath;
            this.settingsData.Spine = project.MetaData.Spine;
            this.settingsData.Anim = project.Code;
        }

        public ProjectSettings(AppSettings appSettings, GlobalState globalState, JsonCode jsonCode)
        {
            this.appSettings = appSettings;
            this.globalState = globalState;
            this.jsonCode = jsonCode;

            this.settingsName = $"settings{this.globalState.programExt}";
            this.settingsData = new SettingsData()
            {
                Path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    this.globalState.workspace
                ),
                Name = "NewProject",
                Spine = "4.3.2",
                Anim = BASE_ANIM,
            };
        }

        public void ExistOrCreateProjectDirs()
        {
            if (!Directory.Exists(this.appSettings.appSettings!.Workspace))
            {
                Directory.CreateDirectory(this.appSettings.appSettings!.Workspace);
            }

            if (!Directory.Exists(Path.Combine(this.appSettings.appSettings!.Workspace, "res")))
            {
                Directory.CreateDirectory(
                    Path.Combine(this.appSettings.appSettings!.Workspace, "res")
                );
            }

            if (!File.Exists(Path.Combine(this.appSettings.appSettings!.Workspace, settingsName)))
            {
                File.Create(Path.Combine(this.appSettings.appSettings!.Workspace, settingsName))
                    .Close();
            }
        }

        public void WriteSettings()
        {
            string settingsPath = Path.Combine(
                this.appSettings.appSettings!.Workspace,
                settingsName
            );

            ExistOrCreateProjectDirs();

            File.WriteAllText(
                settingsPath,
                JsonConvert.SerializeObject(this.settingsData, this.globalState.jsonSettings)
            );
        }

        public void WriteAnimation(string anim)
        {
            string settingsPath = Path.Combine(
                this.appSettings.appSettings!.Workspace,
                settingsName
            );

            ExistOrCreateProjectDirs();

            this.settingsData.Anim = anim;

            ExistOrCreateProjectDirs();

            File.WriteAllText(
                settingsPath,
                JsonConvert.SerializeObject(this.settingsData, this.globalState.jsonSettings)
            );
        }

        private void readFile(string settingsPath)
        {
            if (!File.Exists(settingsPath))
            {
                WriteSettings();
                return;
            }

            ExistOrCreateProjectDirs();

            var settings = JsonConvert.DeserializeObject<SettingsData>(
                File.ReadAllText(settingsPath)
            );

            if (settings != null && settings.Name != null && settings.Path != null)
            {
                this.settingsData.Path = settings.Path;
                this.settingsData.Name = settings.Name;
                this.settingsData.Spine = settings.Spine;
                this.settingsData.Anim = settings.Anim;
            }
            else
            {
                WriteSettings();
            }
        }

        public void ReadSettings()
        {
            string settingsPath = Path.Combine(
                this.appSettings.appSettings!.Workspace,
                settingsName
            );

            readFile(settingsPath);
        }

        public void ReadSettings(string? path)
        {
            if (path != null)
            {
                readFile(path);
            }
        }
    }

    /// <summary>
    /// Contains jsonifyed settings data
    /// </summary>
    public class SettingsData
    {
        [JsonProperty("project_path")]
        public required string Path { get; set; }

        [JsonProperty("project_name")]
        public required string Name { get; set; }

        [JsonProperty("project_spine")]
        public required string Spine { get; set; }

        [JsonProperty("project_anim")]
        public required string Anim { get; set; }
    }
}
