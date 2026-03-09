using System;
using System.IO;
using Avalonia;
using Avalonia.Styling;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Services
{
    public class AppSettings
    {
        private string AppSettingsPath;
        private string AppSettingsFile;
        public AppSettingsData? appSettings = null;

        private GlobalState globalState;

        public AppSettings(GlobalState globalState)
        {
            this.globalState = globalState;

            AppSettingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlumJsonAnimator"
            );
            AppSettingsFile = Path.Combine(
                AppSettingsPath,
                $"settings{this.globalState.programExt}"
            );

            this.appSettings = new AppSettingsData()
            {
                Lang = "ru",
                LastDir = "",
                Theme = "light",
                Workspace = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    this.globalState.workspace
                ),
            };
        }

        public void SaveSettings()
        {
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }

            if (!File.Exists(AppSettingsFile))
            {
                File.Create(AppSettingsFile).Close();
            }

            File.WriteAllText(
                AppSettingsFile,
                JsonConvert.SerializeObject(this.appSettings, this.globalState.jsonSettings)
            );
        }

        public void ReadSettings()
        {
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }

            if (!File.Exists(AppSettingsFile))
            {
                File.Create(AppSettingsFile).Close();
                SaveSettings();
            }

            var settings = JsonConvert.DeserializeObject<AppSettingsData>(
                File.ReadAllText(AppSettingsFile)
            );

            if (settings != null)
            {
                if (
                    settings.LastDir != null
                    && settings.LastDir != ""
                    && Directory.Exists(settings.LastDir)
                )
                {
                    this.appSettings!.LastDir = settings.LastDir;
                    this.appSettings.Workspace = settings.LastDir;
                    this.appSettings.Theme = settings.Theme;

                    this.globalState.theme = settings.Theme;

                    if (this.globalState.theme == "dark")
                    {
                        Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
                    }
                    else
                    {
                        Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
                    }
                }
            }
        }

        public string GetTheme()
        {
            return this.appSettings!.Theme;
        }
    }

    public class AppSettingsData()
    {
        [JsonProperty("last_dir")]
        public required string LastDir { get; set; }

        [JsonProperty("workspace")]
        public required string Workspace { get; set; }

        [JsonProperty("theme")]
        public string Theme { get; set; } = "light";

        [JsonProperty("language")]
        public string Lang { get; set; } = "ru";
    }
}
