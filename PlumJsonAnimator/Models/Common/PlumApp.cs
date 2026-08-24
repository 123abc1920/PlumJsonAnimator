using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Commands;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.Models.Common;

public class PlumApp
{
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
    private readonly ImageExporter _imageExporter;
    private readonly HistoryManager _historyManager;

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
        Engine engine,
        ImageExporter imageExporter,
        HistoryManager historyManager
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
        _imageExporter = imageExporter;
        _historyManager = historyManager;
    }

    public void Start()
    {
        Localization.LoadLangs();
        Localization.LoadLangResorce();

        AppSettings.ReadSettings();
        GlobalState.captureArea = AppSettings.CreateCaptureArea(
            this.GlobalState.canvasWidth,
            this.GlobalState.canvasHeight
        );

        ProjectSettings.ReadSettings();
        InitProject(new Project(GlobalState, _interpolation, Localization));
    }

    private void InitProject(Project project)
    {
        GlobalState.CurrentProject = project;

        GlobalState.CurrentProject.SetupProjectSettings(ProjectSettings.GetSettingsData());
        _projectManager.LoadRes(GlobalState.CurrentProject);

        ValidResult validateResult = _jsonCode.Regenerate(GlobalState.CurrentProject, true);
        this.GlobalState.jsonError.IsOk = validateResult.IsOk;

        this._historyManager.Clear();
    }

    public bool CanGenerateProject()
    {
        this.GlobalState.jsonError.ErrorText = this._jsonValidator.Validate(
            GlobalState.CurrentProject.Code
        );
        if (this.GlobalState.jsonError.IsOk)
        {
            ValidResult validateResult = this._jsonCode.Regenerate(
                GlobalState.CurrentProject,
                false
            );
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
        this._jsonCode.Regenerate(GlobalState.CurrentProject, true);
    }

    public void GenerateCode()
    {
        this._jsonCode.generateCode(GlobalState.CurrentProject);
    }

    public ExportResult ExportSpineJson(string outFolder)
    {
        return this._jsonExport.exportSpineJson(outFolder, GlobalState.CurrentProject);
    }

    public ExportResult ImportSpineJson(string inputFile)
    {
        return this._jsonExport.importSpineJson(inputFile, GlobalState.CurrentProject);
    }

    public string Prettify(string text)
    {
        if (text == null)
        {
            return "";
        }
        return this._prettify.prettify(text);
    }

    public void RunAnimation()
    {
        this._engine.runAnimation(GlobalState.CurrentProject?.CurrentAnimation);
    }

    public string GetMessage(LocalizationConsts constStr)
    {
        return Localization.GetMessage(constStr);
    }

    public bool SaveProject()
    {
        string project = JsonConvert.SerializeObject(
            this._jsonCode.generateJSONData(GlobalState.CurrentProject),
            GlobalState.jsonSettings
        );
        ProjectSettings.WriteProjectJSON(project);

        return true;
    }

    public void AddRes(string[] paths)
    {
        this._projectManager.GetProjectDir(GlobalState.CurrentProject);

        foreach (string p in paths)
        {
            string resName = "img" + GlobalState.CurrentProject?.Resources.Count.ToString();
            string ext = this._projectManager.CopyRes(resName, p, GlobalState.CurrentProject);
            if (ext != "")
            {
                Res image = new ImageRes(
                    this._projectManager,
                    this.GlobalState,
                    this._projectManager.GetResDir(GlobalState.CurrentProject, $"{resName}{ext}"),
                    resName,
                    ext
                );
                GlobalState.CurrentProject?.AddRes(image);
            }
        }
    }

    public void DropImageToBone(int id, Res res)
    {
        Bone bone = GlobalState.CurrentProject.MainSkeleton.GetBoneById(id);
        if (bone != null)
        {
            DropImageToBoneCommand dropImageToBoneCommand = new DropImageToBoneCommand(
                bone,
                GlobalState.CurrentProject,
                res,
                new Slot(this.GlobalState, bone)
            );
            this._historyManager.DoCommand(dropImageToBoneCommand);
        }
    }

    public async void OpenProject(Window win)
    {
        var path = await this._projectManager.OpenProjectDialog(win);

        if (path == "" || path == null)
            return;

        ProjectSettings.ReadSettings(path);
        SettingsData settingsData = ProjectSettings.GetSettingsData();
        AppSettings.ChangeProject(Path.Combine(settingsData.Path, settingsData.Name));

        Project newProject = new Project(
            settingsData.Name,
            settingsData.Path,
            GlobalState,
            this._interpolation,
            Localization
        );

        ProjectSettings.SaveSettings();
        AppSettings.SaveSettings();

        InitProject(newProject);
    }

    public bool RenameProject(SettingsData settingsData)
    {
        settingsData.Anim = GlobalState.CurrentProject!.Code;

        var oldName = GlobalState.CurrentProject!.Name;
        var oldPath = GlobalState.CurrentProject.ProjectPath;

        var oldDir = Path.Combine(oldPath, oldName);
        var newDir = Path.Combine(GlobalState.CurrentProject.ProjectPath, settingsData.Name);

        this._projectManager.CopyDir(oldDir, newDir);

        GlobalState.CurrentProject.SetupProjectSettings(settingsData);
        ProjectSettings.UpdateSettings(GlobalState.CurrentProject);
        AppSettings.ChangeProject(newDir);

        this._projectManager.MoveRes(GlobalState.CurrentProject);

        ProjectSettings.SaveSettings();

        return true;
    }

    public bool NewProject(string? projectName, string? projectPath)
    {
        Project? result = this._projectManager.NewProject(projectName, projectPath);
        if (result != null)
        {
            this.GlobalState.CurrentProject = result;
            return true;
        }
        return false;
    }

    public bool SaveSettings(AppSettingsData data)
    {
        AppSettings.SetSettings(data);
        Localization.LoadLangResorce(data.Lang);

        return true;
    }

    // TODO: паттерн стратегия
    public async Task<ExportResult> ExportAsJpg(double start, double end, string outputFolder)
    {
        ExportResult result = await this._imageExporter.ExportAsJpg(
            start,
            end,
            outputFolder,
            GlobalState.CurrentProject
        );
        return result;
    }

    public async Task<ExportResult> ExportAsGif(double start, double end, string outputFile)
    {
        ExportResult result = await this._imageExporter.ExportAsGif(
            start,
            end,
            outputFile,
            GlobalState.CurrentProject
        );
        return result;
    }

    public async Task<ExportResult> ExportAsMp4(
        double start,
        double end,
        string outputFile,
        string ffmpegPath
    )
    {
        ExportResult result = await this._imageExporter.ExportAsMp4(
            start,
            end,
            outputFile,
            ffmpegPath,
            GlobalState.CurrentProject
        );
        return result;
    }

    public async Task<ExportResult> ExportAsPng(double start, double end, string outputFolder)
    {
        ExportResult result = await this._imageExporter.ExportAsPng(
            start,
            end,
            outputFolder,
            GlobalState.CurrentProject
        );
        return result;
    }

    public void DeleteRes(Res? res)
    {
        if (res != null)
        {
            foreach (Skin s in GlobalState.CurrentProject.Skins)
            {
                s.RemoveResIfContains(res);
            }
            GlobalState.CurrentProject.Resources.Remove(res);
            this._projectManager.DeleteResource(res.Name, res.ext, GlobalState.CurrentProject);
        }
    }

    public void DeleteBone(Bone? bone)
    {
        DeleteBoneCommand command = new DeleteBoneCommand(bone, GlobalState.CurrentProject);
        this._historyManager.DoCommand(command);
    }

    public void AddBone(Bone? selectedBone)
    {
        AddBoneCommand command = new AddBoneCommand(selectedBone, GlobalState.CurrentProject);
        this._historyManager.DoCommand(command);
    }

    public void AddAnimation()
    {
        AddAnimationCommand addAnimationCommand = new AddAnimationCommand(
            GlobalState.CurrentProject
        );
        this._historyManager.DoCommand(addAnimationCommand);
    }

    public void AddSkin()
    {
        AddSkinCommand addSkinCommand = new AddSkinCommand(GlobalState.CurrentProject);
        this._historyManager.DoCommand(addSkinCommand);
    }

    public void DeleteAnimation()
    {
        DeleteAnimationCommand deleteAnimationCommand = new DeleteAnimationCommand(
            GlobalState.CurrentProject,
            GlobalState.CurrentProject.CurrentAnimation
        );
        this._historyManager.DoCommand(deleteAnimationCommand);
    }

    public void DeleteSkin()
    {
        DeleteSkinCommand deleteSkinCommand = new DeleteSkinCommand(
            GlobalState.CurrentProject,
            GlobalState.CurrentProject.CurrentSkin
        );
        this._historyManager.DoCommand(deleteSkinCommand);
    }

    public void AddSlot()
    {
        Bone? bone = GlobalState.currentBone;
        if (bone != null)
        {
            Slot s = new Slot(GlobalState, bone);
            AddSlotCommand addSlotCommand = new AddSlotCommand(GlobalState.CurrentProject, s, bone);
            this._historyManager.DoCommand(addSlotCommand);
        }
    }

    public void DeleteSlot(Slot? selectedSlot)
    {
        if (selectedSlot != null)
        {
            DeleteSlotCommand deleteSlotCommand = new DeleteSlotCommand(
                GlobalState.CurrentProject,
                selectedSlot,
                GlobalState.currentBone
            );
            this._historyManager.DoCommand(deleteSlotCommand);
        }
    }

    public void AddKeyFrame()
    {
        if (GlobalState.currentBone != null)
        {
            AddKeyFrameCommand addKeyFrameCommand = new AddKeyFrameCommand(
                GlobalState.CurrentProject.CurrentAnimation,
                GlobalState.currentBone,
                GlobalState.CurrentProject.currentMode.type
            );
            this._historyManager.DoCommand(addKeyFrameCommand);
        }
    }

    public void DeleteKeyFrame()
    {
        if (GlobalState.currentBone != null)
        {
            DeleteKeyFrameCommand deleteKeyFrameCommand = new DeleteKeyFrameCommand(
                GlobalState.CurrentProject.CurrentAnimation,
                GlobalState.currentBone,
                GlobalState.CurrentProject.currentMode.type
            );
            this._historyManager.DoCommand(deleteKeyFrameCommand);
        }
    }

    public void Rename(string newName, IRenamable renamableObject)
    {
        var oldName = renamableObject.GetName;
        RenameCommand renameCommand = new RenameCommand(renamableObject, oldName, newName);
        this._historyManager.DoCommand(renameCommand);
    }

    public void Undo()
    {
        this._historyManager.Undo();
    }

    public void Redo()
    {
        this._historyManager.Redo();
    }
}
