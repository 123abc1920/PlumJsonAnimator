using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class SpinejsonSettingsViewModel : ViewModelBase
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

    public int GetCurrThemeInd(string theme)
    {
        for (int i = 0; i < Themes.Count; i++)
        {
            if (Themes[i] == theme)
            {
                return i;
            }
        }

        return 0;
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

    public SpinejsonSettingsViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectSettings projectSettings,
        ProjectManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter
    )
        : base(
            globalState,
            dialogs,
            projectSettings,
            projectManager,
            appSettings,
            localizationService,
            imageExporter
        ) { }
}
