using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class RegionAttachmentInfo : UserControl
{
    public RegionAttachmentInfo(RegionAttachment regionAttachment)
    {
        InitializeComponent();
        DataContext = new RegionAttachmentInfoViewModel(regionAttachment);

        DragDrop.SetAllowDrop(DropBorder, true);
        DropBorder.AddHandler(
            DragDrop.DragOverEvent,
            (s, e) =>
            {
                if (e.Data.Get("Resource") is ImageRes)
                {
                    e.DragEffects = DragDropEffects.Copy;
                    e.Handled = true;
                }
            }
        );
        DropBorder.AddHandler(
            DragDrop.DropEvent,
            (s, e) =>
            {
                if (DataContext is RegionAttachmentInfoViewModel vm)
                {
                    vm.DropImageCommand.Execute(e);
                }
            }
        );
    }
}
