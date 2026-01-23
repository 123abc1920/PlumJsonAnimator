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

namespace SpinejsonEditor.Views;

public partial class MainWindow : Window
{
    private bool _isDragging = false;
    private IBone? selectedBone = null;
    private DispatcherTimer _animationLoop = new DispatcherTimer();
    private int currentTab = 0;

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
        if (currentTab == 0)
        {
            mainCanvas.Children.Clear();
            ConstantsClass.currentProject?.drawSlots(mainCanvas);
            ConstantsClass.currentProject?.MainSkeleton?.drawSkeleton(mainCanvas);
            Engine.runAnimation(ConstantsClass.currentProject.GetAnimation());
        }
        else if (currentTab == 1)
        {
            ConstantsClass.jsonError.ErrorText = JsonValidator.JsonValidator.validate(
                spineJsonText.Text
            );
        }
    }

    private void AnimationLoop_Tick(object? sender, EventArgs e)
    {
        if (currentTab == 0)
        {
            Timeline.CurrentTime = Math.Round(
                ConstantsClass.currentProject.GetAnimation().currentTime
            );
            Timeline.InvalidateVisual();
        }
    }

    private void Add_Bone(object sender, RoutedEventArgs e)
    {
        Bone? selectedItem = boneTreeView.SelectedItem as Bone;
        if (selectedItem != null && selectedItem.isBone == true)
        {
            ConstantsClass.currentProject?.MainSkeleton?.addBone(selectedItem.id);
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
            Slot image = new Slot(ConstantsClass.currentProject.Slots.Count, p);
            Constants.ConstantsClass.currentProject.Slots.Add(image);
        }
    }

    private void Rename_Bone(object sender, RoutedEventArgs e)
    {
        if (selectedBone != null && selectedBone.isBone == true)
        {
            Bone bone = (Bone)selectedBone;
            bone.Name = "NewTitle";
        }
    }

    private void DeleteBone(Bone? bone)
    {
        if (bone != null && bone.Parent != null)
        {
            ConstantsClass.currentProject?.MainSkeleton?.Bones.Remove(bone);
            bone.Parent.Children.Remove(bone);
            foreach (Bone b in bone.Children.ToList())
            {
                DeleteBone(b);
            }
        }
    }

    private void Delete_Bone(object sender, RoutedEventArgs e)
    {
        Bone bone = (Bone)selectedBone;
        DeleteBone(bone);
    }

    private void Delete_Slot(object sender, RoutedEventArgs e)
    {
        Bone bone = (Bone)selectedBone;
        ConstantsClass.currentProject.CurrentSkin.UnbindBoneAndSlot(bone);
    }

    private void Set_Transform_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject.currentMode = TransformModeFactory.createMode(
            ConstantsClass.currentProject.currentMode,
            TransformModesTypes.TRANSLATE
        );
    }

    private void Set_Rotate_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject.currentMode = TransformModeFactory.createMode(
            ConstantsClass.currentProject.currentMode,
            TransformModesTypes.ROTATE
        );
    }

    private void Set_Scale_Mode(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject.currentMode = TransformModeFactory.createMode(
            ConstantsClass.currentProject.currentMode,
            TransformModesTypes.SCALE
        );
    }

    private void Press_Canvas(object sender, PointerPressedEventArgs e)
    {
        var canvas = (Canvas)sender;
        var point = e.GetPosition(canvas);

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
            ConstantsClass.currentProject?.currentMode.transform(
                selectedBone,
                point.X - canvas.Width / 2,
                point.Y - canvas.Height / 2
            );
        }
    }

    private void Release_Canvas(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e) { }

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
                if (element is TreeViewItem item && item.DataContext is Bone Bone)
                {
                    if (Bone.isBone == true)
                    {
                        Bone bone = ConstantsClass.currentProject.MainSkeleton.getBone(Bone.id);
                        if (bone != null)
                        {
                            ConstantsClass.currentProject.CurrentSkin.BindBoneAndSlot(bone, slot);
                        }
                        return;
                    }
                }
                element = element.Parent as Visual;
            }

            Console.WriteLine("Drop outside any Bone");
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
        ConstantsClass.currentProject?.addAnimation();
    }

    private void Add_New_Skin(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject?.AddSkin();
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl tabControl)
        {
            int newIndex = tabControl.SelectedIndex;

            if (newIndex == 0 && Constants.ConstantsClass.jsonError.isOk != true)
            {
                tabControl.SelectionChanged -= TabControl_SelectionChanged;
                tabControl.SelectedIndex = 1;
                tabControl.SelectionChanged += TabControl_SelectionChanged;
                return;
            }

            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                string header = selectedTab.Header?.ToString();

                switch (header)
                {
                    case "Spinejson":
                        currentTab = 1;
                        ConstantsClass.currentProject?.SpinejsonCode.generateCode(
                            ConstantsClass.currentProject
                        );
                        break;

                    case "Анимация":
                        ConstantsClass.currentProject?.SpinejsonCode.regenerate();
                        currentTab = 0;
                        break;
                }
            }
        }
    }

    private void OnBonePointerPressed(object sender, TappedEventArgs e)
    {
        if (sender is StackPanel panel && panel.DataContext is Bone bone)
        {
            if (bone == selectedBone)
            {
                if (bone.HasSlot)
                {
                    selectedBone = bone.Slot;
                    Console.WriteLine("Slot");
                }
                else
                {
                    selectedBone = null;
                    boneTreeView.SelectedItem = null;
                }
            }
            else
            {
                selectedBone = bone;
                Console.WriteLine("Bone");
            }
            e.Handled = true;
        }
    }
}
