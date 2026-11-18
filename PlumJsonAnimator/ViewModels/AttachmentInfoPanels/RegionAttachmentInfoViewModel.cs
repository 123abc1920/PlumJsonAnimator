using System;
using System.Windows.Input;
using Avalonia.Input;
using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.ViewModels;

public partial class RegionAttachmentInfoViewModel : ReactiveObject
{
    public ImageRes? Image
    {
        get => RegionAttachment.ImageRes;
        set
        {
            if (RegionAttachment.ImageRes != value)
            {
                RegionAttachment.ImageRes = value;
                this.RaisePropertyChanged(nameof(Image));
            }
        }
    }

    public ICommand DropImageCommand { get; }

    public RegionAttachment RegionAttachment { get; set; }

    public RegionAttachmentInfoViewModel(RegionAttachment regionAttachment)
    {
        RegionAttachment = regionAttachment;

        DropImageCommand = new Command(parameter =>
        {
            if (parameter is DragEventArgs e && e.Data.Get("Resource") is ImageRes droppedImage)
            {
                Image = droppedImage;
            }
        });
    }
}
