using System.IO;
using System.Linq;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Resources;
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
    private readonly Engine _engine;

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
        Prettify prettify,
        Engine engine
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
        _engine = engine;
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

    public void RunAnimation()
    {
        this._engine.runAnimation(CurrentProject?.CurrentAnimation);
    }

    public string GetMessage(LocalizationConsts constStr)
    {
        return Localization.GetMessage(constStr);
    }

    public void SaveProject()
    {
        string anim = JsonConvert.SerializeObject(
            this._jsonCode.generateJSONData(CurrentProject),
            GlobalState.jsonSettings
        );
        ProjectSettings.WriteAnimation(anim);
        Popups.ShowPopup(
            GetMessage(LocalizationConsts.SAVED),
            GetMessage(LocalizationConsts.INFO_MESSAGE)
        );
    }

    public void AddRes(string[] paths)
    {
        this._projectManager.GetProjectDir(CurrentProject);

        foreach (string p in paths)
        {
            string resName = "img" + CurrentProject?.Resources.Count.ToString();
            string ext = this._projectManager.CopyRes(resName, p, CurrentProject);
            if (ext != "")
            {
                ImageRes image = new ImageRes(
                    this._projectManager,
                    this.GlobalState,
                    Path.Combine(CurrentProject?.GetProjectPath(), "res", $"{resName}{ext}"),
                    resName,
                    ext
                );
                CurrentProject.Resources.Add(image);
            }
        }
    }

    public void DropSlotToBone(int id, Res res)
    {
        Bone bone = CurrentProject.MainSkeleton.GetBoneById(id);
        if (bone != null)
        {
            Slot s = new Slot(this.GlobalState, bone);
            CurrentProject.Slots.Add(s);
            CurrentProject.CurrentSkin.BindSlotAttachment(s, new ImageAttachment((ImageRes)res));
            bone.UpdateSlots();
        }
    }

    public async void OpenProject(Window win)
    {
        var path = await this._projectManager.OpenProjectDialog(win);
        Project? result = this._projectManager.OpenProject(path);
        if (result != null)
        {
            CurrentProject = result;
        }
    }

    public void RenameProject(SettingsData settingsData)
    {
        settingsData.Anim = CurrentProject!.Code;

        var oldName = CurrentProject!.Name;
        var oldPath = CurrentProject.ProjectPath;

        var oldDir = Path.Combine(oldPath, oldName);
        var newDir = Path.Combine(CurrentProject.ProjectPath, settingsData.Name);

        this._projectManager.CopyDir(oldDir, newDir);

        CurrentProject.SetupProjectSettings(settingsData);
        ProjectSettings.UpdateSettings(CurrentProject);
        AppSettings.ChangeProject(newDir);

        this._projectManager.MoveRes(CurrentProject);

        ProjectSettings.WriteSettings();

        Popups.ShowPopup(
            GetMessage(LocalizationConsts.SAVED),
            GetMessage(LocalizationConsts.INFO_MESSAGE)
        );
    }

    public bool NewProject(string? projectName, string? projectPath)
    {
        Project? result = this._projectManager.NewProject(projectName, projectPath);
        if (result != null)
        {
            this.CurrentProject = result;
            return true;
        }
        return false;
    }
}
