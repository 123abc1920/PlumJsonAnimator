using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimModels;
using Avalonia.Threading;
using Constants;
using EngineModels;
using JsonValidator;
using Renameble;

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
        ConstantsClass.currentProject = new EngineModels.Project();
        CurrentProject = ConstantsClass.currentProject;
        JsonErrorObj = ConstantsClass.jsonError;
    }

    protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
