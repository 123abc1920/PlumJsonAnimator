using System;
using System.Collections.Generic;
using System.IO;
using AnimEngine;
using Constants;
using Newtonsoft.Json;

namespace ProjectSettings
{
    public class ProjectSettings
    {
        private static string settingsName = "settings.spjsn";

        public static void ExistOrCreateProjectDirs()
        {
            if (!Directory.Exists(ConstantsClass.currentProject.GetProjectPath()))
            {
                Directory.CreateDirectory(ConstantsClass.currentProject.GetProjectPath());
            }

            if (
                !Directory.Exists(
                    Path.Combine(ConstantsClass.currentProject.GetProjectPath(), "res")
                )
            )
            {
                Directory.CreateDirectory(
                    Path.Combine(ConstantsClass.currentProject.GetProjectPath(), "res")
                );
            }

            if (
                !File.Exists(
                    Path.Combine(ConstantsClass.currentProject.GetProjectPath(), settingsName)
                )
            )
            {
                File.Create(
                        Path.Combine(ConstantsClass.currentProject.GetProjectPath(), settingsName)
                    )
                    .Close();
            }
        }

        public static void WriteAllSettings()
        {
            string settingsPath = Path.Combine(
                ConstantsClass.currentProject.GetProjectPath(),
                settingsName
            );

            SettingsData settings = new SettingsData();

            settings.Path = ConstantsClass.currentProject.ProjectPath;
            settings.Name = ConstantsClass.currentProject.Name;
            settings.Spine = ConstantsClass.currentProject.MetaData.Spine;
            settings.Anim = JsonConvert.SerializeObject(
                ConstantsClass.currentProject.SpinejsonCode.generateJSONData(
                    ConstantsClass.currentProject
                ),
                ConstantsClass.jsonSettings
            );

            ExistOrCreateProjectDirs();

            File.WriteAllText(
                settingsPath,
                JsonConvert.SerializeObject(settings, ConstantsClass.jsonSettings)
            );
        }

        public static void WriteSettings()
        {
            string settingsPath = Path.Combine(
                ConstantsClass.currentProject.GetProjectPath(),
                settingsName
            );

            ExistOrCreateProjectDirs();

            var settings = JsonConvert.DeserializeObject<SettingsData>(
                File.ReadAllText(settingsPath)
            );

            settings.Path = ConstantsClass.currentProject.ProjectPath;
            settings.Name = ConstantsClass.currentProject.Name;
            settings.Spine = ConstantsClass.currentProject.MetaData.Spine;
            settings.Anim = settings.Anim;

            ExistOrCreateProjectDirs();

            File.WriteAllText(
                settingsPath,
                JsonConvert.SerializeObject(settings, ConstantsClass.jsonSettings)
            );
        }

        public static void WriteAnim()
        {
            string settingsPath = Path.Combine(
                ConstantsClass.currentProject.GetProjectPath(),
                settingsName
            );

            ExistOrCreateProjectDirs();

            var settings = JsonConvert.DeserializeObject<SettingsData>(
                File.ReadAllText(settingsPath)
            );

            settings.Path = settings.Path;
            settings.Name = settings.Name;
            settings.Spine = settings.Spine;
            settings.Anim = JsonConvert.SerializeObject(
                ConstantsClass.currentProject.SpinejsonCode.generateJSONData(
                    ConstantsClass.currentProject
                ),
                ConstantsClass.jsonSettings
            );

            ExistOrCreateProjectDirs();

            File.WriteAllText(
                settingsPath,
                JsonConvert.SerializeObject(settings, ConstantsClass.jsonSettings)
            );
        }

        private static void readFile(string settingsPath)
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

            ConstantsClass.currentProject.ProjectPath = settings.Path;
            ConstantsClass.currentProject.Name = settings.Name;

            AppSettings.SaveSettings();
        }

        public static void ReadSettings()
        {
            string settingsPath = Path.Combine(
                ConstantsClass.currentProject.GetProjectPath(),
                settingsName
            );

            readFile(settingsPath);
        }

        public static void ReadSettings(string? path)
        {
            if (path != null)
            {
                readFile(path);
            }
        }
    }
}

public class SettingsData
{
    [JsonProperty("project_path")]
    public string Path { get; set; }

    [JsonProperty("project_name")]
    public string Name { get; set; }

    [JsonProperty("project_spine")]
    public string Spine { get; set; }

    [JsonProperty("project_anim")]
    public string Anim { get; set; }
}
