using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public class ViewModelBase : ObservableObject, INotifyPropertyChanged
{
    protected GlobalState globalState;
    protected Dialogs dialogs;
    protected ProjectSettings projectSettings;
    protected ProjectManager projectManager;
    protected AppSettings appSettings;
    protected LocalizationService localizationService;
    protected ImageExporter imageExporter;

    public List<string> Langs
    {
        get => this.localizationService.langs;
    }
    public string CurrentLang
    {
        get => this.localizationService.currentLang;
        set
        {
            if (this.localizationService.currentLang != value)
            {
                this.localizationService.currentLang = value;
                OnPropertyChanged(nameof(CurrentLang));
            }
        }
    }

    public CaptureArea? GetCaptureArea()
    {
        return this.globalState.captureArea;
    }

    public void SaveSettings(AppSettingsData data)
    {
        this.appSettings.SetSettings(data);

        this.localizationService.LoadLangResorce(data.Lang);

        Popups.ShowPopup(
            GetMessage(LocalizationConsts.SAVED),
            GetMessage(LocalizationConsts.INFO_MESSAGE)
        );
    }

    protected ViewModelBase(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectSettings projectSettings,
        ProjectManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter
    )
    {
        this.globalState = globalState;
        this.dialogs = dialogs;
        this.projectManager = projectManager;
        this.projectSettings = projectSettings;
        this.appSettings = appSettings;
        this.localizationService = localizationService;
        this.imageExporter = imageExporter;

        this.globalState.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GlobalState.CurrentProject))
            {
                OnPropertyChanged(nameof(CurrentProject));
            }
        };
    }

    public ViewModelBase(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectSettings projectSettings,
        ProjectManager projectManager,
        AppSettings appSettings
    )
    {
        this.globalState = globalState;
        this.dialogs = dialogs;
        this.projectSettings = projectSettings;
        this.projectManager = projectManager;
        this.appSettings = appSettings;
    }

    public Project? CurrentProject
    {
        get => globalState.CurrentProject;
        set
        {
            if (globalState.CurrentProject != value)
            {
                globalState.CurrentProject = value;
                OnPropertyChanged(nameof(CurrentProject));
            }
        }
    }

    public void RenameProject(SettingsData settingsData)
    {
        settingsData.Anim = CurrentProject!.Code;

        var oldName = CurrentProject!.Name;
        var oldPath = CurrentProject.ProjectPath;

        var oldDir = Path.Combine(oldPath, oldName);
        var newDir = Path.Combine(CurrentProject.ProjectPath, settingsData.Name);

        this.projectManager.CopyDir(oldDir, newDir);

        CurrentProject.SetupProjectSettings(settingsData);
        this.projectSettings.UpdateSettings(CurrentProject);
        this.appSettings.ChangeProject(newDir);

        this.projectManager.MoveRes(CurrentProject);

        this.projectSettings.WriteSettings();

        Popups.ShowPopup(
            GetMessage(LocalizationConsts.SAVED),
            GetMessage(LocalizationConsts.INFO_MESSAGE)
        );
    }

    public string GetMessage(LocalizationConsts constStr)
    {
        return this.localizationService.GetMessage(constStr);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null
    )
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
