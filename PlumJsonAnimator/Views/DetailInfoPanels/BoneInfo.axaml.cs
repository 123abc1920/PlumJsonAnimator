using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class BoneInfo : UserControl, IInfoPanel
{
    public static InfoPanelTypes InfoPanelType => InfoPanelTypes.BONEINFO;

    public BoneInfo(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
    {
        InitializeComponent();
        DataContext = new BoneInfoViewModel(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp
        );
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
                        viewModel._attachmentFactory.CreateAttachmentByType(
                            AttachmentTypes.REGION,
                            new AttachmentDTO()
                            {
                                Name = $"attachment{Counter.GenerateNamePostfix()}",
                                Res = imageRes,
                            }
                        )
                    );
                }
                e.Handled = true;
            }
        }
    }

    public static IInfoPanel Create(
        GlobalState globalState,
        Dialogs dialogs,
        ProjectFilesManager projectManager,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
    {
        return new BoneInfo(
            globalState,
            dialogs,
            projectManager,
            appSettings,
            localizationService,
            imageExporter,
            plumApp
        );
    }
}
