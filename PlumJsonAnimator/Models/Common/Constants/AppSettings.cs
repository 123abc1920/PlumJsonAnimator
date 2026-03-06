using System;
using System.IO;
using Avalonia;
using Avalonia.Styling;
using Constants;
using Newtonsoft.Json;

namespace AnimEngine
{
    public class AppSettings
    {
        private static string AppSettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PlumJsonAnimator"
        );
        private static string AppSettingsFile = Path.Combine(
            AppSettingsPath,
            $"settings{ConstantsClass.programExt}"
        );
        public static AppSettingsData appSettings = null;

        public static void SaveSettings()
        {
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }

            if (!File.Exists(AppSettingsFile))
            {
                File.Create(AppSettingsFile).Close();
            }

            var settings = JsonConvert.DeserializeObject<AppSettingsData>(
                File.ReadAllText(AppSettingsFile)
            );

            if (settings == null)
            {
                settings = new AppSettingsData();
            }

            settings.LastDir = ConstantsClass.currentProject.GetProjectPath();
            settings.Workspace = ConstantsClass.currentProject.ProjectPath;
            settings.Lang = "ru";
            settings.Theme = ConstantsClass.theme;

            appSettings = settings;

            File.WriteAllText(
                AppSettingsFile,
                JsonConvert.SerializeObject(settings, ConstantsClass.jsonSettings)
            );
        }

        public static void ReadSettings()
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
                    string path = settings.LastDir;
                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    string lastFolder = dirInfo.Name;
                    string parentPath = dirInfo.Parent?.FullName;

                    ConstantsClass.currentProject.ProjectPath = parentPath;
                    ConstantsClass.currentProject.Name = lastFolder;
                    ConstantsClass.theme = settings.Theme;

                    if (ConstantsClass.theme == "dark")
                    {
                        Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                    }
                    else
                    {
                        Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                    }
                }
            }
        }
    }
}

public class AppSettingsData()
{
    [JsonProperty("last_dir")]
    public string LastDir { get; set; }

    [JsonProperty("workspace")]
    public string Workspace { get; set; }

    [JsonProperty("theme")]
    public string Theme { get; set; }

    [JsonProperty("language")]
    public string Lang { get; set; }
}
