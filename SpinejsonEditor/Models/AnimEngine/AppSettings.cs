using System;
using System.IO;
using Constants;
using Newtonsoft.Json;

namespace AnimEngine
{
    public class AppSettings
    {
        private static string AppSettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SpinejsonEditor"
        );
        private static string AppSettingsFile = Path.Combine(AppSettingsPath, "settings.spjsn");

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
}
