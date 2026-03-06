using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimEngine.Models;
using AnimEngine.Project;
using AnimModels;
using Common.Constants;
using SpinejsonGeneration.JsonValidator;

namespace PlumJsonAnimator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public Project CurrentProject { get; set; }
    public JsonError JsonErrorObj { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    private Bone? _currentBone;
    public IRenamable RedactObj { get; set; } = null;
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
                ConstantsClass.theme = _currTheme;
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
                ConstantsClass.currentBone = _currentBone;
                OnPropertyChanged(nameof(CurrentBone));
            }
            else
            {
                ConstantsClass.currentBone = null;
            }
        }
    }

    public MainWindowViewModel()
    {
        CurrentTheme = Themes[0];
    }

    public void AddBone(string title, int id, object? selectedBone)
    {
        if (selectedBone is Bone selectedNode)
        {
            selectedNode.addChildren(new Bone(selectedNode));
        }
    }

    public void AddBone(Bone newBone, Bone? parent)
    {
        if (parent != null)
        {
            parent.Children.Add(newBone);
        }
    }

    public void initProgram()
    {
        ConstantsClass.currentProject = new Project();
        CurrentProject = ConstantsClass.currentProject;
        JsonErrorObj = ConstantsClass.jsonError;
    }

    protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
