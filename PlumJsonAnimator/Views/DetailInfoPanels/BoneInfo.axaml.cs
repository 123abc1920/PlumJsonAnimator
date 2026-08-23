using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class BoneInfo : UserControl
    {
        public BoneInfo()
        {
            InitializeComponent();
        }

        public BoneInfo(ViewModelBase viewModel)
            : this()
        {
            DataContext = viewModel;
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
                if (
                    e.Data.Get("Resource") is ImageRes imageRes
                    && listBoxItem.DataContext is Slot slot
                )
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
    }
}
