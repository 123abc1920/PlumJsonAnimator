using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimEngine;
using AnimModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Constants;
using ProjectManager;
using Resources;
using SpinejsonEditor.ViewModels;
using TransformModes;

namespace SpinejsonEditor.Views;

public partial class MainWindow : Window
{
    private bool _isDragging = false;
    private Bone selectedBone;
    public Bone? SelectedBone
    {
        get => selectedBone;
        set
        {
            if (selectedBone != value)
            {
                selectedBone = value;

                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.CurrentBone = selectedBone;
                }
            }
        }
    }
    private DispatcherTimer _animationLoop = new DispatcherTimer();
    private int currentTab = 0;

    public MainWindow()
    {
        InitializeComponent();
        this.KeyDown += OnWindowKeyDown;

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainWindowViewModel.CurrentBone))
                {
                    selectedBone = viewModel.CurrentBone;
                }
            };
        }
    }

    public void initViews()
    {
        DispatcherTimer _gameLoop = new DispatcherTimer();
        _gameLoop.Interval = TimeSpan.FromMilliseconds(1000 / 60.0);
        _gameLoop.Tick += UpdateCanvas;
        _gameLoop.Start();

        DragDrop.SetAllowDrop(boneTreeView, true);
        boneTreeView.AddHandler(DragDrop.DropEvent, OnTreeViewDrop);

        AppSettings.ReadSettings();
        ProjectSettings.ProjectSettings.ReadSettings();

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

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            SaveProject(sender, new RoutedEventArgs());
            e.Handled = true;
        }
        if (e.Key == Key.O && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            OpenProject(sender, new RoutedEventArgs());
            e.Handled = true;
        }
        if (e.Key == Key.N && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            NewProject(sender, new RoutedEventArgs());
            e.Handled = true;
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

    private async void Add_Res(object sender, RoutedEventArgs e)
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

        ProjectManager.ProjectManager.CreateProjectDir();

        foreach (string p in paths)
        {
            string resName = "img" + ConstantsClass.currentProject.Resources.Count.ToString();
            string ext = ProjectManager.ProjectManager.CopyRes(resName, p);
            ImageRes image = new ImageRes(
                Path.Combine(ConstantsClass.currentProject.GetProjectPath(), "res"),
                resName,
                ext
            );
            ConstantsClass.currentProject.Resources.Add(image);
        }
    }

    private void DeleteRes(object sender, RoutedEventArgs e)
    {
        Res res = resList.SelectedItem as Res;
        if (res != null)
        {
            foreach (Skin s in ConstantsClass.currentProject.Skins)
            {
                s.ContainsAndRemoveRes(res);
            }
            ConstantsClass.currentProject.Resources.Remove(res);
            ProjectManager.ProjectManager.DeleteResource(res.Name, res.ext);
        }
    }

    private void Rename_Bone(object sender, RoutedEventArgs e)
    {
        if (SelectedBone != null && SelectedBone.isBone == true)
        {
            Bone bone = (Bone)SelectedBone;
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
        Bone bone = (Bone)SelectedBone;
        DeleteBone(bone);
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

        if (SelectedBone != null)
        {
            ConstantsClass.currentProject?.currentMode.transform(
                SelectedBone,
                point.X - canvas.Width / 2,
                point.Y - canvas.Height / 2
            );
        }
    }

    private void Release_Canvas(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }

    private void OnResPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is Res res)
        {
            var data = new DataObject();
            data.Set("Resource", res);
            DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
        }
    }

    private void OnTreeViewDrop(object sender, DragEventArgs e)
    {
        if (e.Data.Get("Resource") is Res res)
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
                            Slot s = new Slot("tesr", bone);
                            ConstantsClass.currentProject.Slots.Add(s);
                            ConstantsClass.currentProject.CurrentSkin.BindSlotAttachment(
                                s,
                                new ImageAttachment((ImageRes)res)
                            );
                            bone.UpdateSlots();
                        }
                        return;
                    }
                }
                element = element.Parent as Visual;
            }
        }
    }

    private void OnListBoxDrop(object sender, DragEventArgs e)
    {
        var listBox = (ListBox)sender;
        var point = e.GetPosition(listBox);
        var item = listBox.InputHitTest(point) as Visual;

        while (item != null && !(item is ListBoxItem))
        {
            item = item.Parent as Visual;
        }

        if (item is ListBoxItem listBoxItem)
        {
            if (e.Data.Get("Resource") is ImageRes imageRes && listBoxItem.DataContext is Slot slot)
            {
                ConstantsClass.currentProject.CurrentSkin.BindSlotAttachment(
                    slot,
                    new ImageAttachment(imageRes)
                );
                e.Handled = true;
            }
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
        ConstantsClass.currentProject?.AddAnimation();
    }

    private void Add_New_Skin(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject?.AddSkin();
    }

    private void Delete_Skin(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject?.DeleteSkin();
    }

    private void Delete_Animation(object sender, RoutedEventArgs e)
    {
        ConstantsClass.currentProject?.DeleteAnimation();
    }

    private void Add_Slot(object sender, RoutedEventArgs e)
    {
        Bone bone = selectedBone;
        if (bone != null)
        {
            Slot s = new Slot("tesr", bone);
            ConstantsClass.currentProject.Slots.Add(s);
            ConstantsClass.currentProject.CurrentSkin.AddSlot(s);
            bone.UpdateSlots();
        }
    }

    private void OnSlotSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            Slot selectedSlot = listBox.SelectedItem as Slot;
            if (selectedSlot != null)
            {
                SelectedBone = (Bone)selectedSlot;
            }
        }
    }

    private void Delete_Slot(object sender, RoutedEventArgs e)
    {
        Slot selectedSlot = SlotsList.SelectedItem as Slot;
        if (selectedSlot != null)
        {
            ConstantsClass.currentProject.Slots.Remove(selectedSlot);
            ConstantsClass.currentProject.CurrentSkin.DeleteSlot(selectedSlot);
            selectedBone.UpdateSlots();
        }
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
            if (bone == SelectedBone)
            {
                SelectedBone = null;
                Console.WriteLine("Not bone");
            }
            else
            {
                SelectedBone = bone;

                Console.WriteLine("Bone");
            }
            e.Handled = true;
        }
    }

    private async void OpenSettings(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            var window = new Window
            {
                Title = "Settings",
                Width = 600,
                Height = 400,
                Content = new SettingsView(viewModel),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            await window.ShowDialog(this);
        }
    }

    private void SaveProject(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ProjectSettings.ProjectSettings.WriteAnim();
        Popups.ShowPopup("Saved", this);
    }

    private async void OpenProject(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var path = await ProjectManager.ProjectManager.OpenProject(this);
        ProjectSettings.ProjectSettings.ReadSettings(path);
    }

    private void NewProject(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ProjectManager.ProjectManager.NewProject();
    }
}
