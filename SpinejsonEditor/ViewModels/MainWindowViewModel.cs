using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimModels;
using Constants;
using TreeModel;

namespace SpinejsonEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<Node> Nodes { get; set; }

    public MainWindowViewModel()
    {
        Nodes = new ObservableCollection<Node> { new Node("root", 0) };
    }

    public void AddNode(string title, int id, object? selectedBone)
    {
        if (selectedBone is Node selectedNode)
        {
            selectedNode.SubNodes.Add(new Node(title, id));
        }
        else
        {
            Nodes.Add(new Node(title, id));
        }
    }

    public void initProgram()
    {
        ConstantsClass.mainSkeleton = new Skeleton();
        Console.WriteLine("inited");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
