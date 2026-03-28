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
