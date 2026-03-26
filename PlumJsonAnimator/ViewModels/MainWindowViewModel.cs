using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.Timeline;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

// TODO: docs
public partial class MainWindowViewModel : ViewModelBase
{
    public Canvas? Canvas
    {
        get { return this.imageExporter.canvas; }
        set
        {
            if (this.imageExporter.canvas != value)
            {
                this.imageExporter.canvas = value;
                OnPropertyChanged(nameof(CurrentProject));
            }
        }
    }

    public int CanvasWidth
    {
        get { return this.globalState.canvasWidth; }
        set
        {
            if (this.globalState.canvasWidth != value)
            {
                this.globalState.canvasWidth = value;
                OnPropertyChanged(nameof(CanvasWidth));
            }
        }
    }

    public int CanvasHeight
    {
        get { return this.globalState.canvasHeight; }
        set
        {
            if (this.globalState.canvasHeight != value)
            {
                this.globalState.canvasHeight = value;
                OnPropertyChanged(nameof(CanvasHeight));
            }
        }
    }

    public TimelineControl? Timeline;
    public double CurrentTime
    {
        get { return this.CurrentProject.CurrentAnimation.currentTime; }
        set
        {
            if (this.CurrentProject.CurrentAnimation.currentTime != value)
            {
                this.CurrentProject.CurrentAnimation.currentTime = value;
                this.CurrentProject.CurrentAnimation.SetupBones();
                foreach (Slot s in CurrentProject.Slots)
                {
                    s.UpdateDrawOrderOffset();
                }
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
    }

    public Project? CurrentProject
    {
        get { return this.globalState.currentProject; }
        set
        {
            if (this.globalState.currentProject != value)
            {
                this.globalState.currentProject = value;
                OnPropertyChanged(nameof(CurrentProject));
            }
        }
    }
    public JsonError JsonErrorObj
    {
        get { return this.globalState.jsonError; }
        set
        {
            if (this.globalState.jsonError != value)
            {
                this.globalState.jsonError = value;
                OnPropertyChanged(nameof(JsonErrorObj));
            }
        }
    }
    public int FPS
    {
        get { return this.globalState.FPS; }
        set
        {
            if (this.globalState.FPS != value)
            {
                this.globalState.FPS = value;
                OnPropertyChanged(nameof(FPS));
            }
        }
    }

    public bool DrawBones
    {
        get { return this.globalState.drawBones; }
        set
        {
            if (this.globalState.drawBones != value)
            {
                this.globalState.drawBones = value;
                OnPropertyChanged(nameof(DrawBones));
            }
        }
    }

    public bool CaptureMode
    {
        get { return this.globalState.captureMode; }
        set
        {
            if (this.globalState.captureMode != value)
            {
                this.globalState.captureMode = value;
                OnPropertyChanged(nameof(CaptureMode));
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
                OnPropertyChanged(nameof(FPS));
            }
        }
    }

    public string FfmpegPath
    {
        get { return this.appSettings.appSettings.Ffmpeg; }
        set
        {
            if (this.appSettings.appSettings.Ffmpeg != value || value == null || value == "")
            {
                this.appSettings.appSettings.Ffmpeg = value;
                OnPropertyChanged(nameof(FfmpegPath));
            }
        }
    }
    public IRenamable? RedactObj { get; set; } = null;
    public List<string> Themes { get; set; } = new List<string>() { "light", "dark" };
    public string CurrentTheme
    {
        get => this.globalState.theme;
        set
        {
            if (this.globalState.theme != value)
            {
                this.globalState.theme = value;
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }
    }
    public string _modeName = "";
    public string ModeName
    {
        get => _modeName;
        set
        {
            if (_modeName != value)
            {
                _modeName = value;
                OnPropertyChanged(nameof(ModeName));
            }
        }
    }

    public Bone? CurrentBone
    {
        get => this.globalState.currentBone;
        set
        {
            if (this.globalState.currentBone != value)
            {
                this.globalState.currentBone = value;
                this.globalState.currentBone?.UpdateSlots();
                OnPropertyChanged(nameof(CurrentBone));
            }
            else
            {
                this.globalState.currentBone = null;
            }
        }
    }

    public Animation? CurrentAnimation
    {
        get => CurrentProject!.CurrentAnimation;
        set
        {
            if (CurrentProject!.CurrentAnimation != value)
            {
                CurrentProject!.CurrentAnimation = value;
                OnPropertyChanged(nameof(CurrentBone));
            }
        }
    }

    public void AddBone(string title, int id, object? selectedBone)
    {
        if (selectedBone is Bone selectedNode)
        {
            selectedNode.addChildren(new Bone(this.globalState, selectedNode));
        }
    }

    public void AddBone(Bone newBone, Bone? parent)
    {
        if (parent != null)
        {
            parent.Children.Add(newBone);
        }
    }

    private int GetCurrThemeInd(string theme)
    {
        for (int i = 0; i < Themes.Count; i++)
        {
            if (Themes[i] == theme)
            {
                return i;
            }
        }

        return 0;
    }

    public bool CanGenerateProject()
    {
        JsonErrorObj.ErrorText = Validate(CurrentProject.Code);
        if (JsonErrorObj.isOk)
        {
            ProjectValidResult validateResult = this.jsonCode.regenerate(CurrentProject);
            if (!validateResult.IsOk)
            {
                JsonErrorObj.ErrorText = validateResult.Message;
                return false;
            }
        }
        return true;
    }

    public string Validate(string text)
    {
        return this.jsonValidator.validate(text);
    }

    public void GenerateCode()
    {
        this.jsonCode.generateCode(CurrentProject);
    }

    public void initProgram()
    {
        this.globalState.currentProject = new Project(this.globalState, this.interpolation);

        this.appSettings.ReadSettings();
        this.projectSettings.ReadSettings();

        CurrentProject?.SetupProjectSettings(this.projectSettings.GetSettingsData());

        this.projectManager.LoadRes(CurrentProject);

        CurrentTheme = Themes[GetCurrThemeInd(this.appSettings.GetTheme())];

        ProjectValidResult validateResult = this.jsonCode.regenerate(CurrentProject);
        if (!validateResult.IsOk)
        {
            Popups.ShowPopup("Возникли проблемы в json коде, невозможно восстановить проект");
        }

        this.globalState.captureArea = this.appSettings.CreateCaptureArea(
            CanvasWidth,
            CanvasHeight
        );
    }

    public CaptureArea? GetCaptureArea()
    {
        return this.globalState.captureArea;
    }

    public bool NewProject(string? projectName, string? projectPath)
    {
        Project? result = this.projectManager.NewProject(projectName, projectPath);
        if (result != null)
        {
            CurrentProject = result;
            return true;
        }
        return false;
    }

    public async Task<ExportResult> ExportAsGif(double start, double end, string outputFile)
    {
        ExportResult result = await this.imageExporter.ExportAsGif(start, end, outputFile);
        return result;
    }

    public async Task<ExportResult> ExportAsJpg(double start, double end, string outputFolder)
    {
        ExportResult result = await this.imageExporter.ExportAsJpg(start, end, outputFolder);
        return result;
    }

    public async Task<ExportResult> ExportAsMp4(
        double start,
        double end,
        string outputFile,
        string ffmpegPath
    )
    {
        ExportResult result = await this.imageExporter.ExportAsMp4(
            start,
            end,
            outputFile,
            ffmpegPath
        );
        return result;
    }

    public async Task<ExportResult> ExportAsPng(double start, double end, string outputFolder)
    {
        ExportResult result = await this.imageExporter.ExportAsPng(start, end, outputFolder);
        return result;
    }

    public ExportResult exportSpineJson(string outFolder)
    {
        return this.jsonExport.exportSpineJson(outFolder);
    }

    public ExportResult importSpineJson(string inputFile)
    {
        return this.jsonExport.importSpineJson(inputFile);
    }

    public void RenameProject(SettingsData settingsData)
    {
        settingsData.Anim = CurrentProject!.Code;

        var oldName = CurrentProject!.Name;
        var oldPath = CurrentProject.ProjectPath;

        var oldDir = Path.Combine(oldPath, oldName);
        var newDir = Path.Combine(CurrentProject.ProjectPath, settingsData.Name);

        this.projectManager.CopyDir(oldDir, newDir);

        CurrentProject.SetupProjectSettings(settingsData);
        this.projectSettings.UpdateSettings(CurrentProject);
        this.appSettings.ChangeProject(newDir);

        this.projectManager.MoveRes(CurrentProject);

        this.projectSettings.WriteSettings();

        Popups.ShowPopup("Saved");
    }

    public void SaveSettings(AppSettingsData data)
    {
        this.appSettings.SetSettings(data);
        Popups.ShowPopup("Saved");
    }

    public void WriteSettings()
    {
        this.projectSettings.WriteSettings();
    }

    public void AddRes(string[] paths)
    {
        this.projectManager.CreateProjectDir();

        foreach (string p in paths)
        {
            string resName = "img" + CurrentProject.Resources.Count.ToString();
            string ext = this.projectManager.CopyRes(resName, p);
            ImageRes image = new ImageRes(
                this.projectManager,
                this.globalState,
                Path.Combine(CurrentProject.GetProjectPath(), "res", $"{resName}{ext}"),
                resName,
                ext
            );
            CurrentProject.Resources.Add(image);
        }
    }

    public void DropSlotToBone(int id, Res res)
    {
        Bone bone = CurrentProject.MainSkeleton.getBone(id);
        if (bone != null)
        {
            Slot s = new Slot(this.globalState, bone);
            CurrentProject.Slots.Add(s);
            CurrentProject.CurrentSkin.BindSlotAttachment(s, new ImageAttachment((ImageRes)res));
            bone.UpdateSlots();
        }
    }

    public async void OpenProject(Window win)
    {
        var path = await this.projectManager.OpenProjectDialog(win);
        Project? result = this.projectManager.OpenProject(path);
        if (result != null)
        {
            CurrentProject = result;
        }
    }

    public string Prettify(string text)
    {
        if (text == null)
        {
            return "";
        }
        return this.prettify.prettify(text);
    }

    private void DeleteBoneReqursion(Bone? bone)
    {
        if (bone != null && bone.Parent != null)
        {
            CurrentProject?.MainSkeleton?.Bones.Remove(bone);
            bone.Parent.Children.Remove(bone);
            foreach (Bone b in bone.Children.ToList())
            {
                DeleteBoneReqursion(b);
            }
        }
    }

    public ICommand AddBoneView { get; }
    public ICommand RenameRes { get; }
    public ICommand DeleteRes { get; }
    public ICommand RenameSlot { get; }
    public ICommand RenameBone { get; }
    public ICommand DeleteBone { get; }
    public ICommand SetTransformMode { get; }
    public ICommand AddAnimation { get; }
    public ICommand AddSkin { get; }
    public ICommand DeleteAnimation { get; }
    public ICommand DeleteSkin { get; }
    public ICommand AddSlot { get; }
    public ICommand DeleteSlot { get; }
    public ICommand SaveProject { get; }
    public ICommand PrevKeyFrame { get; }
    public ICommand NextKeyFrame { get; }
    public ICommand AddKeyFrame { get; }
    public ICommand DeleteKeyFrame { get; }
    public ICommand PlayAnim { get; }

    private AppSettings appSettings;
    private ProjectSettings projectSettings;
    private ProjectManager projectManager;
    private GlobalState globalState;
    private JsonCode jsonCode;
    private Interpolation interpolation;
    private ImageExporter imageExporter;
    private TransformModeFactory transformModeFactory;
    public Prettify prettify;
    public JsonExport jsonExport;
    public JsonValidator jsonValidator;
    public Engine engine;

    public MainWindowViewModel(
        AppSettings appSettings,
        ProjectSettings projectSettings,
        ProjectManager projectManager,
        GlobalState globalState,
        JsonCode jsonCode,
        Interpolation interpolation,
        ImageExporter imageExporter,
        TransformModeFactory transformModeFactory,
        Prettify prettify,
        JsonExport jsonExport,
        JsonValidator jsonValidator,
        Engine engine
    )
    {
        this.appSettings = appSettings;
        this.projectSettings = projectSettings;
        this.projectManager = projectManager;
        this.globalState = globalState;
        this.jsonCode = jsonCode;
        this.interpolation = interpolation;
        this.imageExporter = imageExporter;
        this.transformModeFactory = transformModeFactory;
        this.prettify = prettify;
        this.jsonExport = jsonExport;
        this.jsonValidator = jsonValidator;
        this.engine = engine;

        CurrentTheme = Themes[0];

        this.globalState.TimeUpdated += () =>
        {
            foreach (Slot s in CurrentProject.Slots)
            {
                s.UpdateDrawOrderOffset();
            }
            OnPropertyChanged(nameof(CurrentTime));
        };

        AddBoneView = new Command.Command(parameter =>
        {
            if (parameter is TreeView treeView)
            {
                Bone? selectedItem = treeView.SelectedItem as Bone;
                if (selectedItem != null && selectedItem.IsBone)
                {
                    CurrentProject?.MainSkeleton?.addBone(selectedItem.id);
                }
            }
        });
        RenameRes = new Command.Command(parameter =>
        {
            if (parameter is ListBox resList)
            {
                Res selectedRes = resList.SelectedItem as Res;
                if (selectedRes != null)
                {
                    RedactObj = selectedRes;
                    Dialogs.ShowDialog("Rename", this, ViewType.RENAME);
                }
            }
        });
        DeleteRes = new Command.Command(parameter =>
        {
            if (parameter is ListBox resList)
            {
                Res res = resList.SelectedItem as Res;
                if (res != null)
                {
                    foreach (Skin s in CurrentProject.Skins)
                    {
                        s.ContainsAndRemoveRes(res);
                    }
                    CurrentProject.Resources.Remove(res);
                    this.projectManager.DeleteResource(res.Name, res.ext);
                }
            }
        });
        RenameSlot = new Command.Command(parameter =>
        {
            if (parameter is ListBox SlotsList)
            {
                Slot selectedSlot = SlotsList.SelectedItem as Slot;
                if (selectedSlot != null)
                {
                    RedactObj = selectedSlot;
                    Dialogs.ShowDialog("Rename", this, ViewType.RENAME);
                }
            }
        });
        RenameBone = new Command.Command(parameter =>
        {
            if (parameter is TreeView boneTreeView)
            {
                Bone bone = boneTreeView.SelectedItem as Bone;
                if (bone != null)
                {
                    RedactObj = bone;
                    Dialogs.ShowDialog("Rename", this, ViewType.RENAME);
                }
            }
        });
        DeleteBone = new Command.Command(_ =>
        {
            Bone bone = CurrentBone;
            DeleteBoneReqursion(bone);
        });
        SetTransformMode = new Command.Command(parameter =>
        {
            if (parameter is string modeType)
            {
                if (modeType == "transform")
                {
                    CurrentProject.currentMode = this.transformModeFactory.createMode(
                        CurrentProject.currentMode,
                        TransformModesTypes.TRANSLATE
                    );
                }
                if (modeType == "rotate")
                {
                    CurrentProject.currentMode = this.transformModeFactory.createMode(
                        CurrentProject.currentMode,
                        TransformModesTypes.ROTATE
                    );
                }
                if (modeType == "scale")
                {
                    CurrentProject.currentMode = this.transformModeFactory.createMode(
                        CurrentProject.currentMode,
                        TransformModesTypes.SCALE
                    );
                }

                ModeName = CurrentProject.currentMode.name;
            }
        });
        AddAnimation = new Command.Command(_ =>
        {
            CurrentProject?.AddAnimation();
        });
        AddSkin = new Command.Command(_ =>
        {
            CurrentProject?.AddSkin();
        });
        DeleteAnimation = new Command.Command(_ =>
        {
            CurrentProject?.DeleteAnimation();
        });
        DeleteSkin = new Command.Command(_ =>
        {
            CurrentProject?.DeleteSkin();
        });
        AddSlot = new Command.Command(_ =>
        {
            Bone bone = CurrentBone;
            if (bone != null)
            {
                Slot s = new Slot(this.globalState, bone);
                CurrentProject.Slots.Add(s);
                CurrentProject.CurrentSkin.AddSlot(s);
                bone.UpdateSlots();
            }
        });
        DeleteSlot = new Command.Command(parameter =>
        {
            if (parameter is ListBox SlotsList)
            {
                Slot selectedSlot = SlotsList.SelectedItem as Slot;
                if (selectedSlot != null)
                {
                    CurrentProject.Slots.Remove(selectedSlot);
                    CurrentProject.CurrentSkin.DeleteSlot(selectedSlot);
                    CurrentBone.UpdateSlots();
                }
            }
        });

        SaveProject = new Command.Command(_ =>
        {
            string anim = JsonConvert.SerializeObject(
                this.jsonCode.generateJSONData(CurrentProject),
                this.globalState.jsonSettings
            );
            this.projectSettings.WriteAnim(anim);
            Popups.ShowPopup("Saved");
        });

        PrevKeyFrame = new Command.Command(_ =>
        {
            if (CurrentBone != null)
            {
                CurrentTime = CurrentProject.CurrentAnimation.FindKeyFrame(
                    CurrentBone,
                    CurrentProject.CurrentAnimation.currentTime,
                    CurrentProject.currentMode.type,
                    false
                );
            }
        });
        NextKeyFrame = new Command.Command(_ =>
        {
            if (CurrentBone != null)
            {
                CurrentTime = CurrentProject.CurrentAnimation.FindKeyFrame(
                    CurrentBone,
                    CurrentProject.CurrentAnimation.currentTime,
                    CurrentProject.currentMode.type,
                    true
                );
            }
        });
        AddKeyFrame = new Command.Command(_ =>
        {
            if (CurrentBone != null)
            {
                CurrentProject.CurrentAnimation.AddKeyFrame(
                    CurrentBone,
                    CurrentProject.currentMode.type
                );
            }
        });
        DeleteKeyFrame = new Command.Command(_ =>
        {
            if (CurrentBone != null)
            {
                CurrentProject.CurrentAnimation.DeleteKeyFrame(
                    CurrentBone,
                    CurrentProject.currentMode.type
                );
            }
        });

        PlayAnim = new Command.Command(_ =>
        {
            this.engine.runAnimation();
        });
    }
}
