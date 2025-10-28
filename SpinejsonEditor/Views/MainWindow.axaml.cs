using System;
using AnimModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Constants;
using transformModes;
using TreeModel;

namespace SpinejsonEditor.Views;

public partial class MainWindow : Window
{
    private bool _isDragging = false;
    private Bone? selectedBone = null;

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
        Node selectedItem = (Node)boneTreeView.SelectedItem;
        ConstantsClass.mainSkeleton.addBone(selectedItem.id);
        ConstantsClass.viewModel.AddNode(
            ConstantsClass.mainSkeleton.getLast(),
            ConstantsClass.mainSkeleton.getId(),
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

    private void Set_Transform_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentMode = new TransformMode();
    }

    private void Set_Rotate_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentMode = new RotateMode();
    }

    private void Set_Scale_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentMode = new ScaleMode();
    }

    private void Press_Canvas(object sender, PointerPressedEventArgs e)
    {
        var canvas = (Canvas)sender;
        var point = e.GetPosition(canvas);

        Node selectedItem = (Node)boneTreeView.SelectedItem;
        if (selectedItem != null)
        {
            var id = selectedItem.id;
            selectedBone = ConstantsClass.mainSkeleton.getBone(id);
        }

        var properties = e.GetCurrentPoint(canvas).Properties;
        if (properties.IsLeftButtonPressed)
        {
            _isDragging = true;
        }
    }

    private void Move_Canvas(object sender, PointerEventArgs e)
    {
        if (!_isDragging)
            return;

        var canvas = (Canvas)sender;
        var point = e.GetPosition(canvas);

        if (selectedBone != null)
        {
            Constants.ConstantsClass.currentMode.transform(selectedBone, point.X, point.Y);
        }
    }

    private void Release_Canvas(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        selectedBone = null;
    }
}
