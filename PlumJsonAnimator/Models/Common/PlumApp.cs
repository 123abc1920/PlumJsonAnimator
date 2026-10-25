using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Commands;
using PlumJsonAnimator.Models.Factories;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.Models.Common;

public class PlumApp
{
    public LocalizationService Localization { get; }

    private readonly AppSettings _appSettings;
    private readonly GlobalState _globalState;
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
    private readonly EasingFactory _easingFactory;
    private readonly AttachmentFactory _attachmentFactory;

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
        CanvasRenderer canvasUpdater,
        EasingFactory easingFactory,
        AttachmentFactory attachmentFactory
    )
    {
        Localization = localization;

        _appSettings = appSettings;
        _globalState = globalState;
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
        _easingFactory = easingFactory;
        _attachmentFactory = attachmentFactory;
    }

    public void Start()
    {
        Localization.LoadLangs();
        Localization.LoadLangResorce();

        _appSettings.ReadSettings();

        _globalState.CaptureArea = _appSettings.CreateCaptureArea(
            _globalState.CanvasWidth,
            _globalState.CanvasHeight
        );

        _globalState.LastSaveTime = DateTime.Now;

        var projectWorkspace = _appSettings._appSettings.Workspace;
        ProjectSettings projectSettings = new ProjectSettings(
            Path.Combine(projectWorkspace, _globalState.SettingsFileName),
            _appSettings,
            _globalState
        );

        InitProject(
            new Project(
                projectSettings,
                _globalState,
                _interpolation,
                Localization,
                _easingFactory,
                _attachmentFactory
            )
        );

        _autoSaver.StartAutoSaveAsync(_globalState.AutoSaveSec);
    }

    private void InitProject(Project project)
    {
        _globalState.CurrentProject = project;

        _fileManager.LoadRes(_globalState.CurrentProject);

        RegenerateResult regenerateResult = _jsonCode.Regenerate(_globalState.CurrentProject, true);
        _globalState.JsonError.IsOk = regenerateResult.IsOk;

        _historyManager.Clear();
    }

    public async void OpenProject(Window win)
    {
        var path = await _fileManager.OpenProjectDialog(win);

        if (path == "" || path == null)
            return;

        ProjectSettings projectSettings = new ProjectSettings(path, _appSettings, _globalState);
        _appSettings.ChangeProject(projectSettings.GetSettingsData());

        Project newProject = new Project(
            projectSettings,
            _globalState,
            _interpolation,
            Localization,
            _easingFactory,
            _attachmentFactory
        );

        InitProject(newProject);
    }

    public bool CanGenerateProject()
    {
        return _canvasUpdater.CanGenerateProject();
    }

    public void RegenerateProject()
    {
        _jsonCode.Regenerate(_globalState.CurrentProject, true);
    }

    public ExportResult ExportSpineJson(string outFolder)
    {
        return _jsonExport.exportSpineJson(outFolder, _globalState.CurrentProject);
    }

    public ExportResult ImportSpineJson(string inputFile)
    {
        return _jsonExport.importSpineJson(inputFile, _globalState.CurrentProject);
    }

    public string Prettify(string text)
    {
        if (text == null)
        {
            return "";
        }
        return _prettify.prettify(text);
    }

    public void RunAnimation()
    {
        _engine.runAnimation(_globalState.CurrentProject?.CurrentAnimation);
    }

    public string GetMessage(LocalizationConsts constStr)
    {
        return Localization.GetMessage(constStr);
    }

    public bool SaveProject()
    {
        _globalState.CurrentProject?.SaveProject(_jsonCode);
        return true;
    }

    public void AddRes(string[] paths)
    {
        _fileManager.GetProjectDir(_globalState.CurrentProject);

        foreach (string p in paths)
        {
            string resName = "img" + _globalState.CurrentProject?.Resources.Count.ToString();
            string ext = _fileManager.CopyRes(resName, p, _globalState.CurrentProject);
            if (ext != "")
            {
                Res image = new ImageRes(
                    _fileManager,
                    _globalState,
                    _fileManager.GetResDir(_globalState.CurrentProject, $"{resName}{ext}"),
                    resName,
                    ext
                );
                _globalState.CurrentProject?.AddRes(image);
            }
        }
    }

    public void DropImageToBone(int id, Res res)
    {
        Bone bone = _globalState.CurrentProject.MainSkeleton.GetBoneById(id);
        if (bone != null)
        {
            DropImageToBoneCommand dropImageToBoneCommand = new DropImageToBoneCommand(
                bone,
                _globalState.CurrentProject,
                res,
                new Slot(_globalState, bone)
            );
            _historyManager.DoCommand(dropImageToBoneCommand);
        }
    }

    public bool RenameProject(SettingsData settingsData)
    {
        settingsData.Anim = _globalState.CurrentProject!.Code;

        var oldName = _globalState.CurrentProject!.Name;
        var oldPath = _globalState.CurrentProject.ProjectPath;

        var oldDir = Path.Combine(oldPath, oldName);
        var newDir = Path.Combine(_globalState.CurrentProject.ProjectPath, settingsData.Name);

        _fileManager.CopyDir(oldDir, newDir);

        _globalState.CurrentProject.SetupProjectSettings(settingsData);
        _appSettings.ChangeProject(settingsData);

        _fileManager.MoveRes(_globalState.CurrentProject);

        _globalState.CurrentProject.SaveProjectSettings();

        return true;
    }

    public bool NewProject(string? projectName, string? projectPath)
    {
        if (projectName != null && projectPath != null)
        {
            _globalState.CurrentProject?.SaveProjectSettings();

            ProjectSettings projectSettings = new ProjectSettings(
                projectPath,
                projectName,
                _appSettings,
                _globalState
            );

            _appSettings.ChangeProject(projectSettings.GetSettingsData());

            Project newProject = new Project(
                projectSettings,
                _globalState,
                _interpolation,
                Localization,
                _easingFactory,
                _attachmentFactory
            );

            _globalState.CurrentProject = newProject;

            return true;
        }
        return false;
    }

    public bool SaveSettings(AppSettingsData data)
    {
        _appSettings.SetSettings(data);
        Localization.LoadLangResorce(data.Lang);

        return true;
    }

    // TODO: паттерн стратегия
    public async Task<ExportResult> ExportAsJpg(double start, double end, string outputFolder)
    {
        ExportResult result = await _imageExporter.ExportAsJpg(
            start,
            end,
            outputFolder,
            _globalState.CurrentProject
        );
        return result;
    }

    public async Task<ExportResult> ExportAsGif(double start, double end, string outputFile)
    {
        ExportResult result = await _imageExporter.ExportAsGif(
            start,
            end,
            outputFile,
            _globalState.CurrentProject
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
        ExportResult result = await _imageExporter.ExportAsMp4(
            start,
            end,
            outputFile,
            ffmpegPath,
            _globalState.CurrentProject
        );
        return result;
    }

    public async Task<ExportResult> ExportAsPng(double start, double end, string outputFolder)
    {
        ExportResult result = await _imageExporter.ExportAsPng(
            start,
            end,
            outputFolder,
            _globalState.CurrentProject
        );
        return result;
    }

    public void DeleteRes(Res? res)
    {
        if (res != null)
        {
            foreach (Skin s in _globalState.CurrentProject.Skins)
            {
                s.RemoveResIfContains(res);
            }
            _globalState.CurrentProject.Resources.Remove(res);
            _fileManager.DeleteResource(res.Name, res.Ext, _globalState.CurrentProject);
        }
    }

    public void DeleteBone(Bone? bone)
    {
        DeleteBoneCommand command = new DeleteBoneCommand(bone, _globalState.CurrentProject);
        _historyManager.DoCommand(command);
    }

    public void AddBone(Bone? selectedBone)
    {
        AddBoneCommand command = new AddBoneCommand(selectedBone, _globalState.CurrentProject);
        _historyManager.DoCommand(command);
    }

    public void AddAnimation()
    {
        AddAnimationCommand addAnimationCommand = new AddAnimationCommand(
            _globalState.CurrentProject
        );
        _historyManager.DoCommand(addAnimationCommand);
    }

    public void AddSkin()
    {
        AddSkinCommand addSkinCommand = new AddSkinCommand(_globalState.CurrentProject);
        _historyManager.DoCommand(addSkinCommand);
    }

    public void DeleteAnimation()
    {
        DeleteAnimationCommand deleteAnimationCommand = new DeleteAnimationCommand(
            _globalState.CurrentProject,
            _globalState.CurrentProject.CurrentAnimation
        );
        _historyManager.DoCommand(deleteAnimationCommand);
    }

    public void DeleteSkin()
    {
        DeleteSkinCommand deleteSkinCommand = new DeleteSkinCommand(
            _globalState.CurrentProject,
            _globalState.CurrentProject.CurrentSkin
        );
        _historyManager.DoCommand(deleteSkinCommand);
    }

    public void AddSlot()
    {
        Bone? bone = _globalState.CurrentBone;
        if (bone != null)
        {
            Slot s = new Slot(_globalState, bone);
            AddSlotCommand addSlotCommand = new AddSlotCommand(
                _globalState.CurrentProject,
                s,
                bone
            );
            _historyManager.DoCommand(addSlotCommand);
        }
    }

    public void DeleteSlot(Slot? selectedSlot)
    {
        if (selectedSlot != null)
        {
            DeleteSlotCommand deleteSlotCommand = new DeleteSlotCommand(
                _globalState.CurrentProject,
                selectedSlot,
                _globalState.CurrentBone
            );
            _historyManager.DoCommand(deleteSlotCommand);
        }
    }

    public void AddKeyFrame()
    {
        if (_globalState.CurrentBone != null)
        {
            AddKeyFrameCommand addKeyFrameCommand = new AddKeyFrameCommand(
                _globalState.CurrentProject.CurrentAnimation,
                _globalState.CurrentBone,
                _globalState.CurrentProject.CurrentMode.Type,
                _easingFactory.CreateEasing(_globalState.CurrentEasingType)
            );
            _historyManager.DoCommand(addKeyFrameCommand);
        }
    }

    public void DeleteKeyFrame()
    {
        if (_globalState.CurrentBone != null)
        {
            DeleteKeyFrameCommand deleteKeyFrameCommand = new DeleteKeyFrameCommand(
                _globalState.CurrentProject.CurrentAnimation,
                _globalState.CurrentBone,
                _globalState.CurrentProject.CurrentMode.Type
            );
            _historyManager.DoCommand(deleteKeyFrameCommand);
        }
    }

    public void Rename(string newName, IRenamable renamableObject)
    {
        var oldName = renamableObject.GetName;
        RenameCommand renameCommand = new RenameCommand(renamableObject, oldName, newName);
        _historyManager.DoCommand(renameCommand);
    }

    public void Transform(double a, double b)
    {
        _globalState.CurrentProject?.CurrentMode.Transform(
            _globalState.CurrentBone,
            a,
            b,
            _easingFactory.CreateEasing(_globalState.CurrentEasingType)
        );
    }

    public void ChangeBoneStatus(BoneStatus oldBoneStatus, BoneStatus newBoneStatus, bool isAnim)
    {
        ChangeBoneStatusCommand changeBoneStatusCommand = new ChangeBoneStatusCommand(
            _globalState.CurrentBone,
            oldBoneStatus,
            newBoneStatus,
            _globalState.CurrentProject.CurrentAnimation,
            isAnim,
            _globalState.CurrentProject.CurrentAnimation.CurrentTime
        );
        _historyManager.DoCommand(changeBoneStatusCommand);
    }

    public void Undo()
    {
        _historyManager.Undo();
    }

    public void Redo()
    {
        _historyManager.Redo();
    }
}
