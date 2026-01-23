using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimModels;
using Constants;
using EngineModels;
using JsonValidator;

namespace SpinejsonEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public Project CurrentProject { get; set; }
    public JsonError JsonErrorObj { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Animation> Animations
    {
        get => CurrentProject.Animations;
        set
        {
            CurrentProject.Animations = value;
            OnPropertyChanged();
        }
    }

    public int CurrentAnimationIndex
    {
        get => CurrentProject.currentAnimation;
        set
        {
            CurrentProject.currentAnimation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentAnimation));
        }
    }

    public Animation CurrentAnimation
    {
        get =>
            CurrentProject.currentAnimation >= 0
            && CurrentProject.currentAnimation < CurrentProject.Animations.Count
                ? CurrentProject.Animations[CurrentProject.currentAnimation]
                : null;
        set
        {
            if (value != null)
            {
                CurrentAnimationIndex = CurrentProject.Animations.IndexOf(value);
            }
        }
    }

    public MainWindowViewModel()
    {
        
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
