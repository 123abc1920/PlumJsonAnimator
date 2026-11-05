using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class RegionAttachmentInfo : UserControl
{
    public RegionAttachmentInfo(ImageRes? imageRes)
    {
        InitializeComponent();
        DataContext = new RegionAttachmentInfoViewModel(imageRes);
    }
}
