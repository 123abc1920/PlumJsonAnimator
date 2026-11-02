using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public class ViewModelBase : ObservableObject, INotifyPropertyChanged
{
    public GlobalState _globalState;
    protected Dialogs _dialogs;
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
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
    {
        _globalState = globalState;
        _dialogs = dialogs;
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

    private bool _isAnimMode = true;
    public bool IsAnimMode
    {
        set
        {
            if (_isAnimMode != value)
            {
                _isAnimMode = value;
                OnPropertyChanged(nameof(IsAnimMode));
            }
        }
        get => _isAnimMode;
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

    // TODO: fix OpenRenameDialog
    protected void OpenRenameDialog(IRenamable? renamableObject)
    {
        if (renamableObject != null)
        {
            // var viewModel = (RenameViewModel)GetViewModel(DialogType.RENAME);
            // viewModel.RenamableObject = renamableObject;
            // _dialogs.ShowDialog(
            //     GetMessage(LocalizationConsts.RENAME),
            //     viewModel,
            //     DialogType.RENAME
            // );
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
