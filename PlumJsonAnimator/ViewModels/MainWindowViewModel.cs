using System;
using System.Drawing;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.Timeline;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator.ViewModels;

// TODO: docs
// TODO: add factory pattern
public partial class MainWindowViewModel : ViewModelBase
{
    public Canvas? Canvas
    {
        get { return this.imageExporter.Canvas; }
        set { this.imageExporter.Canvas = value; }
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
    }

    public int FPS
    {
        get { return this.globalState.FPS; }
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

    private bool _isAnimMode = true;
    public bool IsAnimMode
    {
        set
        {
            if (_isAnimMode != value)
            {
                _isAnimMode = value;
                OnPropertyChanged(nameof(IsAnimMode));
            }
        }
        get => _isAnimMode;
    }

    public DateTime LastSaveTime
    {
        get => globalState.LastSaveTime;
        set => globalState.LastSaveTime = value;
    }

    public object CurrentInfoPanel { get; set; }

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
                // TODO: фабрика
                CurrentInfoPanel = new BoneInfo();
                OnPropertyChanged(nameof(CurrentBone));
                OnPropertyChanged(nameof(CurrentInfoPanel));
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

    public bool CanGenerateProject()
    {
        return this.PlumApp.CanGenerateProject();
    }

    public void RegenerateProject()
    {
        this.PlumApp.RegenerateProject();
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
        this.PlumApp.GenerateCode();
    }

    public ExportResult exportSpineJson(string outFolder)
    {
        return this.PlumApp.ExportSpineJson(outFolder);
    }

    public ExportResult importSpineJson(string inputFile)
    {
        return this.PlumApp.ImportSpineJson(inputFile);
    }

    public void AddRes(string[] paths)
    {
        this.PlumApp.AddRes(paths);
    }

    public void DropSlotToBone(int id, Res res)
    {
        this.PlumApp.DropImageToBone(id, res);
    }

    public async void OpenProject(Window win)
    {
        this.PlumApp.OpenProject(win);
    }

    public string Prettify(string text)
    {
        return this.PlumApp.Prettify(text);
    }

    public void Transform(double a, double b)
    {
        if (this.globalState.currentBone != null)
        {
            this.PlumApp.Transform(a, b);
        }
    }

    private void OpenRenameDialog(IRenamable? _renamableObject)
    {
        if (_renamableObject != null)
        {
            var viewModel = (RenameViewModel)GetViewModel(DialogType.RENAME);
            viewModel.RenamableObject = _renamableObject;
            this.dialogs.ShowDialog(
                GetMessage(LocalizationConsts.RENAME),
                viewModel,
                DialogType.RENAME
            );
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
    public ICommand Undo { get; }
    public ICommand Redo { get; }

    private readonly IServiceProvider _serviceProvider;

    public MainWindowViewModel(
        IServiceProvider serviceProvider,
        AppSettings appSettings,
        ProjectSettings projectSettings,
        ProjectFilesManager projectManager,
        GlobalState globalState,
        ImageExporter imageExporter,
        LocalizationService localizationService,
        Dialogs dialogs,
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
        _serviceProvider = serviceProvider;

        globalState.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GlobalState.LastSaveTime))
                OnPropertyChanged(nameof(LastSaveTime));
        };

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
                PlumApp.AddBone(selectedItem);
            }
        });
        RenameRes = new Command(parameter =>
        {
            if (parameter is ListBox resList)
            {
                Res? selectedRes = resList.SelectedItem as Res;
                OpenRenameDialog(selectedRes);
            }
        });
        DeleteRes = new Command(parameter =>
        {
            if (parameter is ListBox resList)
            {
                Res? res = resList.SelectedItem as Res;
                PlumApp.DeleteRes(res);
            }
        });
        RenameSlot = new Command(parameter =>
        {
            if (parameter is ListBox SlotsList)
            {
                Slot? selectedSlot = SlotsList.SelectedItem as Slot;
                OpenRenameDialog(selectedSlot);
            }
        });
        RenameBone = new Command(parameter =>
        {
            if (parameter is TreeView boneTreeView)
            {
                Bone? bone = boneTreeView.SelectedItem as Bone;
                OpenRenameDialog(bone);
            }
        });
        DeleteBone = new Command(_ =>
        {
            this.PlumApp.DeleteBone(CurrentBone);
        });
        AddAnimation = new Command(_ =>
        {
            PlumApp.AddAnimation();
        });
        AddSkin = new Command(_ =>
        {
            PlumApp.AddSkin();
        });
        DeleteAnimation = new Command(_ =>
        {
            PlumApp.DeleteAnimation();
        });
        DeleteSkin = new Command(_ =>
        {
            PlumApp.DeleteSkin();
        });
        AddSlot = new Command(_ =>
        {
            PlumApp.AddSlot();
        });
        DeleteSlot = new Command(parameter =>
        {
            if (parameter is ListBox SlotsList)
            {
                Slot? selectedSlot = SlotsList.SelectedItem as Slot;
                PlumApp.DeleteSlot(selectedSlot);
            }
        });

        SaveProject = new Command(_ =>
        {
            bool isSuccess = this.PlumApp.SaveProject();
            if (isSuccess)
            {
                Popups.ShowPopup(
                    GetMessage(LocalizationConsts.SAVED),
                    GetMessage(LocalizationConsts.INFO_MESSAGE)
                );
            }
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
            PlumApp.AddKeyFrame();
        });
        DeleteKeyFrame = new Command(_ =>
        {
            PlumApp.DeleteKeyFrame();
        });

        PlayAnim = new Command(_ =>
        {
            this.PlumApp.RunAnimation();
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

        Undo = new Command(_ =>
        {
            this.PlumApp.Undo();
        });
        Redo = new Command(_ =>
        {
            this.PlumApp.Redo();
        });
    }
}
