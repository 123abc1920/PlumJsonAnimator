using System;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Common.Timeline;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
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
    public Canvas? Canvas
    {
        get { return _imageExporter.Canvas; }
        set { _imageExporter.Canvas = value; }
    }

    public int CanvasWidth
    {
        get { return _globalState.CanvasWidth; }
        set
        {
            if (_globalState.CanvasWidth != value)
            {
                _globalState.CanvasWidth = value;
                OnPropertyChanged(nameof(CanvasWidth));
            }
        }
    }

    public int CanvasHeight
    {
        get { return _globalState.CanvasHeight; }
        set
        {
            if (_globalState.CanvasHeight != value)
            {
                _globalState.CanvasHeight = value;
                OnPropertyChanged(nameof(CanvasHeight));
            }
        }
    }

    public int CurrentTab
    {
        get => _globalState.CurrentTab;
        set
        {
            if (_globalState.CurrentTab != value)
            {
                _globalState.CurrentTab = value;
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
        get { return _globalState.JsonError; }
    }

    public int FPS
    {
        get { return _globalState.FPS; }
    }

    public bool DrawBones
    {
        get { return _globalState.DrawBones; }
        set
        {
            if (_globalState.DrawBones != value)
            {
                _globalState.DrawBones = value;
                OnPropertyChanged(nameof(DrawBones));
            }
        }
    }

    public bool SetBasePos
    {
        get { return _globalState.SetBasePos; }
        set
        {
            if (_globalState.SetBasePos != value)
            {
                _globalState.SetBasePos = value;
                OnPropertyChanged(nameof(SetBasePos));
            }
        }
    }

    public bool CaptureMode
    {
        get { return _globalState.CaptureMode; }
        set
        {
            if (_globalState.CaptureMode != value)
            {
                _globalState.CaptureMode = value;
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
                    CurrentProject!.CurrentMode = new TransformMode(_globalState);
                }
                else if (value == "rotate")
                {
                    CurrentProject!.CurrentMode = new RotateMode(_globalState);
                }
                else if (value == "scale")
                {
                    CurrentProject!.CurrentMode = new ScaleMode(_globalState);
                }
                else if (value == "shear")
                {
                    CurrentProject!.CurrentMode = new ShearMode(_globalState);
                }
                else
                {
                    CurrentProject!.CurrentMode = new NoMode(_globalState);
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
        get => _globalState.LastSaveTime;
        set => _globalState.LastSaveTime = value;
    }

    public object CurrentInfoPanel { get; set; }

    public string IsTransformModeActive => TransformMode;

    public Bone? CurrentBone
    {
        get => _globalState.CurrentBone;
        set
        {
            if (_globalState.CurrentBone != value)
            {
                _globalState.CurrentBone = value;
                _globalState.CurrentBone?.UpdateSlots();

                if (_globalState.CurrentBone != null)
                {
                    if (_globalState.CurrentBone.IsBone)
                    {
                        CurrentInfoPanel = _infoPanelFactory.Create(InfoPanelTypes.BONEINFO);
                    }
                    else
                    {
                        CurrentInfoPanel = _infoPanelFactory.Create(InfoPanelTypes.SLOTINFO);
                    }
                }

                OnPropertyChanged(nameof(CurrentBone));
                OnPropertyChanged(nameof(CurrentInfoPanel));
            }
        }
    }

    private const double ZoomStep = 0.1;
    private const double MinZoomCanvas = 0.1;
    private const double MaxZoomCanvas = 5.0;
    public double ZoomCanvas
    {
        get => _globalState.ZoomCanvas;
        set
        {
            if (_globalState.ZoomCanvas != value && value > MinZoomCanvas && value < MaxZoomCanvas)
            {
                _globalState.ZoomCanvas = value;
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
        ViewModelBase viewModel = GetViewModel(viewType);
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
        if (_globalState.CurrentBone != null)
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
    private readonly InfoPanelFactory _infoPanelFactory;

    public MainWindowViewModel(
        IServiceProvider serviceProvider,
        AppSettings appSettings,
        ProjectFilesManager projectManager,
        GlobalState globalState,
        ImageExporter imageExporter,
        LocalizationService localizationService,
        Dialogs dialogs,
        PlumApp plumApp,
        AttachmentFactory attachmentFactory,
        InfoPanelFactory infoPanelFactory
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
        _attachmentFactory = attachmentFactory;
        _infoPanelFactory = infoPanelFactory;

        globalState.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GlobalState.LastSaveTime))
                OnPropertyChanged(nameof(LastSaveTime));
        };

        var appSettingsVM = (AppSettingsViewModel)GetViewModel(DialogType.SETTINGS);
        appSettingsVM.CurrentTheme = appSettingsVM.Themes[0];

        _globalState.TimeUpdated += () =>
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
