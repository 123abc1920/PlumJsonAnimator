using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.Models.Common;

public class PlumApp
{
    public Project CurrentProject { get; private set; }
    public AppSettings AppSettings { get; }
    public ProjectSettings ProjectSettings { get; }
    public GlobalState GlobalState { get; }
    public LocalizationService Localization { get; }

    private readonly Interpolation _interpolation;
    private readonly ProjectFilesManager _projectManager;
    private readonly JsonCode _jsonCode;

    public PlumApp(
        AppSettings appSettings,
        ProjectSettings projectSettings,
        GlobalState globalState,
        Interpolation interpolation,
        LocalizationService localization,
        ProjectFilesManager projectManager,
        JsonCode jsonCode
    )
    {
        AppSettings = appSettings;
        ProjectSettings = projectSettings;
        GlobalState = globalState;
        _interpolation = interpolation;
        Localization = localization;
        _projectManager = projectManager;
        _jsonCode = jsonCode;
    }

    public void Start()
    {
        Localization.LoadLangs();
        Localization.LoadLangResorce();

        CurrentProject = new Project(GlobalState, _interpolation, Localization);

        AppSettings.ReadSettings();
        ProjectSettings.ReadSettings();

        CurrentProject.SetupProjectSettings(ProjectSettings.GetSettingsData());
        _projectManager.LoadRes(CurrentProject);

        GlobalState.CurrentProject = CurrentProject;
        GlobalState.captureArea = AppSettings.CreateCaptureArea(
            this.GlobalState.canvasWidth,
            this.GlobalState.canvasHeight
        );

        ValidResult validateResult = _jsonCode.Regenerate(CurrentProject, true);
        this.GlobalState.jsonError.isOk = validateResult.IsOk;
    }
}
