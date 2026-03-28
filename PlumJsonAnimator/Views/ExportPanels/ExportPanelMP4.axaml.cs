using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class ExportPanelMP4 : UserControl
    {
        public ExportPanelMP4()
        {
            InitializeComponent();
        }

        public ExportPanelMP4(ViewModelBase viewModel)
            : this()
        {
            DataContext = viewModel;

            var vm = (ExportPanelMP4ViewModel)viewModel;

            this.FindControl<TextBox>("path").Text = vm.ExportPath;
            this.FindControl<TextBox>("pName").Text = viewModel.CurrentProject!.Name;
            this.FindControl<TextBox>("start").Text = "0";
            this.FindControl<TextBox>("end").Text = viewModel
                .CurrentProject.CurrentAnimation!.MaxTime()
                .ToString();
        }

        private async void SelectFfmpeg(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var storageProvider = topLevel.StorageProvider;
            var fileTypeFilter = new FilePickerFileType[]
            {
                new("*.exe") { Patterns = new[] { "*.exe" } },
            };

            var result = await storageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Выберите файл ffmpeg.exe",
                    AllowMultiple = false,
                    FileTypeFilter = fileTypeFilter,
                }
            );

            var filePath = result?.FirstOrDefault()?.Path.LocalPath;

            if (filePath != null && filePath != "")
            {
                if (DataContext is ExportPanelMP4ViewModel viewModel)
                {
                    viewModel.FfmpegPath = filePath;
                }
            }
        }

        private async void SelectFolder(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var storageProvider = topLevel.StorageProvider;
            var folder = await storageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions { Title = "Выберите папку", AllowMultiple = false }
            );

            if (folder.Count > 0)
            {
                if (DataContext is ExportPanelMP4ViewModel viewModel)
                {
                    viewModel.ExportPath = folder[0].Path.LocalPath;
                    this.FindControl<TextBox>("path").Text = viewModel.ExportPath;
                }
            }
        }

        private async void ExportAsMp4(object sender, RoutedEventArgs e)
        {
            if (DataContext is ExportPanelMP4ViewModel viewModel)
            {
                var startTextBox = this.FindControl<TextBox>("start");
                var endTextBox = this.FindControl<TextBox>("end");

                if (
                    this.FindControl<TextBox>("ffmpegPath").Text == ""
                    || this.FindControl<TextBox>("ffmpegPath").Text == null
                )
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.INPUT_FFMPEG),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                    return;
                }

                if (
                    this.FindControl<TextBox>("path").Text == ""
                    || this.FindControl<TextBox>("path").Text == null
                )
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.INPUT_FOLDER),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                    return;
                }

                if (
                    this.FindControl<TextBox>("pName").Text == ""
                    || this.FindControl<TextBox>("pName").Text == null
                )
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.INPUT_NAME),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                    return;
                }

                if (
                    double.TryParse(startTextBox.Text, out double startValue)
                    && double.TryParse(endTextBox.Text, out double endValue)
                )
                {
                    ExportResult result = ExportResult.INCORRECT_JSON;

                    result = await viewModel.ExportAsMp4(
                        startValue,
                        endValue,
                        System.IO.Path.Combine(
                            this.FindControl<TextBox>("path").Text,
                            $"{this.FindControl<TextBox>("pName").Text}.mp4"
                        ),
                        this.FindControl<TextBox>("ffmpegPath").Text
                    );

                    if (result == ExportResult.SUCCESS)
                    {
                        Popups.ShowPopup(
                            viewModel.GetMessage(LocalizationConsts.EXPORT_SUCCESS),
                            viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                        );
                    }
                    else if (result == ExportResult.NO_FOLDER)
                    {
                        Popups.ShowPopup(
                            viewModel.GetMessage(LocalizationConsts.FOLDER_NOT_EXIST),
                            viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                        );
                    }
                    else if (result == ExportResult.NO_FFMPEG)
                    {
                        Popups.ShowPopup(
                            viewModel.GetMessage(LocalizationConsts.FFMPEG_NOT_EXIST),
                            viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                        );
                    }
                }
                else
                {
                    Popups.ShowPopup(
                        viewModel.GetMessage(LocalizationConsts.INCORRECT_TIME),
                        viewModel.GetMessage(LocalizationConsts.INFO_MESSAGE)
                    );
                }
            }
        }
    }
}
