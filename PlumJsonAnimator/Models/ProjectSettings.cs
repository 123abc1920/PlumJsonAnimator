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
    private AppSettings _appSettings;
    private GlobalState _globalState;

    private const string BASE_ANIM =
        "{'skeleton':{'spine':'4.3.2'},'bones':[{'name':'root','x':100.0,'y':100.0}],'slots':[],'skins':[{'name':'default','attachments':{}}],'animations':{'anim0':{'bones':{},'drawOrder':[]}}}";

    private SettingsData _settingsData;

    public ProjectSettings(AppSettings appSettings, GlobalState globalState)
    {
        _appSettings = appSettings;
        _globalState = globalState;

        _settingsData = new SettingsData()
        {
            Path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                _globalState.GlobalWorkspace
            ),
            Name = "NewProject",
            Spine = "4.3.2",
            Anim = BASE_ANIM,
        };
    }

    public ProjectSettings(
        string settingsPathFile,
        AppSettings appSettings,
        GlobalState globalState
    )
        : this(appSettings, globalState)
    {
        if (!File.Exists(settingsPathFile))
        {
            _settingsData.Path = settingsPathFile;
            SaveSettings();
        }
        else
        {
            SettingsData? settingsData = ReadSettings(settingsPathFile);
            if (settingsData != null)
            {
                string directoryPath = Path.GetDirectoryName(settingsPathFile);
                string projectName = Path.GetFileName(directoryPath);
                string workspacePath = Path.GetDirectoryName(directoryPath);
                _settingsData = settingsData;
                _settingsData.Name = projectName;
                _settingsData.Path = workspacePath;
            }
        }
    }

    public ProjectSettings(
        string path,
        string name,
        AppSettings appSettings,
        GlobalState globalState
    )
        : this(appSettings, globalState)
    {
        if (!File.Exists(path))
        {
            _settingsData.Path = path;
            _settingsData.Name = name;
            SaveSettings();
        }
        else
        {
            SettingsData? settingsData = ReadSettings(path);
            if (settingsData != null)
            {
                _settingsData = settingsData;
            }
        }
    }

    public SettingsData GetSettingsData()
    {
        return _settingsData;
    }

    public void ExistOrCreateProjectDirs()
    {
        if (!Directory.Exists(_appSettings.appSettings!.Workspace))
        {
            Directory.CreateDirectory(_appSettings.appSettings!.Workspace);
        }

        if (!Directory.Exists(Path.Combine(_appSettings.appSettings!.Workspace, "res")))
        {
            Directory.CreateDirectory(Path.Combine(_appSettings.appSettings!.Workspace, "res"));
        }

        if (
            !File.Exists(
                Path.Combine(_appSettings.appSettings!.Workspace, _globalState.SettingsFileName)
            )
        )
        {
            File.Create(
                    Path.Combine(_appSettings.appSettings!.Workspace, _globalState.SettingsFileName)
                )
                .Close();
        }
    }

    public void SaveSettings()
    {
        string settingsPath = Path.Combine(
            _appSettings.appSettings!.Workspace,
            _globalState.SettingsFileName
        );

        ExistOrCreateProjectDirs();

        File.WriteAllText(
            settingsPath,
            JsonConvert.SerializeObject(_settingsData, _globalState.jsonSettings)
        );
    }

    private void WriteProjectInFile(string filePath, string jsonifyedProject)
    {
        ExistOrCreateProjectDirs();

        _settingsData.Anim = jsonifyedProject;

        ExistOrCreateProjectDirs();

        File.WriteAllText(
            filePath,
            JsonConvert.SerializeObject(_settingsData, _globalState.jsonSettings)
        );
    }

    public void WriteProjectJSON(string jsonifyedProject)
    {
        string settingsPath = Path.Combine(
            _appSettings.appSettings!.Workspace,
            _globalState.SettingsFileName
        );
        WriteProjectInFile(settingsPath, jsonifyedProject);
    }

    public void WriteAutoSave(string jsonifyedProject)
    {
        string settingsPath = Path.Combine(
            _appSettings.appSettings!.Workspace,
            _globalState.AutoSaveFile
        );
        Console.WriteLine(settingsPath);
        WriteProjectInFile(settingsPath, jsonifyedProject);
    }

    private SettingsData? ReadFile(string settingsPath)
    {
        ExistOrCreateProjectDirs();

        var settings = JsonConvert.DeserializeObject<SettingsData>(File.ReadAllText(settingsPath));

        if (settings != null)
        {
            settings.Path = Directory
                .GetParent(Directory.GetParent(settingsPath).FullName)
                .FullName;

            return settings;
        }
        
        return null;
    }

    public SettingsData? ReadSettings()
    {
        string settingsPath = Path.Combine(
            _appSettings.appSettings!.Workspace,
            _globalState.SettingsFileName
        );

        return ReadFile(settingsPath);
    }

    public SettingsData? ReadSettings(string? path)
    {
        if (path != null)
        {
            return ReadFile(path);
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

    [JsonIgnore]
    public required string Name { get; set; }

    [JsonProperty("project_spine")]
    public required string Spine { get; set; }

    [JsonProperty("project_anim")]
    public required string Anim { get; set; }
}
