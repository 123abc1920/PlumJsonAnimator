using System.Threading.Tasks;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class ExportPanelGIFViewModel : ViewModelBase
{
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
        ProjectManager projectManager,
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
        ) { }

    public async Task<ExportResult> ExportAsGif(double start, double end, string outputFile)
    {
        ExportResult result = await this.imageExporter.ExportAsGif(start, end, outputFile);
        return result;
    }
}
