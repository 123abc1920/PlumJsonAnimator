using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class BoneInfo : UserControl, IInfoPanel
{
    public static PanelTypes InfoPanelType => PanelTypes.BONEINFO;

    public BoneInfo(GlobalState globalState, HistoryManager historyManager)
    {
        InitializeComponent();
        DataContext = new BoneInfoViewModel(globalState, historyManager);
    }

    private void OnSlotSelectionChanged(object sender, TappedEventArgs e)
    {
        Slot selectedSlot = SlotsList.SelectedItem as Slot;
        if (selectedSlot != null)
        {
            if (DataContext is BoneInfoViewModel viewModel)
            {
                viewModel.GlobalState.SelectedItem = selectedSlot;
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
        HistoryManager historyManager,
        AttachmentTypeConverter? attachmentTypeConverter = null
    )
    {
        return new BoneInfo(globalState, historyManager);
    }
}
