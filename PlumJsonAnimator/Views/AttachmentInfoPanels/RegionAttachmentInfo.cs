using Avalonia.Controls;
using PlumJsonAnimator.Common;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views;

public partial class RegionAttachmentInfo : UserControl
{
    public RegionAttachmentInfo()
    {
        InitializeComponent();
        DataContext = new RegionAttachmentInfoViewModel();
    }
}
