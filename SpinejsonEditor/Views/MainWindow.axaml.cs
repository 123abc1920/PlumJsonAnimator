using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Constants;
using SpinejsonEditor.ViewModels;
using TreeModel;

namespace SpinejsonEditor.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Add_Bone(object sender, RoutedEventArgs e)
    {
        ConstantsClass.mainSkeleton.addBone();
        ConstantsClass.viewModel.AddNode("kkk");
    }
}
