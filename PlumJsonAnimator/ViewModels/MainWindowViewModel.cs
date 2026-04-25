using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.Timeline;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using static PlumJsonAnimator.Services.JsonCode;

namespace PlumJsonAnimator.ViewModels;

// TODO: docs
// TODO: add factory pattern
public partial class MainWindowViewModel : ViewModelBase
{
    public Canvas? Canvas
    {
        get { return this.imageExporter.Canvas; }
        set
        {
            if (this.imageExporter.Canvas != value)
            {
                this.imageExporter.Canvas = value;
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

    public bool SetBasePos
    {
        get { return this.globalState.setBasePos; }
        set
        {
            if (this.globalState.setBasePos != value)
            {
                this.globalState.setBasePos = value;
                OnPropertyChanged(nameof(SetBasePos));
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

    private string _transformMode;
    public string TransformMode
    {
        get => _transformMode;
        set
        {
            if (_transformMode != value)
            {
                _transformMode = value;

                if (value == "transform")
                {
                    CurrentProject!.currentMode = new TransformMode(this.globalState);
                }
                else if (value == "rotate")
                {
                    CurrentProject!.currentMode = new RotateMode(this.globalState);
                }
                else if (value == "scale")
                {
                    CurrentProject!.currentMode = new ScaleMode(this.globalState);
                }
                else
                {
                    CurrentProject!.currentMode = new NoMode(this.globalState);
                }

                OnPropertyChanged(nameof(TransformMode));
                OnPropertyChanged(nameof(IsTransformModeActive));
            }
        }
    }

    public string IsTransformModeActive => TransformMode;

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

    private const double ZOOM_STEP = 0.1;
    private const double MIN_ZOOM_CANVAS = 0.1;
    private const double MAX_ZOOM_CANVAS = 5.0;
    public double ZoomCanvas
    {
        get => this.globalState.zoomCanvas;
        set
        {
            if (
                this.globalState.zoomCanvas != value
                && value > MIN_ZOOM_CANVAS
                && value < MAX_ZOOM_CANVAS
            )
            {
                this.globalState.zoomCanvas = value;
                CanvasWidth = (int)(GlobalState.BASE_CANVAS_SIZE * value);
                CanvasHeight = (int)(GlobalState.BASE_CANVAS_SIZE * value);
                OnPropertyChanged(nameof(ZoomCanvas));
            }
        }
    }

    public void AddBone(string title, int id, object? selectedBone)
    {
        if (selectedBone is Bone selectedNode)
        {
            selectedNode.AddChildren(
                new Bone(this.globalState, selectedNode, this.localizationService)
            );
        }
    }

    public void AddBone(Bone newBone, Bone? parent)
    {
        if (parent != null)
        {
            parent.Children.Add(newBone);
        }
    }

    public bool CanGenerateProject()
    {
        JsonErrorObj.ErrorText = Validate(CurrentProject.Code);
        if (JsonErrorObj.isOk)
        {
            ValidResult validateResult = this.jsonCode.Regenerate(CurrentProject, false);
            if (!validateResult.IsOk)
            {
                JsonErrorObj.ErrorText = validateResult.Message;
                return false;
            }
        }
        return true;
    }

    public void RegenerateProject()
    {
        this.jsonCode.Regenerate(CurrentProject, true);
    }

    public string Validate(string text)
    {
        return this.jsonValidator.Validate(text);
    }

    public void SetMainWin(Window window)
    {
        this.dialogs.mainWin = window;
    }

    private ViewModelBase GetViewModel(DialogType viewType)
    {
        ViewModelBase viewModel = this;

        if (viewType == DialogType.SETTINGS)
        {
            viewModel = _serviceProvider.GetRequiredService<AppSettingsViewModel>();
        }
        if (viewType == DialogType.NEWPROJECT)
        {
            viewModel = _serviceProvider.GetRequiredService<NewProjectViewModel>();
        }
        if (viewType == DialogType.RENAME)
        {
            viewModel = _serviceProvider.GetRequiredService<RenameViewModel>();
        }
        if (viewType == DialogType.EXPORT_JPG)
        {
            viewModel = _serviceProvider.GetRequiredService<ExportPanelJPGViewModel>();
        }
        if (viewType == DialogType.EXPORT_PNG)
        {
            viewModel = _serviceProvider.GetRequiredService<ExportPanelPNGViewModel>();
        }
        if (viewType == DialogType.EXPORT_GIF)
        {
            viewModel = _serviceProvider.GetRequiredService<ExportPanelGIFViewModel>();
        }
        if (viewType == DialogType.EXPORT_MP4)
        {
            viewModel = _serviceProvider.GetRequiredService<ExportPanelMP4ViewModel>();
        }

        return viewModel;
    }

    public async void ShowDialog(string title, Window owner, DialogType viewType)
    {
        ViewModelBase viewModel = GetViewModel(viewType);
        this.dialogs.ShowDialog(title, viewModel, owner, viewType);
    }

    public void GenerateCode()
    {
        this.jsonCode.generateCode(CurrentProject);
    }

    public void initProgram()
    {
        this.CurrentProject = new Project(
            this.globalState,
            this.interpolation,
            this.localizationService
        );

        this.appSettings.ReadSettings();
        this.projectSettings.ReadSettings();

        CurrentProject?.SetupProjectSettings(this.projectSettings.GetSettingsData());

        this.projectManager.LoadRes(CurrentProject);

        var appSettingsVM = (AppSettingsViewModel)GetViewModel(DialogType.SETTINGS);
        appSettingsVM.CurrentTheme = appSettingsVM.Themes[
            appSettingsVM.GetCurrThemeInd(this.appSettings.GetTheme())
        ];

        this.globalState.captureArea = this.appSettings.CreateCaptureArea(
            CanvasWidth,
            CanvasHeight
        );

        ValidResult validateResult = this.jsonCode.Regenerate(CurrentProject, true);
        this.JsonErrorObj.isOk = validateResult.IsOk;
    }

    public ExportResult exportSpineJson(string outFolder)
    {
        return this.jsonExport.exportSpineJson(outFolder, CurrentProject);
    }

    public ExportResult importSpineJson(string inputFile)
    {
        return this.jsonExport.importSpineJson(inputFile, CurrentProject);
    }

    public void WriteSettings()
    {
        this.projectSettings.WriteSettings();
    }

    public void AddRes(string[] paths)
    {
        this.projectManager.GetProjectDir(CurrentProject);

        foreach (string p in paths)
        {
            string resName = "img" + CurrentProject?.Resources.Count.ToString();
            string ext = this.projectManager.CopyRes(resName, p, CurrentProject);
            if (ext != "")
            {
                ImageRes image = new ImageRes(
                    this.projectManager,
                    this.globalState,
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
    public ICommand ZoomCanvasComm { get; }
    public ICommand ToggleTransformModeCommand { get; }

    private JsonCode jsonCode;
    private Interpolation interpolation;
    private TransformModeFactory transformModeFactory;
    public Prettify prettify;
    public JsonExport jsonExport;
    public JsonValidator jsonValidator;
    public Engine engine;

    private readonly IServiceProvider _serviceProvider;

    public MainWindowViewModel(
        IServiceProvider serviceProvider,
        AppSettings appSettings,
        ProjectSettings projectSettings,
        ProjectFilesManager projectManager,
        GlobalState globalState,
        JsonCode jsonCode,
        Interpolation interpolation,
        ImageExporter imageExporter,
        TransformModeFactory transformModeFactory,
        Prettify prettify,
        JsonExport jsonExport,
        JsonValidator jsonValidator,
        Engine engine,
        LocalizationService localizationService,
        Dialogs dialogs
    )
        : base(
            globalState,
            dialogs,
            projectSettings,
            projectManager,
            appSettings,
            localizationService,
            imageExporter
        )
    {
        _serviceProvider = serviceProvider;

        this.jsonCode = jsonCode;
        this.interpolation = interpolation;
        this.transformModeFactory = transformModeFactory;
        this.prettify = prettify;
        this.jsonExport = jsonExport;
        this.jsonValidator = jsonValidator;
        this.engine = engine;

        var appSettingsVM = (AppSettingsViewModel)GetViewModel(DialogType.SETTINGS);
        appSettingsVM.CurrentTheme = appSettingsVM.Themes[0];

        this.globalState.TimeUpdated += () =>
        {
            foreach (Slot s in CurrentProject.Slots)
            {
                s.UpdateDrawOrderOffset();
            }
            OnPropertyChanged(nameof(CurrentTime));
        };

        ToggleTransformModeCommand = new Command(parameter =>
        {
            if (parameter is string mode)
            {
                if (TransformMode == mode)
                {
                    TransformMode = null;
                }
                else
                {
                    TransformMode = mode;
                }
            }
        });

        AddBoneView = new Command(parameter =>
        {
            if (parameter is TreeView treeView)
            {
                Bone? selectedItem = treeView.SelectedItem as Bone;
                if (selectedItem != null && selectedItem.IsBone)
                {
                    CurrentProject?.MainSkeleton?.AddBoneToParent(selectedItem.id);
                }
            }
        });
        RenameRes = new Command(parameter =>
        {
            if (parameter is ListBox resList)
            {
                Res selectedRes = resList.SelectedItem as Res;
                if (selectedRes != null)
                {
                    var viewModel = (RenameViewModel)GetViewModel(DialogType.RENAME);
                    viewModel.RedactObj = selectedRes;
                    this.dialogs.ShowDialog(
                        GetMessage(LocalizationConsts.RENAME),
                        viewModel,
                        DialogType.RENAME
                    );
                }
            }
        });
        DeleteRes = new Command(parameter =>
        {
            if (parameter is ListBox resList)
            {
                Res res = resList.SelectedItem as Res;
                if (res != null)
                {
                    foreach (Skin s in CurrentProject.Skins)
                    {
                        s.RemoveResIfContains(res);
                    }
                    CurrentProject.Resources.Remove(res);
                    this.projectManager.DeleteResource(res.Name, res.ext, CurrentProject);
                }
            }
        });
        RenameSlot = new Command(parameter =>
        {
            if (parameter is ListBox SlotsList)
            {
                Slot selectedSlot = SlotsList.SelectedItem as Slot;
                if (selectedSlot != null)
                {
                    var viewModel = (RenameViewModel)GetViewModel(DialogType.RENAME);
                    viewModel.RedactObj = selectedSlot;
                    this.dialogs.ShowDialog(
                        GetMessage(LocalizationConsts.RENAME),
                        viewModel,
                        DialogType.RENAME
                    );
                }
            }
        });
        RenameBone = new Command(parameter =>
        {
            if (parameter is TreeView boneTreeView)
            {
                Bone bone = boneTreeView.SelectedItem as Bone;
                if (bone != null)
                {
                    var viewModel = (RenameViewModel)GetViewModel(DialogType.RENAME);
                    viewModel.RedactObj = bone;
                    this.dialogs.ShowDialog(
                        GetMessage(LocalizationConsts.RENAME),
                        viewModel,
                        DialogType.RENAME
                    );
                }
            }
        });
        DeleteBone = new Command(_ =>
        {
            Bone bone = CurrentBone;
            DeleteBoneReqursion(bone);
        });
        AddAnimation = new Command(_ =>
        {
            CurrentProject?.AddAnimation();
        });
        AddSkin = new Command(_ =>
        {
            CurrentProject?.AddSkin();
        });
        DeleteAnimation = new Command(_ =>
        {
            CurrentProject?.DeleteAnimation();
        });
        DeleteSkin = new Command(_ =>
        {
            CurrentProject?.DeleteSkin();
        });
        AddSlot = new Command(_ =>
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
        DeleteSlot = new Command(parameter =>
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

        SaveProject = new Command(_ =>
        {
            string anim = JsonConvert.SerializeObject(
                this.jsonCode.generateJSONData(CurrentProject),
                this.globalState.jsonSettings
            );
            this.projectSettings.WriteAnimation(anim);
            Popups.ShowPopup(
                GetMessage(LocalizationConsts.SAVED),
                GetMessage(LocalizationConsts.INFO_MESSAGE)
            );
        });

        PrevKeyFrame = new Command(_ =>
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
        NextKeyFrame = new Command(_ =>
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
        AddKeyFrame = new Command(_ =>
        {
            if (CurrentBone != null)
            {
                CurrentProject.CurrentAnimation.AddKeyFrame(
                    CurrentBone,
                    CurrentProject.currentMode.type
                );
            }
        });
        DeleteKeyFrame = new Command(_ =>
        {
            if (CurrentBone != null)
            {
                CurrentProject?.CurrentAnimation?.DeleteKeyFrame(
                    CurrentBone,
                    CurrentProject.currentMode.type
                );
            }
        });

        PlayAnim = new Command(_ =>
        {
            this.engine.runAnimation(this?.CurrentProject?.CurrentAnimation);
        });

        ZoomCanvasComm = new Command(parameter =>
        {
            if (parameter is string isZoomPlus)
            {
                if (isZoomPlus == "true")
                {
                    this.ZoomCanvas += ZOOM_STEP;
                }
                else
                {
                    this.ZoomCanvas -= ZOOM_STEP;
                }
            }
        });
    }
}
