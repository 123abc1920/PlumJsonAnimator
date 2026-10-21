using System.Threading.Tasks;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class ExportPanelMP4ViewModel : ViewModelBase
{
    private double _progressValue = 0;
    public double ProgressValue
    {
        get => _progressValue;
        set
        {
            if (value != _progressValue)
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }
    }

    public string ExportPath
    {
        get { return _imageExporter.ExportPath; }
        set
        {
            if (_imageExporter.ExportPath != value)
            {
                _imageExporter.ExportPath = value;
                OnPropertyChanged(nameof(ExportPath));
            }
        }
    }

    public string FfmpegPath
    {
        get { return _appSettings._appSettings.Ffmpeg; }
        set
        {
            if (_appSettings._appSettings.Ffmpeg != value || value == null || value == "")
            {
                _appSettings._appSettings.Ffmpeg = value;
                OnPropertyChanged(nameof(FfmpegPath));
            }
        }
    }

    public ExportPanelMP4ViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ImageExporter imageExporter,
        AppSettings appSettings,
        ProjectFilesManager projectManager,
        LocalizationService localizationService,
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
        _imageExporter.ProgressChanged += (sender, percent) =>
        {
            ProgressValue = percent;
        };
    }

    public async Task<ExportResult> ExportAsMp4(
        double start,
        double end,
        string outputFile,
        string ffmpegPath
    )
    {
        return await PlumApp.ExportAsMp4(start, end, outputFile, ffmpegPath);
    }
}
