using System.Collections.Generic;
using System.ComponentModel;
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
    public GlobalState _globalState;
    protected Dialogs _dialogs;
    protected ProjectFilesManager _projectManager;
    protected AppSettings _appSettings;
    protected LocalizationService _localizationService;
    protected ImageExporter _imageExporter;
    public PlumApp PlumApp { get; set; }

    public List<string> Langs
    {
        get => _localizationService.Langs;
    }
    public string CurrentLang
    {
        get => _localizationService._currentLang;
        set
        {
            if (_localizationService._currentLang != value)
            {
                _localizationService._currentLang = value;
                OnPropertyChanged(nameof(CurrentLang));
            }
        }
    }

    public CaptureArea? GetCaptureArea()
    {
        return _globalState.CaptureArea;
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
        _globalState = globalState;
        _dialogs = dialogs;
        _projectManager = projectManager;
        _appSettings = appSettings;
        _localizationService = localizationService;
        _imageExporter = imageExporter;
        PlumApp = plumApp;

        _globalState.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GlobalState.CurrentProject))
            {
                OnPropertyChanged(nameof(CurrentProject));
            }
        };
    }

    public Project? CurrentProject
    {
        get => _globalState.CurrentProject;
    }

    public void RenameProject(SettingsData settingsData)
    {
        bool isSuccess = PlumApp.RenameProject(settingsData);

        if (isSuccess)
        {
            Popups.ShowPopup(
                GetMessage(LocalizationConsts.SAVED),
                GetMessage(LocalizationConsts.INFO_MESSAGE)
            );
        }
    }

    public string GetMessage(LocalizationConsts constStr)
    {
        return PlumApp.GetMessage(constStr);
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
