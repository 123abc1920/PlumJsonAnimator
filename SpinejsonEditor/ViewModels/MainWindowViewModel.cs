using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimModels;
using Constants;
using EngineModels;
using TreeModel;

namespace SpinejsonEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<Node> Nodes { get; set; }
    public Project CurrentProject { get; set; }

    public MainWindowViewModel()
    {
        Nodes = new ObservableCollection<Node> { new Node("root", 0, null) };
    }

    public void AddNode(string title, int id, object? selectedBone)
    {
        if (selectedBone is Node selectedNode)
        {
            selectedNode.SubNodes.Add(new Node(title, id, selectedNode));
        }
        else
        {
            Nodes.Add(new Node(title, id, null));
        }
    }

    public void AddNode(Node newNode, Node? parent)
    {
        if (parent != null)
        {
            parent.SubNodes.Add(newNode);
        }
        else
        {
            Nodes[0].SubNodes.Add(newNode);
        }
    }

    public void initProgram()
    {
        ConstantsClass.currentProject = new EngineModels.Project();
        CurrentProject = ConstantsClass.currentProject;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
