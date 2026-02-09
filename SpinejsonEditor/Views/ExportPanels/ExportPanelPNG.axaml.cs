using AnimExport.ImageExport;
using AnimExport.JsonExport;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Constants;
using SpinejsonEditor.ViewModels;

namespace SpinejsonEditor.Views
{
    public partial class ExportPanelPNG : UserControl
    {
        public ExportPanelPNG()
        {
            InitializeComponent();

            this.FindControl<TextBox>("path").Text = ImageExporter.ExportPath;
            this.FindControl<TextBox>("start").Text = "0";
            this.FindControl<TextBox>("end").Text = ConstantsClass
                .currentProject.CurrentAnimation.MaxTime()
                .ToString();
        }

        public ExportPanelPNG(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
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
                if (DataContext is MainWindowViewModel viewModel)
                {
                    ImageExporter.ExportPath = folder[0].Path.LocalPath;
                    this.FindControl<TextBox>("path").Text = ImageExporter.ExportPath;
                }
            }
        }

        private async void ExportAsPng(object sender, RoutedEventArgs e)
        {
            var startTextBox = this.FindControl<TextBox>("start");
            var endTextBox = this.FindControl<TextBox>("end");

            if (
                this.FindControl<TextBox>("path").Text == ""
                || this.FindControl<TextBox>("path").Text == null
            )
            {
                Popups.ShowPopup("Введите папку");
                return;
            }

            if (
                double.TryParse(startTextBox.Text, out double startValue)
                && double.TryParse(endTextBox.Text, out double endValue)
            )
            {
                ExportResult result = await AnimExport.ImageExport.ImageExporter.ExportAsPng(
                    startValue,
                    endValue,
                    this.FindControl<TextBox>("path").Text
                );

                if (result == ExportResult.SUCCESS)
                {
                    Popups.ShowPopup("Успешно экспортировано", this);
                }
                else if (result == ExportResult.NO_FOLDER)
                {
                    Popups.ShowPopup("Папка не найдена", this);
                }
            }
            else
            {
                Popups.ShowPopup("Неверные значения времени", this);
            }
        }
    }
}
