using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Constants;
using SpinejsonEditor.ViewModels;
using TreeModel;

namespace SpinejsonEditor.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DispatcherTimer _gameLoop = new DispatcherTimer();
        _gameLoop.Interval = TimeSpan.FromMilliseconds(16);
        _gameLoop.Tick += UpdateCanvas;
        _gameLoop.Start();
    }

    private void UpdateCanvas(object? sender, EventArgs e)
    {
        mainCanvas.Children.Clear();
        ConstantsClass.mainSkeleton.drawSkeleton(mainCanvas);
    }

    private void Add_Bone(object sender, RoutedEventArgs e)
    {
        ConstantsClass.mainSkeleton.addBone();
        ConstantsClass.viewModel.AddNode(
            ConstantsClass.mainSkeleton.getLast(),
            boneTreeView.SelectedItem
        );
    }

    private void Rename_Bone(object sender, RoutedEventArgs e)
    {
        if (boneTreeView.SelectedItem is Node selectedNode)
        {
            selectedNode.Title = "NewTitle";
        }
    }
}
