using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Views
{
    public partial class ExportPanelPNG : UserControl
    {
        public ExportPanelPNG()
        {
            InitializeComponent();
        }

        public ExportPanelPNG(ViewModelBase viewModel)
            : this()
        {
            DataContext = viewModel;

            var vm = (ExportPanelPNGViewModel)viewModel;

            this.FindControl<TextBox>("path").Text = vm.ExportPath;
            this.FindControl<TextBox>("start").Text = "0";
            this.FindControl<TextBox>("end").Text = viewModel
                .CurrentProject.CurrentAnimation!.MaxTime()
                .ToString();
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
                if (DataContext is ExportPanelPNGViewModel viewModel)
                {
                    viewModel.ExportPath = folder[0].Path.LocalPath;
                    this.FindControl<TextBox>("path").Text = viewModel.ExportPath;
                }
            }
        }

        private async void ExportAsPng(object sender, RoutedEventArgs e)
        {
            if (DataContext is ExportPanelPNGViewModel viewModel)
            {
                var startTextBox = this.FindControl<TextBox>("start");
                var endTextBox = this.FindControl<TextBox>("end");

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
                    double.TryParse(startTextBox.Text, out double startValue)
                    && double.TryParse(endTextBox.Text, out double endValue)
                )
                {
                    ExportResult result = ExportResult.INCORRECT_JSON;

                    result = await viewModel.ExportAsPng(
                        startValue,
                        endValue,
                        this.FindControl<TextBox>("path").Text
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
