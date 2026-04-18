using System.Threading.Tasks;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class ExportPanelGIFViewModel : ViewModelBase
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
        get { return this.imageExporter.ExportPath; }
        set
        {
            if (this.imageExporter.ExportPath != value)
            {
                this.imageExporter.ExportPath = value;
                OnPropertyChanged(nameof(ExportPath));
            }
        }
    }

    public ExportPanelGIFViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ImageExporter imageExporter,
        ProjectSettings projectSettings,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService
    )
        : base(
            globalState,
            dialogs,
            projectSettings,
            projectManager,
            appSettings,
            localizationService,
            imageExporter
        )
    {
        this.imageExporter.ProgressChanged += (sender, percent) =>
        {
            ProgressValue = percent;
        };
    }

    public async Task<ExportResult> ExportAsGif(double start, double end, string outputFile)
    {
        ExportResult result = await this.imageExporter.ExportAsGif(
            start,
            end,
            outputFile,
            this.globalState.CurrentProject
        );
        return result;
    }
}
