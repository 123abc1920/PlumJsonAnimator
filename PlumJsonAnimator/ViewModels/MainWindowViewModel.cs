using System;
using System.Windows.Input;
using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.States;
using PlumJsonAnimator.Common.Timeline;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Common.TransformModes;
using PlumJsonAnimator.Models.Factories;
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
    public Control? Canvas
    {
        get { return _imageExporter.Canvas; }
        set { _imageExporter.Canvas = value; }
    }

    public int CanvasWidth
    {
        get { return GlobalState.CanvasWidth; }
        set
        {
            if (GlobalState.CanvasWidth != value)
            {
                GlobalState.CanvasWidth = value;
                OnPropertyChanged(nameof(CanvasWidth));
            }
        }
    }

    public int CanvasHeight
    {
        get { return GlobalState.CanvasHeight; }
        set
        {
            if (GlobalState.CanvasHeight != value)
            {
                GlobalState.CanvasHeight = value;
                OnPropertyChanged(nameof(CanvasHeight));
            }
        }
    }

    public int CurrentTab
    {
        get => GlobalState.CurrentTab;
        set
        {
            if (GlobalState.CurrentTab != value)
            {
                GlobalState.CurrentTab = value;
                CurrentBone = null;
                OnPropertyChanged(nameof(CurrentTab));
            }
        }
    }

    public TimelineControl? Timeline;
    public double CurrentTime
    {
        get { return CurrentProject.CurrentAnimation.CurrentTime; }
        set
        {
            if (CurrentProject.CurrentAnimation.CurrentTime != value)
            {
                CurrentProject.CurrentAnimation.CurrentTime = value;
                CurrentProject.CurrentAnimation.SetupBones();
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
        get { return GlobalState.JsonError; }
    }

    public int FPS
    {
        get { return GlobalState.FPS; }
    }

    public bool DrawBones
    {
        get { return GlobalState.DrawBones; }
        set
        {
            if (GlobalState.DrawBones != value)
            {
                GlobalState.DrawBones = value;
                OnPropertyChanged(nameof(DrawBones));
            }
        }
    }

    public bool SetBasePos
    {
        get { return GlobalState.SetBasePos; }
        set
        {
            if (GlobalState.SetBasePos != value)
            {
                GlobalState.SetBasePos = value;
                OnPropertyChanged(nameof(SetBasePos));
            }
        }
    }

    public bool CaptureMode
    {
        get { return GlobalState.CaptureMode; }
        set
        {
            if (GlobalState.CaptureMode != value)
            {
                GlobalState.CaptureMode = value;
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
                    CurrentProject!.CurrentMode = new TransformMode(GlobalState);
                }
                else if (value == "rotate")
                {
                    CurrentProject!.CurrentMode = new RotateMode(GlobalState);
                }
                else if (value == "scale")
                {
                    CurrentProject!.CurrentMode = new ScaleMode(GlobalState);
                }
                else if (value == "shear")
                {
                    CurrentProject!.CurrentMode = new ShearMode(GlobalState);
                }
                else
                {
                    CurrentProject!.CurrentMode = new NoMode(GlobalState);
                }

                OnPropertyChanged(nameof(TransformMode));
                OnPropertyChanged(nameof(IsTransformModeActive));
            }
        }
    }

    private EasingTypes _easingMode = EasingTypes.LINEAR;
    public EasingTypes EasingMode
    {
        get => _easingMode;
        set
        {
            if (_easingMode != value)
            {
                _easingMode = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime LastSaveTime
    {
        get => GlobalState.LastSaveTime;
        set => GlobalState.LastSaveTime = value;
    }

    public string IsTransformModeActive => TransformMode;

    public Bone? CurrentBone
    {
        get => GlobalState.SelectedItem as Bone;
        set
        {
            if (value is null)
            {
                GlobalState.SelectedItem = null;
                OnPropertyChanged(nameof(CurrentBone));
                return;
            }

            if (value is Bone bone && GlobalState.SelectedItem != bone)
            {
                GlobalState.SelectedItem = (IPresentableItem?)bone;
                bone.UpdateSlots();
                OnPropertyChanged(nameof(CurrentBone));
            }
        }
    }

    private const double ZoomStep = 0.1;
    private const double MinZoomCanvas = 0.1;
    private const double MaxZoomCanvas = 5.0;
    public double ZoomCanvas
    {
        get => GlobalState.ZoomCanvas;
        set
        {
            if (GlobalState.ZoomCanvas != value && value > MinZoomCanvas && value < MaxZoomCanvas)
            {
                GlobalState.ZoomCanvas = value;
                CanvasWidth = (int)(GlobalState.BaseCanvasSize * value);
                CanvasHeight = (int)(GlobalState.BaseCanvasSize * value);
                OnPropertyChanged(nameof(ZoomCanvas));
            }
        }
    }

    public bool CanGenerateProject()
    {
        return PlumApp.CanGenerateProject();
    }

    public void RegenerateProject()
    {
        PlumApp.RegenerateProject();
    }

    public void SetMainWin(Window window)
    {
        _dialogs.MainWin = window;
    }

    public async void ShowDialog(string title, Window owner, DialogType viewType)
    {
        ViewModelBase viewModel = _viewModelFactory.GetViewModel(viewType);
        _dialogs.ShowDialog(title, viewModel, owner, viewType);
    }

    public ExportResult exportSpineJson(string outFolder)
    {
        return PlumApp.ExportSpineJson(outFolder);
    }

    public ExportResult importSpineJson(string inputFile)
    {
        return PlumApp.ImportSpineJson(inputFile);
    }

    public void AddRes(string[] paths)
    {
        PlumApp.AddRes(paths);
    }

    public void DropSlotToBone(int id, Res res)
    {
        PlumApp.DropImageToBone(id, res);
    }

    public async void OpenProject(Window win)
    {
        PlumApp.OpenProject(win);
    }

    public string Prettify(string text)
    {
        return PlumApp.Prettify(text);
    }

    public void Transform(double a, double b)
    {
        if (GlobalState.SelectedItem is Bone bone)
        {
            PlumApp.Transform(a, b);
        }
    }

    public ICommand AddBoneView { get; }
    public ICommand RenameRes { get; }
    public ICommand DeleteRes { get; }
    public ICommand RenameBone { get; }
    public ICommand DeleteBone { get; }
    public ICommand SetTransformMode { get; }
    public ICommand AddAnimation { get; }
    public ICommand AddSkin { get; }
    public ICommand DeleteAnimation { get; }
    public ICommand DeleteSkin { get; }
    public ICommand SaveProject { get; }
    public ICommand PrevKeyFrame { get; }
    public ICommand NextKeyFrame { get; }
    public ICommand AddKeyFrame { get; }
    public ICommand DeleteKeyFrame { get; }
    public ICommand PlayAnim { get; }
    public ICommand ZoomCanvasComm { get; }
    public ICommand ToggleTransformModeCommand { get; }
    public ICommand SetEasingModeCommand { get; }
    public ICommand Undo { get; }
    public ICommand Redo { get; }

    public readonly AttachmentFactory _attachmentFactory;
    private readonly ViewModelFactory _viewModelFactory;
    public PanelState PanelState { get; set; }

    public MainWindowViewModel(
        AppSettings appSettings,
        GlobalState globalState,
        ImageExporter imageExporter,
        LocalizationService localizationService,
        Dialogs dialogs,
        PlumApp plumApp,
        AttachmentFactory attachmentFactory,
        PanelState panelState,
        ViewModelFactory viewModelFactory
    )
        : base(globalState, dialogs, appSettings, localizationService, imageExporter, plumApp)
    {
        _attachmentFactory = attachmentFactory;
        PanelState = panelState;
        _viewModelFactory = viewModelFactory;

        globalState.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Common.Constants.GlobalState.LastSaveTime))
                base.OnPropertyChanged(nameof(LastSaveTime));
        };

        var appSettingsVM = (AppSettingsViewModel)
            _viewModelFactory.GetViewModel(DialogType.SETTINGS);
        appSettingsVM.CurrentTheme = appSettingsVM.Themes[0];

        GlobalState.TimeUpdated += () =>
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
            PlumApp.DeleteBone(CurrentBone);
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

        SaveProject = new Command(_ =>
        {
            bool isSuccess = PlumApp.SaveProject();
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
                    CurrentProject.CurrentAnimation.CurrentTime,
                    CurrentProject.CurrentMode.Type,
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
                    CurrentProject.CurrentAnimation.CurrentTime,
                    CurrentProject.CurrentMode.Type,
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
            PlumApp.RunAnimation();
        });

        ZoomCanvasComm = new Command(parameter =>
        {
            if (parameter is string isZoomPlus)
            {
                if (isZoomPlus == "true")
                {
                    ZoomCanvas += ZoomStep;
                }
                else
                {
                    ZoomCanvas -= ZoomStep;
                }
            }
        });
        SetEasingModeCommand = new Command(_ =>
        {
            CurrentProject?.CurrentAnimation?.ChangeCurrentEasingMode(
                CurrentBone,
                _easingMode,
                CurrentProject.CurrentMode.Type
            );
        });

        Undo = new Command(_ =>
        {
            PlumApp.Undo();
        });
        Redo = new Command(_ =>
        {
            PlumApp.Redo();
        });
    }
}
