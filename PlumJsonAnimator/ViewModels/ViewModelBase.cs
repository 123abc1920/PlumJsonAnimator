using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public class ViewModelBase : ObservableObject, INotifyPropertyChanged
{
    protected GlobalState globalState;
    protected Dialogs dialogs;
    protected ProjectFilesManager projectManager;
    protected AppSettings appSettings;
    protected LocalizationService localizationService;
    protected ImageExporter imageExporter;
    public PlumApp PlumApp { get; set; }

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

    protected ViewModelBase(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
    {
        this.globalState = globalState;
        this.dialogs = dialogs;
        this.projectManager = projectManager;
        this.appSettings = appSettings;
        this.localizationService = localizationService;
        this.imageExporter = imageExporter;
        this.PlumApp = plumApp;

        this.globalState.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GlobalState.CurrentProject))
            {
                OnPropertyChanged(nameof(CurrentProject));
            }
        };
    }

    public Project? CurrentProject
    {
        get => globalState.CurrentProject;
    }

    public void RenameProject(SettingsData settingsData)
    {
        this.PlumApp.RenameProject(settingsData);
    }

    public string GetMessage(LocalizationConsts constStr)
    {
        return this.PlumApp.GetMessage(constStr);
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
