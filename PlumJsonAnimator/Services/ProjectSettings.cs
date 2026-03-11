using System;
using System.IO;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Services
{
    public class ProjectSettings
    {
        private AppSettings appSettings;
        private GlobalState globalState;
        private JsonCode jsonCode;
        private string settingsName;

        private SettingsData settingsData;

        public SettingsData GetSettingsData()
        {
            return this.settingsData;
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
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    this.globalState.workspace
                ),
                Name = "NewProject",
                Spine = "4.3.2",
                Anim = "",
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

        public void WriteAllSettings()
        {
            string settingsPath = Path.Combine(
                this.appSettings.appSettings!.Workspace,
                settingsName
            );

            /*settings.Path = this.globalState.currentProject.ProjectPath;
            settings.Name = this.globalState.currentProject.Name;
            settings.Spine = this.globalState.currentProject.MetaData.Spine;
            settings.Anim = JsonConvert.SerializeObject(
                this.jsonCode.generateJSONData(this.globalState.currentProject),
                this.globalState.jsonSettings
            );*/

            ExistOrCreateProjectDirs();

            File.WriteAllText(
                settingsPath,
                JsonConvert.SerializeObject(this.settingsData, this.globalState.jsonSettings)
            );
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

        public void WriteAnim(string anim)
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
                WriteAllSettings();
                return;
            }

            ExistOrCreateProjectDirs();

            var settings = JsonConvert.DeserializeObject<SettingsData>(
                File.ReadAllText(settingsPath)
            );

            if (settings != null)
            {
                this.settingsData.Path = settings.Path;
                this.settingsData.Name = settings.Name;
                this.settingsData.Spine = settings.Spine;
                this.settingsData.Anim = settings.Anim;
            }

            this.appSettings.SaveSettings();
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
