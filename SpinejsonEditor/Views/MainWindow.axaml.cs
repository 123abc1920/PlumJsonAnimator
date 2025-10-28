using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AnimModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Constants;
using SlotsView;
using transformModes;
using TreeModel;

namespace SpinejsonEditor.Views;

public partial class MainWindow : Window
{
    private bool _isDragging = false;
    private Bone? selectedBone = null;
    private ObservableCollection<string> SlotImagesTitles { get; set; } =
        new ObservableCollection<string>();

    public MainWindow()
    {
        InitializeComponent();

        DispatcherTimer _gameLoop = new DispatcherTimer();
        _gameLoop.Interval = TimeSpan.FromMilliseconds(16);
        _gameLoop.Tick += UpdateCanvas;
        _gameLoop.Start();

        slotsList.ItemsSource = SlotImagesTitles;
    }

    private void UpdateCanvas(object? sender, EventArgs e)
    {
        mainCanvas.Children.Clear();
        ConstantsClass.mainSkeleton.drawSkeleton(mainCanvas);
        ConstantsClass.drawSlots(mainCanvas);
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

    private async void Add_Image(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions { Title = "Open Image File", AllowMultiple = true }
        );

        string[] paths = [];
        if (files?.Count > 0)
        {
            paths = files.Select(f => f.Path.LocalPath).ToArray();
        }

        foreach (string p in paths)
        {
            SlotImage image = new SlotImage(p, Constants.ConstantsClass.SlotImages.Count, p);
            Constants.ConstantsClass.SlotImages.Add(image);
            SlotImagesTitles.Add(image.Title);
        }
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
            Constants.ConstantsClass.currentMode.transform(
                selectedBone,
                point.X - canvas.Width / 2,
                point.Y - canvas.Height / 2
            );
        }
    }

    private void Release_Canvas(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        selectedBone = null;
    }
}
