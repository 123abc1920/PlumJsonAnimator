using System.Threading.Tasks;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class ExportPanelJPGViewModel : ViewModelBase
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

    public ExportPanelJPGViewModel(
        GlobalState globalState,
        Dialogs dialogs,
        ImageExporter imageExporter,
        ProjectManager projectManager,
        ProjectSettings projectSettings,
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

    public async Task<ExportResult> ExportAsJpg(double start, double end, string outputFolder)
    {
        ExportResult result = await this.imageExporter.ExportAsJpg(start, end, outputFolder);
        return result;
    }
}
