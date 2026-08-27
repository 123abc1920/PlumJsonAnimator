using System;
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
    public GlobalState GlobalState { get; }
    public LocalizationService Localization { get; }

    private readonly Interpolation _interpolation;
    private readonly ProjectFilesManager _fileManager;
    private readonly JsonCode _jsonCode;
    private readonly JsonValidator _jsonValidator;
    private readonly JsonExport _jsonExport;
    private readonly Prettify _prettify;
    private readonly Engine _engine;
    private readonly ImageExporter _imageExporter;
    private readonly HistoryManager _historyManager;
    private readonly AutoSaver _autoSaver;
    private readonly CanvasRenderer _canvasUpdater;

    public PlumApp(
        AppSettings appSettings,
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
        HistoryManager historyManager,
        AutoSaver autoSaver,
        CanvasRenderer canvasUpdater
    )
    {
        AppSettings = appSettings;
        GlobalState = globalState;
        Localization = localization;

        _interpolation = interpolation;
        _fileManager = projectManager;
        _jsonCode = jsonCode;
        _jsonValidator = jsonValidator;
        _jsonExport = jsonExport;
        _prettify = prettify;
        _engine = engine;
        _imageExporter = imageExporter;
        _historyManager = historyManager;
        _autoSaver = autoSaver;
        _canvasUpdater = canvasUpdater;
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
        GlobalState.lastSaveTime = DateTime.Now;

        var projectWorkspace = AppSettings.appSettings.Workspace;
        ProjectSettings projectSettings = new ProjectSettings(
            Path.Combine(projectWorkspace, GlobalState.SETTINGS_FILE_NAME),
            AppSettings,
            GlobalState
        );

        InitProject(new Project(projectSettings, GlobalState, _interpolation, Localization));

        _autoSaver.StartAutoSaveAsync(GlobalState.autoSaveSec);
    }

    private void InitProject(Project project)
    {
        GlobalState.CurrentProject = project;

        _fileManager.LoadRes(GlobalState.CurrentProject);

        ValidResult validateResult = _jsonCode.Regenerate(GlobalState.CurrentProject, true);
        this.GlobalState.jsonError.IsOk = validateResult.IsOk;

        this._historyManager.Clear();

        project.SaveProjectSettings();
    }

    public async void OpenProject(Window win)
    {
        var path = await this._fileManager.OpenProjectDialog(win);

        if (path == "" || path == null)
            return;

        ProjectSettings projectSettings = new ProjectSettings(path, AppSettings, GlobalState);
        AppSettings.ChangeProject(projectSettings.GetSettingsData());

        Project newProject = new Project(
            projectSettings,
            GlobalState,
            this._interpolation,
            Localization
        );

        AppSettings.SaveSettings();

        InitProject(newProject);
    }

    public bool CanGenerateProject()
    {
        return this._canvasUpdater.CanGenerateProject();
    }

    public void RegenerateProject()
    {
        this._jsonCode.Regenerate(GlobalState.CurrentProject, true);
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
        GlobalState.CurrentProject?.SaveProject(this._jsonCode);
        return true;
    }

    public void AddRes(string[] paths)
    {
        this._fileManager.GetProjectDir(GlobalState.CurrentProject);

        foreach (string p in paths)
        {
            string resName = "img" + GlobalState.CurrentProject?.Resources.Count.ToString();
            string ext = this._fileManager.CopyRes(resName, p, GlobalState.CurrentProject);
            if (ext != "")
            {
                Res image = new ImageRes(
                    this._fileManager,
                    this.GlobalState,
                    this._fileManager.GetResDir(GlobalState.CurrentProject, $"{resName}{ext}"),
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

    public bool RenameProject(SettingsData settingsData)
    {
        settingsData.Anim = GlobalState.CurrentProject!.Code;

        var oldName = GlobalState.CurrentProject!.Name;
        var oldPath = GlobalState.CurrentProject.ProjectPath;

        var oldDir = Path.Combine(oldPath, oldName);
        var newDir = Path.Combine(GlobalState.CurrentProject.ProjectPath, settingsData.Name);

        this._fileManager.CopyDir(oldDir, newDir);

        GlobalState.CurrentProject.SetupProjectSettings(settingsData);
        AppSettings.ChangeProject(settingsData);

        this._fileManager.MoveRes(GlobalState.CurrentProject);

        GlobalState.CurrentProject.SaveProjectSettings();

        return true;
    }

    public bool NewProject(string? projectName, string? projectPath)
    {
        Project? result = this._fileManager.NewProject(projectName, projectPath);
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
            this._fileManager.DeleteResource(res.Name, res.ext, GlobalState.CurrentProject);
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

    public void Transform(double a, double b)
    {
        GlobalState.CurrentProject?.currentMode.Transform(GlobalState.currentBone, a, b);
    }

    public void ChangeBoneStatus(BoneStatus oldBoneStatus, BoneStatus newBoneStatus, bool isAnim)
    {
        ChangeBoneStatusCommand changeBoneStatusCommand = new ChangeBoneStatusCommand(
            GlobalState.currentBone,
            oldBoneStatus,
            newBoneStatus,
            GlobalState.CurrentProject.CurrentAnimation,
            isAnim,
            GlobalState.CurrentProject.CurrentAnimation.currentTime
        );
        this._historyManager.DoCommand(changeBoneStatusCommand);
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
