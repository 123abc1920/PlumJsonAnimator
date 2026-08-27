using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class AppSettingsViewModel : ViewModelBase
{
    public string FfmpegPath
    {
        get { return this.appSettings.appSettings.Ffmpeg; }
        set
        {
            if (this.appSettings.appSettings.Ffmpeg != value || value == null || value == "")
            {
                this.appSettings.appSettings.Ffmpeg = value;
                OnPropertyChanged(nameof(FfmpegPath));
            }
        }
    }

    public List<string> Themes { get; set; } = new List<string>() { "light", "dark" };
    public string CurrentTheme
    {
        get => this.globalState.theme;
        set
        {
            if (this.globalState.theme != value)
            {
                this.globalState.theme = value;
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }
    }

    public void SaveSettings(AppSettingsData data)
    {
        bool isSuccess = this.PlumApp.SaveSettings(data);

        if (isSuccess)
        {
            Popups.ShowPopup(
                GetMessage(LocalizationConsts.SAVED),
                GetMessage(LocalizationConsts.INFO_MESSAGE)
            );
        }
    }

    public AppSettingsViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
        : base(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp
        )
    {
        this.CurrentTheme = appSettings.appSettings.Theme;
    }
}
