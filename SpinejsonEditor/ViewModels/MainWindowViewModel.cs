using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimModels;
using Avalonia.Interactivity;
using Constants;
using TreeModel;

namespace SpinejsonEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<Node> Nodes { get; set; }

    public MainWindowViewModel()
    {
        Nodes = new ObservableCollection<Node>
        {
            new Node(
                "Animals",
                new ObservableCollection<Node>
                {
                    new Node(
                        "Mammals",
                        new ObservableCollection<Node>
                        {
                            new Node("Lion"),
                            new Node("Cat"),
                            new Node("Zebra"),
                        }
                    ),
                }
            ),
        };
    }

    public void AddNode(string title)
    {
        Nodes.Add(new Node(title));
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
