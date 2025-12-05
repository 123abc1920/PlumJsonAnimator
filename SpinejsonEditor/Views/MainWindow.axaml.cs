using System;
using System.Linq;
using AnimEngine;
using AnimModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Constants;
using TransformModes;
using TreeModel;

namespace SpinejsonEditor.Views;

public partial class MainWindow : Window
{
    private bool _isDragging = false;
    private IBone? selectedBone = null;
    private DispatcherTimer _animationLoop = new DispatcherTimer();

    public MainWindow()
    {
        InitializeComponent();
    }

    public void initViews()
    {
        DispatcherTimer _gameLoop = new DispatcherTimer();
        _gameLoop.Interval = TimeSpan.FromMilliseconds(1000 / 60.0);
        _gameLoop.Tick += UpdateCanvas;
        _gameLoop.Start();

        DragDrop.SetAllowDrop(boneTreeView, true);
        boneTreeView.AddHandler(DragDrop.DropEvent, OnTreeViewDrop);

        _animationLoop.Interval = TimeSpan.FromSeconds(1);
        _animationLoop.Tick += AnimationLoop_Tick;
    }

    private void UpdateCanvas(object? sender, EventArgs e)
    {
        mainCanvas.Children.Clear();
        ConstantsClass.currentProject?.drawSlots(mainCanvas);
        ConstantsClass.currentProject?.mainSkeleton?.drawSkeleton(mainCanvas);
        Engine.runAnimation(ConstantsClass.currentProject.GetAnimation());
    }

    private void AnimationLoop_Tick(object? sender, EventArgs e)
    {
        Timeline.CurrentTime = Math.Round(ConstantsClass.currentProject.GetAnimation().currentTime);
        Timeline.InvalidateVisual();
    }

    private void Add_Bone(object sender, RoutedEventArgs e)
    {
        Node? selectedItem = boneTreeView.SelectedItem as Node;
        if (selectedItem != null && selectedItem.isBone == true)
        {
            ConstantsClass.currentProject?.mainSkeleton?.addBone(selectedItem.id);
            ConstantsClass.viewModel?.AddNode(
                ConstantsClass.currentProject.mainSkeleton.getLast(),
                ConstantsClass.currentProject.mainSkeleton.getId(),
                boneTreeView.SelectedItem
            );
        }
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
            Slot image = new Slot(p, ConstantsClass.currentProject.Slots.Count, p);
            Constants.ConstantsClass.currentProject.Slots.Add(image);
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
        ConstantsClass.currentProject.currentMode = new TransformMode();
    }

    private void Set_Rotate_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject.currentMode = new RotateMode();
    }

    private void Set_Scale_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject.currentMode = new ScaleMode();
    }

    private void Press_Canvas(object sender, PointerPressedEventArgs e)
    {
        var canvas = (Canvas)sender;
        var point = e.GetPosition(canvas);

        Node? selectedItem = boneTreeView.SelectedItem as Node;
        if (selectedItem != null)
        {
            if (selectedItem.isBone == true)
            {
                var id = selectedItem.id;
                selectedBone = ConstantsClass.currentProject?.mainSkeleton?.getBone(id);
            }
            else
            {
                var id = selectedItem.id;
                selectedBone = ConstantsClass.currentProject?.GetSlot(id);
            }
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
            Constants.ConstantsClass.currentProject?.currentMode.transform(
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

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Constants.ConstantsClass.currentProject.Name = projectName.Text;
            Constants.ConstantsClass.currentProject.ProjectPath = projectPath.Text;
        }
    }

    private void OnSlotPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is Slot slot)
        {
            var data = new DataObject();
            data.Set("Slot", slot);
            DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
        }
    }

    private void OnTreeViewDrop(object sender, DragEventArgs e)
    {
        if (e.Data.Get("Slot") is Slot slot)
        {
            var position = e.GetPosition(boneTreeView);
            var element = boneTreeView.InputHitTest(position) as Visual;

            while (element != null)
            {
                if (element is TreeViewItem item && item.DataContext is Node node)
                {
                    if (node.isBone == true)
                    {
                        Bone bone = ConstantsClass.currentProject.mainSkeleton.getBone(node.id);
                        if (bone != null)
                        {
                            if (bone.slot != null)
                            {
                                bone.slot.BoundedBone = null;
                            }
                            slot.BoundedBone = bone;
                            bone.slot = slot;
                            Node slotNode = new Node(false, slot.Title, slot.id, node.parent);
                            ConstantsClass.viewModel?.AddNode(slotNode, node.parent);
                        }
                        return;
                    }
                }
                element = element.Parent as Visual;
            }

            Console.WriteLine("Drop outside any node");
        }
    }

    private void Play_Animation(object sender, RoutedEventArgs e)
    {
        if (ConstantsClass.currentProject.GetAnimation().playOrPause() == true)
        {
            this._animationLoop.Start();
        }
        else
        {
            this._animationLoop.Stop();
        }
    }

    private void Add_New_Animation(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject.addAnimation();
    }
}
