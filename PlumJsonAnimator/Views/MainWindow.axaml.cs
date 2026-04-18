using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;
using SukiUI.Controls;
using SukiUI.Toasts;

namespace PlumJsonAnimator.Views;

public partial class MainWindow : SukiWindow
{
    public static ISukiToastManager ToastManager = new SukiToastManager();

    private Dictionary<char, char> pairedSymbols = new Dictionary<char, char>()
    {
        { '{', '}' },
        { '[', ']' },
        { '"', '"' },
        { '<', '>' },
    };
    private bool _isDragging = false;

    private int currentTab = 0;

    public MainWindow()
    {
        InitializeComponent();
        this.KeyDown += OnWindowKeyDown;

        ToastHost.Manager = ToastManager;
        Popups.ToastManager = ToastManager;
    }

    public void initViews()
    {
        DispatcherTimer _canvasLoop = new DispatcherTimer();
        _canvasLoop.Interval = TimeSpan.FromMilliseconds(1000 / 60.0);
        _canvasLoop.Tick += UpdateCanvas;
        _canvasLoop.Start();

        DragDrop.SetAllowDrop(boneTreeView, true);
        boneTreeView.AddHandler(DragDrop.DropEvent, OnTreeViewDrop);

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.Canvas = mainCanvas;
            viewModel.Timeline = Timeline;
            viewModel.SetMainWin(this);
            if (!viewModel.JsonErrorObj.isOk)
            {
                Popups.ShowPopup(
                    viewModel.GetMessage(LocalizationConsts.REGENERATE_ERROR),
                    viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                );
            }
        }
    }

    private void UpdateCanvas(object? sender, EventArgs e)
    {
        if (currentTab == 0)
        {
            mainCanvas.Children.Clear();
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.CurrentProject?.drawSlots(mainCanvas);
                if (viewModel.DrawBones)
                {
                    viewModel.CurrentProject?.MainSkeleton?.drawSkeleton(mainCanvas);
                    if (viewModel.CurrentBone?.IsBone == true)
                    {
                        viewModel.CurrentBone?.DrawBone(mainCanvas);
                    }
                    viewModel.GenerateCode();
                }
                if (viewModel.CaptureMode)
                {
                    viewModel.GetCaptureArea()?.Draw(mainCanvas);
                }
            }
        }
        else if (currentTab == 1)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                if (viewModel.CanGenerateProject() == true)
                {
                    currentTab = 1;
                }
            }
        }
    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.SaveProject.Execute(null);
            }
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

    private async void AddRes(object sender, RoutedEventArgs e)
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

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.AddRes(paths);
        }
    }

    private void Press_Canvas(object sender, PointerPressedEventArgs e)
    {
        var canvas = (Canvas)sender;
        var point = e.GetPosition(canvas);

        var properties = e.GetCurrentPoint(canvas).Properties;
        if (properties.IsLeftButtonPressed)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                if (viewModel.CaptureMode == false)
                {
                    _isDragging = true;
                }
                else
                {
                    viewModel.GetCaptureArea()?.SelectPoint((int)(point.X), (int)(point.Y));
                }
            }
        }
    }

    private void Move_Canvas(object sender, PointerEventArgs e)
    {
        var canvas = (Canvas)sender;
        var point = e.GetPosition(canvas);

        if (DataContext is MainWindowViewModel viewModel)
        {
            if (viewModel.CaptureMode == true)
            {
                viewModel.GetCaptureArea()?.MovePoint((int)(point.X), (int)(point.Y));
                return;
            }

            if (_isDragging)
            {
                if (viewModel.CurrentBone != null)
                {
                    viewModel.CurrentProject?.currentMode.Transform(
                        viewModel.CurrentBone,
                        point.X - canvas.Width / 2,
                        point.Y - canvas.Height / 2
                    );
                }
            }
        }
    }

    private void Release_Canvas(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;

        if (DataContext is MainWindowViewModel viewModely)
        {
            if (viewModely.CaptureMode)
            {
                viewModely.GetCaptureArea()?.UnSelectPoint();
            }
            return;
        }

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.CurrentProject?.currentMode.ClearMode();
        }
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
                if (element is TreeViewItem item && item.DataContext is Bone bone)
                {
                    if (bone.IsBone == true)
                    {
                        if (DataContext is MainWindowViewModel viewModel)
                        {
                            viewModel.DropSlotToBone(bone.id, res);
                            return;
                        }
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
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.CurrentProject.CurrentSkin.BindSlotAttachment(
                        slot,
                        new ImageAttachment(imageRes)
                    );
                }
                e.Handled = true;
            }
        }
    }

    private void OnSlotSelectionChanged(object sender, TappedEventArgs e)
    {
        Slot selectedSlot = SlotsList.SelectedItem as Slot;
        if (selectedSlot != null)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.CurrentBone = (Bone)selectedSlot;
            }
        }
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl tabControl)
        {
            int newIndex = tabControl.SelectedIndex;

            if (DataContext is MainWindowViewModel viewModel)
            {
                if (newIndex == 0 && viewModel.JsonErrorObj.isOk != true)
                {
                    tabControl.SelectionChanged -= TabControl_SelectionChanged;
                    tabControl.SelectedIndex = 1;
                    tabControl.SelectionChanged += TabControl_SelectionChanged;
                    return;
                }

                if (tabControl.SelectedItem is TabItem selectedTab)
                {
                    currentTab = tabControl.SelectedIndex;
                }
            }
        }
    }

    private void OnBonePointerPressed(object sender, TappedEventArgs e)
    {
        if (sender is StackPanel panel && panel.DataContext is Bone bone)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                if (bone == viewModel.CurrentBone)
                {
                    viewModel.CurrentBone = null;
                    Console.WriteLine("Not bone");
                }
                else
                {
                    viewModel.CurrentBone = bone;

                    Console.WriteLine("Bone");
                }
            }
            e.Handled = true;
        }
    }

    private async void OpenSettings(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.ShowDialog(
                viewModel.GetMessage(LocalizationConsts.SETTINGS),
                this,
                DialogType.SETTINGS
            );
        }
    }

    private async void OpenProject(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.OpenProject(this);
        }
    }

    private async void NewProject(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.ShowDialog(
                viewModel.GetMessage(LocalizationConsts.NEW_PROJECT),
                this,
                DialogType.NEWPROJECT
            );
        }
    }

    private void IncreaseTimeLine(object sender, RoutedEventArgs e)
    {
        Timeline.Zoom++;
    }

    private void DecreaseTimeLine(object sender, RoutedEventArgs e)
    {
        Timeline.Zoom--;
    }

    private void InsertPairedSymbol(char first, char second)
    {
        var caretIndex = spineJsonText.CaretIndex;
        var text = spineJsonText.Text;
        spineJsonText.Text = text.Insert(caretIndex, first.ToString() + second);
        spineJsonText.CaretIndex = caretIndex + 1;
    }

    private void SpineJsonText_KeyDown(object sender, KeyEventArgs e)
    {
        if (
            (
                e.Key == Key.F
                && e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                && e.KeyModifiers.HasFlag(KeyModifiers.Alt)
            )
            || (
                e.Key == Key.L
                && e.KeyModifiers.HasFlag(KeyModifiers.Control)
                && e.KeyModifiers.HasFlag(KeyModifiers.Alt)
            )
        )
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                spineJsonText.Text = viewModel.Prettify(spineJsonText.Text);
            }

            e.Handled = true;
        }

        char? pressedChar = null;

        if (e.Key == Key.OemOpenBrackets)
        {
            pressedChar = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? '{' : '[';
        }
        else if (e.Key == Key.OemQuotes)
        {
            pressedChar = '"';
        }
        else if (e.Key == Key.OemComma && e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            pressedChar = '<';
        }

        if (pressedChar.HasValue && pairedSymbols.ContainsKey(pressedChar.Value))
        {
            e.Handled = true;
            var closingChar = pairedSymbols[pressedChar.Value];
            InsertPairedSymbol(pressedChar.Value, closingChar);
        }
    }

    private async void ExportAnim(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var storageProvider = topLevel.StorageProvider;
        var folder = await storageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions { Title = "Выберите папку", AllowMultiple = false }
        );

        if (folder.Count > 0)
        {
            ExportResult result = ExportResult.NO_FOLDER;
            if (DataContext is MainWindowViewModel viewModel)
            {
                result = viewModel.exportSpineJson(folder[0].Path.LocalPath);

                if (result == ExportResult.SUCCESS)
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.ANIM_SUCCESS),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                }
                else if (result == ExportResult.NO_FOLDER)
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.FOLDER_ERROR),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                }
            }
        }
    }

    private async void ImportAnim(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions { Title = "Open JSON File", AllowMultiple = false }
        );

        string[] paths = [];
        if (files?.Count > 0)
        {
            paths = files.Select(f => f.Path.LocalPath).ToArray();
        }

        string path = null;
        if (paths.Length > 0)
        {
            path = paths[0];
        }

        if (path != null && path != "")
        {
            ExportResult result = ExportResult.NO_FOLDER;
            if (DataContext is MainWindowViewModel viewModel)
            {
                result = viewModel.importSpineJson(path);

                if (result == ExportResult.SUCCESS)
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.IMPORT_SUCCESS),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                }
                else if (result == ExportResult.INCORRECT_JSON)
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.FILE_DAMAGED),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                }
                else if (result == ExportResult.NO_FOLDER)
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.FILE_NOT_EXIST),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                }
            }
        }
    }

    private async void ExportAsPng(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.ShowDialog(
                viewModel.GetMessage(LocalizationConsts.EXPORT_AS_PNG),
                this,
                DialogType.EXPORT_PNG
            );
        }
    }

    private async void ExportAsJpg(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.ShowDialog(
                viewModel.GetMessage(LocalizationConsts.EXPORT_AS_JPG),
                this,
                DialogType.EXPORT_JPG
            );
        }
    }

    private async void ExportAsGif(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.ShowDialog(
                viewModel.GetMessage(LocalizationConsts.EXPORT_AS_GIF),
                this,
                DialogType.EXPORT_GIF
            );
        }
    }

    private async void ExportAsMp4(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.ShowDialog(
                viewModel.GetMessage(LocalizationConsts.EXPORT_AS_MP4),
                this,
                DialogType.EXPORT_MP4
            );
        }
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        var isChecked = checkBox.IsChecked;

        if (isChecked != null)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.DrawBones = (bool)isChecked;
            }
        }
    }

    private void EditCaptureMode(object sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        var isChecked = checkBox?.IsChecked;
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.CaptureMode = (isChecked != null) ? (bool)isChecked : false;
        }
    }
}
