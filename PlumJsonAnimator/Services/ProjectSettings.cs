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

        private void SetupProject()
        {
            this.globalState.currentProject!.ProjectPath = this.settingsData.Path;
            this.globalState.currentProject.Name = this.settingsData.Name;
            this.globalState.currentProject.MetaData.Spine = this.settingsData.Spine;
            this.jsonCode.Text = this.settingsData.Anim;
        }

        public void ExistOrCreateProjectDirs()
        {
            if (!Directory.Exists(this.globalState.currentProject!.GetProjectPath()))
            {
                Directory.CreateDirectory(this.globalState.currentProject.GetProjectPath());
            }

            if (
                !Directory.Exists(
                    Path.Combine(this.globalState.currentProject.GetProjectPath(), "res")
                )
            )
            {
                Directory.CreateDirectory(
                    Path.Combine(this.globalState.currentProject.GetProjectPath(), "res")
                );
            }

            if (
                !File.Exists(
                    Path.Combine(this.globalState.currentProject.GetProjectPath(), settingsName)
                )
            )
            {
                File.Create(
                        Path.Combine(this.globalState.currentProject.GetProjectPath(), settingsName)
                    )
                    .Close();
            }
        }

        public void WriteAllSettings()
        {
            string settingsPath = Path.Combine(
                this.globalState.currentProject!.GetProjectPath(),
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
                this.globalState.currentProject!.GetProjectPath(),
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
                this.globalState.currentProject!.GetProjectPath(),
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
                this.globalState.currentProject!.GetProjectPath(),
                settingsName
            );

            readFile(settingsPath);
            SetupProject();
        }

        public void ReadSettings(string? path)
        {
            if (path != null)
            {
                readFile(path);
                SetupProject();
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
