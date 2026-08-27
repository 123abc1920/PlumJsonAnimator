using System.Threading.Tasks;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class ExportPanelPNGViewModel : ViewModelBase
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

    public ExportPanelPNGViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ImageExporter imageExporter,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
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
        this.imageExporter.ProgressChanged += (sender, percent) =>
        {
            ProgressValue = percent;
        };
    }

    public async Task<ExportResult> ExportAsPng(double start, double end, string outputFolder)
    {
        return await this.PlumApp.ExportAsPng(start, end, outputFolder);
    }
}
