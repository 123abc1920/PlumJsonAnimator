using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

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
    public event EventHandler? TickRequested;

    private AppSettings appSettings;
    private ProjectSettings projectSettings;
    private ProjectManager projectManager;
    private GlobalState globalState;
    private JsonCode jsonCode;
    private Interpolation interpolation;
    private ImageExporter imageExporter;

    public MainWindowViewModel(
        AppSettings appSettings,
        ProjectSettings projectSettings,
        ProjectManager projectManager,
        GlobalState globalState,
        JsonCode jsonCode,
        Interpolation interpolation,
        ImageExporter imageExporter
    )
    {
        this.appSettings = appSettings;
        this.projectSettings = projectSettings;
        this.projectManager = projectManager;
        this.globalState = globalState;
        this.jsonCode = jsonCode;
        this.interpolation = interpolation;
        this.imageExporter = imageExporter;

        CurrentTheme = Themes[0];

        AddBoneView = new Command.Command(parameter =>
        {
            if (parameter is TreeView treeView)
            {
                Bone? selectedItem = treeView.SelectedItem as Bone;
                if (selectedItem != null && selectedItem.isBone)
                {
                    CurrentProject?.MainSkeleton?.addBone(selectedItem.id);
                }
            }
        });
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

    public void initProgram()
    {
        this.globalState.currentProject = new Project(this.globalState, this.interpolation);

        this.appSettings.ReadSettings();
        this.projectSettings.ReadSettings();
        this.projectManager.LoadRes();

        CurrentTheme = Themes[GetCurrThemeInd(this.appSettings.GetTheme())];

        ProjectValidResult validateResult = this.jsonCode.regenerate();
        if (!validateResult.IsOk)
        {
            Popups.ShowPopup("Возникли проблемы в json коде, невозможно восстановить проект");
        }
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

    public void RenameProject(string oldDir, string newDir)
    {
        this.projectManager.RenameProject(oldDir, newDir);
    }

    public void CopyDir(string oldDir, string newDir)
    {
        this.projectManager.CopyDir(oldDir, newDir);
    }

    public void SaveSettings()
    {
        this.appSettings.SaveSettings();
    }

    public void WriteSettings()
    {
        this.projectSettings.WriteSettings();
    }

    public ICommand AddBoneView { get; }
}
