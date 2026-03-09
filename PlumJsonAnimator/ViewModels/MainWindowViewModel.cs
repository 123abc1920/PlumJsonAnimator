using System;
using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public Project CurrentProject { get; set; }
    public JsonError JsonErrorObj { get; set; }
    private Bone? _currentBone;
    public IRenamable? RedactObj { get; set; } = null;
    public List<string> Themes { get; set; } = new List<string>() { "light", "dark" };
    private string _currTheme = "light";
    public string CurrentTheme
    {
        get => _currTheme;
        set
        {
            if (_currTheme != value)
            {
                _currTheme = value;
                this.globalState.theme = _currTheme;
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
        get => _currentBone;
        set
        {
            if (_currentBone != value)
            {
                _currentBone = value;
                this.globalState.currentBone = _currentBone;
                OnPropertyChanged(nameof(CurrentBone));
            }
            else
            {
                this.globalState.currentBone = null;
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

    public MainWindowViewModel(
        AppSettings appSettings,
        ProjectSettings projectSettings,
        ProjectManager projectManager,
        GlobalState globalState,
        JsonCode jsonCode,
        Interpolation interpolation
    )
    {
        this.appSettings = appSettings;
        this.projectSettings = projectSettings;
        this.projectManager = projectManager;
        this.globalState = globalState;
        this.jsonCode = jsonCode;
        this.interpolation = interpolation;

        CurrentTheme = Themes[0];
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

    private void OnTick()
    {
        TickRequested?.Invoke(this, EventArgs.Empty);
    }

    public void initProgram()
    {
        this.globalState.currentProject = new Project(this.globalState, this.interpolation);
        CurrentProject = this.globalState.currentProject;
        JsonErrorObj = this.globalState.jsonError;

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
}
