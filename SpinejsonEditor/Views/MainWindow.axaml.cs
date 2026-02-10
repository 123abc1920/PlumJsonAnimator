using System;
using System.IO;
using System.Linq;
using AnimEngine;
using AnimExport.ImageExport;
using AnimExport.JsonExport;
using AnimModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Constants;
using Resources;
using SpinejsonEditor.ViewModels;
using SpinejsonGeneration;
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

        Popups.win = this;
        ImageExporter.canvas = mainCanvas;

        AppSettings.ReadSettings();
        ProjectSettings.ProjectSettings.ReadSettings();
        ProjectManager.ProjectManager.LoadRes();
        ProjectValidResult validateResult =
            ConstantsClass.currentProject.SpinejsonCode.regenerate();
        if (!validateResult.IsOk)
        {
            Popups.ShowPopup("Возникли проблемы в json коде, невозможно восстановить проект", this);
            currentTab = 1;
        }

        EventHandler animationTick = (sender, e) =>
        {
            if (currentTab == 0)
            {
                Timeline.CurrentTime += 1.0 / ConstantsClass.FPS;
            }
        };
        ConstantsClass.MainEngine.AddCustomTickHandler(animationTick);
    }

    private void UpdateCanvas(object? sender, EventArgs e)
    {
        if (currentTab == 0)
        {
            mainCanvas.Children.Clear();
            ConstantsClass.currentProject?.drawSlots(mainCanvas);
            ConstantsClass.currentProject?.MainSkeleton?.drawSkeleton(mainCanvas);
        }
        else if (currentTab == 1)
        {
            ConstantsClass.jsonError.ErrorText = JsonValidator.JsonValidator.validate(
                spineJsonText.Text
            );
            if (ConstantsClass.jsonError.isOk)
            {
                ProjectValidResult validateResult =
                    ConstantsClass.currentProject.SpinejsonCode.regenerate();
                if (!validateResult.IsOk)
                {
                    ConstantsClass.jsonError.ErrorText = validateResult.Message;
                    currentTab = 1;
                }
            }
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

    private async void RenameRes(object sender, RoutedEventArgs e)
    {
        Res selectedRes = resList.SelectedItem as Res;
        if (selectedRes != null)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.RedactObj = selectedRes;
                Dialogs.ShowDialog("Rename", viewModel, this, ViewType.RENAME);
            }
        }
    }

    private async void RenameSlot(object sender, RoutedEventArgs e)
    {
        Slot selectedSlot = SlotsList.SelectedItem as Slot;
        if (selectedSlot != null)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.RedactObj = selectedSlot;
                Dialogs.ShowDialog("Rename", viewModel, this, ViewType.RENAME);
            }
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
        Bone bone = boneTreeView.SelectedItem as Bone;
        if (bone != null)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.RedactObj = bone;
                Dialogs.ShowDialog("Rename", viewModel, this, ViewType.RENAME);
            }
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
        ConstantsClass.MainEngine.runAnimation();
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

    private void OnSlotSelectionChanged(object sender, TappedEventArgs e)
    {
        Slot selectedSlot = SlotsList.SelectedItem as Slot;
        if (selectedSlot != null)
        {
            SelectedBone = (Bone)selectedSlot;
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

    private async void OpenSettings(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            Dialogs.ShowDialog("Settings", viewModel, this, ViewType.SETTINGS);
        }
    }

    private void SaveProject(object sender, RoutedEventArgs e)
    {
        ProjectSettings.ProjectSettings.WriteAnim();
        Popups.ShowPopup("Saved", this);
    }

    private async void OpenProject(object sender, RoutedEventArgs e)
    {
        var path = await ProjectManager.ProjectManager.OpenProject(this);
        ProjectSettings.ProjectSettings.ReadSettings(path);
    }

    private async void NewProject(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            Dialogs.ShowDialog("New Project", viewModel, this, ViewType.NEWPROJECT);
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

    private void PrevKeyframe(object sender, RoutedEventArgs e)
    {
        if (selectedBone != null)
        {
            Timeline.CurrentTime = ConstantsClass.currentProject.CurrentAnimation.FindKeyFrame(
                selectedBone,
                ConstantsClass.currentProject.CurrentAnimation.currentTime,
                ConstantsClass.currentProject.currentMode.type,
                false
            );
        }
    }

    private void NextKeyframe(object sender, RoutedEventArgs e)
    {
        if (selectedBone != null)
        {
            Timeline.CurrentTime = ConstantsClass.currentProject.CurrentAnimation.FindKeyFrame(
                selectedBone,
                ConstantsClass.currentProject.CurrentAnimation.currentTime,
                ConstantsClass.currentProject.currentMode.type,
                true
            );
        }
    }

    private void AddKeyFrame(object sender, RoutedEventArgs e)
    {
        if (selectedBone != null)
        {
            ConstantsClass.currentProject.CurrentAnimation.AddKeyFrame(
                selectedBone,
                ConstantsClass.currentProject.currentMode.type
            );
        }
    }

    private void DeleteKeyFrame(object sender, RoutedEventArgs e)
    {
        if (selectedBone != null)
        {
            ConstantsClass.currentProject.CurrentAnimation.DeleteKeyFrame(
                selectedBone,
                ConstantsClass.currentProject.currentMode.type
            );
        }
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
            spineJsonText.Text = Prettify.Prettify.prettify(spineJsonText.Text);
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

        if (pressedChar.HasValue && ConstantsClass.pairedSymbols.ContainsKey(pressedChar.Value))
        {
            e.Handled = true;
            var closingChar = ConstantsClass.pairedSymbols[pressedChar.Value];
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
            ExportResult result = SpineJsonExport.exportSpineJson(folder[0].Path.LocalPath);
            if (result == ExportResult.SUCCESS)
            {
                Popups.ShowPopup("Анимация успешно экспортирована!", this);
            }
            else if (result == ExportResult.NO_FOLDER)
            {
                Popups.ShowPopup("Невозможно экспортировать в эту папку", this);
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
            ExportResult result = SpineJsonExport.importSpineJson(path);
            if (result == ExportResult.SUCCESS)
            {
                Popups.ShowPopup("Успешно импортировано!");
            }
            else if (result == ExportResult.INCORRECT_JSON)
            {
                Popups.ShowPopup("Файл поврежден.");
            }
            else if (result == ExportResult.NO_FOLDER)
            {
                Popups.ShowPopup("Файл не существует.");
            }
        }
    }

    private async void ExportAsPng(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            Dialogs.ShowDialog("Экспорт как PNG", viewModel, this, ViewType.EXPORT_PNG);
        }
    }

    private async void ExportAsJpg(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            Dialogs.ShowDialog("Экспорт как JPG", viewModel, this, ViewType.EXPORT_JPG);
        }
    }

    private async void ExportAsGif(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            Dialogs.ShowDialog("Экспорт как GIF", viewModel, this, ViewType.EXPORT_GIF);
        }
    }

    private async void ExportAsMp4(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            Dialogs.ShowDialog("Экспорт как MP4", viewModel, this, ViewType.EXPORT_MP4);
        }
    }
}
