using System.Threading.Tasks;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class ExportPanelJPGViewModel : ViewModelBase
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

    public ExportPanelJPGViewModel(
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
        _imageExporter.ProgressChanged += (sender, percent) =>
        {
            ProgressValue = percent;
        };
    }

    public async Task<ExportResult> ExportAsJpg(double start, double end, string outputFolder)
    {
        return await PlumApp.ExportAsJpg(start, end, outputFolder);
    }
}
