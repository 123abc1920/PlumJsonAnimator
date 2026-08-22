using System.Linq;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.SkeletonNameSpace;
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
    private readonly JsonValidator _jsonValidator;
    private readonly JsonExport _jsonExport;
    private readonly Prettify _prettify;

    public PlumApp(
        AppSettings appSettings,
        ProjectSettings projectSettings,
        GlobalState globalState,
        Interpolation interpolation,
        LocalizationService localization,
        ProjectFilesManager projectManager,
        JsonCode jsonCode,
        JsonValidator jsonValidator,
        JsonExport jsonExport,
        Prettify prettify
    )
    {
        AppSettings = appSettings;
        ProjectSettings = projectSettings;
        GlobalState = globalState;
        Localization = localization;

        _interpolation = interpolation;
        _projectManager = projectManager;
        _jsonCode = jsonCode;
        _jsonValidator = jsonValidator;
        _jsonExport = jsonExport;
        _prettify = prettify;
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
        this.GlobalState.jsonError.IsOk = validateResult.IsOk;
    }

    public bool CanGenerateProject()
    {
        this.GlobalState.jsonError.ErrorText = this._jsonValidator.Validate(CurrentProject.Code);
        if (this.GlobalState.jsonError.IsOk)
        {
            ValidResult validateResult = this._jsonCode.Regenerate(CurrentProject, false);
            if (!validateResult.IsOk)
            {
                this.GlobalState.jsonError.ErrorText = validateResult.Message;
                return false;
            }
        }
        return true;
    }

    public void RegenerateProject()
    {
        this._jsonCode.Regenerate(CurrentProject, true);
    }

    public void GenerateCode()
    {
        this._jsonCode.generateCode(CurrentProject);
    }

    public ExportResult ExportSpineJson(string outFolder)
    {
        return this._jsonExport.exportSpineJson(outFolder, CurrentProject);
    }

    public ExportResult ImportSpineJson(string inputFile)
    {
        return this._jsonExport.importSpineJson(inputFile, CurrentProject);
    }

    public void WriteSettings()
    {
        ProjectSettings.WriteSettings();
    }

    public string Prettify(string text)
    {
        if (text == null)
        {
            return "";
        }
        return this._prettify.prettify(text);
    }

    public void DeleteBoneReqursion(Bone? bone)
    {
        if (bone != null && bone.Parent != null)
        {
            foreach (Slot s in bone.Slots)
            {
                CurrentProject?.DeleteSlotFromProject(s);
            }
            foreach (Bone b in bone.Children.ToList())
            {
                DeleteBoneReqursion(b);
            }
            CurrentProject?.DeleteBoneFromProject(bone);
        }
    }
}
