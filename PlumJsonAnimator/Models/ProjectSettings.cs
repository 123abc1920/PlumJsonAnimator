using System;
using System.IO;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models;

/// <summary>
/// Provides methods for work with project settings. It is in "{projectDir}/{projectName}/settings.plmjsn ususally.
/// </summary>
public class ProjectSettings
{
    private AppSettings appSettings;
    private GlobalState globalState;

    private const string BASE_ANIM =
        "{'skeleton':{'spine':'4.3.2'},'bones':[{'name':'root','x':100.0,'y':100.0}],'slots':[],'skins':[{'name':'default','attachments':{}}],'animations':{'anim0':{'bones':{},'drawOrder':[]}}}";

    private SettingsData settingsData;

    public SettingsData GetSettingsData()
    {
        return this.settingsData;
    }

    public ProjectSettings(AppSettings appSettings, GlobalState globalState)
    {
        this.appSettings = appSettings;
        this.globalState = globalState;

        this.settingsData = new SettingsData()
        {
            Path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                this.globalState.globalWorkspace
            ),
            Name = "NewProject",
            Spine = "4.3.2",
            Anim = BASE_ANIM,
        };
    }

    public ProjectSettings(string path, AppSettings appSettings, GlobalState globalState)
        : this(appSettings, globalState)
    {
        SettingsData? settingsData = ReadSettings(path);
        if (settingsData != null)
        {
            this.settingsData = settingsData;
        }
    }

    public void ExistOrCreateProjectDirs()
    {
        if (!Directory.Exists(this.appSettings.appSettings!.Workspace))
        {
            Directory.CreateDirectory(this.appSettings.appSettings!.Workspace);
        }

        if (!Directory.Exists(Path.Combine(this.appSettings.appSettings!.Workspace, "res")))
        {
            Directory.CreateDirectory(Path.Combine(this.appSettings.appSettings!.Workspace, "res"));
        }

        if (
            !File.Exists(
                Path.Combine(
                    this.appSettings.appSettings!.Workspace,
                    this.globalState.SETTINGS_FILE_NAME
                )
            )
        )
        {
            File.Create(
                    Path.Combine(
                        this.appSettings.appSettings!.Workspace,
                        this.globalState.SETTINGS_FILE_NAME
                    )
                )
                .Close();
        }
    }

    public void SaveSettings()
    {
        string settingsPath = Path.Combine(
            this.appSettings.appSettings!.Workspace,
            this.globalState.SETTINGS_FILE_NAME
        );

        ExistOrCreateProjectDirs();

        File.WriteAllText(
            settingsPath,
            JsonConvert.SerializeObject(this.settingsData, this.globalState.jsonSettings)
        );
    }

    private void WriteProjectInFile(string filePath, string jsonifyedProject)
    {
        ExistOrCreateProjectDirs();

        this.settingsData.Anim = jsonifyedProject;

        ExistOrCreateProjectDirs();

        File.WriteAllText(
            filePath,
            JsonConvert.SerializeObject(this.settingsData, this.globalState.jsonSettings)
        );
    }

    public void WriteProjectJSON(string jsonifyedProject)
    {
        string settingsPath = Path.Combine(
            this.appSettings.appSettings!.Workspace,
            this.globalState.SETTINGS_FILE_NAME
        );
        WriteProjectInFile(settingsPath, jsonifyedProject);
    }

    public void WriteAutoSave(string jsonifyedProject)
    {
        string settingsPath = Path.Combine(
            this.appSettings.appSettings!.Workspace,
            this.globalState.AUTO_SAVE_FILE
        );
        Console.WriteLine(settingsPath);
        WriteProjectInFile(settingsPath, jsonifyedProject);
    }

    private SettingsData? readFile(string settingsPath)
    {
        if (!File.Exists(settingsPath))
        {
            SaveSettings();
            return null;
        }

        ExistOrCreateProjectDirs();

        var settings = JsonConvert.DeserializeObject<SettingsData>(File.ReadAllText(settingsPath));

        if (settings != null && settings.Name != null)
        {
            settings.Path = Directory
                .GetParent(Directory.GetParent(settingsPath).FullName)
                .FullName;

            return settings;
        }
        else
        {
            return null;
        }
    }

    public SettingsData? ReadSettings()
    {
        string settingsPath = Path.Combine(
            this.appSettings.appSettings!.Workspace,
            this.globalState.SETTINGS_FILE_NAME
        );

        return readFile(settingsPath);
    }

    public SettingsData? ReadSettings(string? path)
    {
        if (path != null)
        {
            return readFile(path);
        }
        return null;
    }
}

/// <summary>
/// Contains jsonifyed settings data
/// </summary>
public class SettingsData
{
    [JsonIgnore]
    public required string Path { get; set; }

    [JsonProperty("project_name")]
    public required string Name { get; set; }

    [JsonProperty("project_spine")]
    public required string Spine { get; set; }

    [JsonProperty("project_anim")]
    public required string Anim { get; set; }
}
