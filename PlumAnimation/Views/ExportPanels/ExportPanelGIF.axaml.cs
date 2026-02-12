using System.IO;
using AnimExport.ImageExport;
using AnimExport.JsonExport;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Constants;
using PlumAnimation.ViewModels;

namespace PlumAnimation.Views
{
    public partial class ExportPanelGIF : UserControl
    {
        public ExportPanelGIF()
        {
            InitializeComponent();

            this.FindControl<TextBox>("path").Text = ImageExporter.ExportPath;
            this.FindControl<TextBox>("pName").Text = ConstantsClass.currentProject.Name;
            this.FindControl<TextBox>("start").Text = "0";
            this.FindControl<TextBox>("end").Text = ConstantsClass
                .currentProject.CurrentAnimation.MaxTime()
                .ToString();
        }

        public ExportPanelGIF(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private async void SelectFolder(object sender, RoutedEventArgs e)
        {
            /*var topLevel = TopLevel.GetTopLevel(this);
            var storageProvider = topLevel.StorageProvider;
            var fileTypeFilter = new FilePickerFileType[]
            {
                new("*.gif") { Patterns = new[] { "*.gif" } },
            };

            var result = await storageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Выберите файл gif",
                    AllowMultiple = false,
                    FileTypeFilter = fileTypeFilter,
                }
            );

            var filePath = result?.FirstOrDefault()?.Path.LocalPath;

            if (filePath != null && filePath != "")
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    ImageExporter.ExportPath = filePath;
                    this.FindControl<TextBox>("path").Text = ImageExporter.ExportPath;
                }
            }*/
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

        private async void ExportAsGif(object sender, RoutedEventArgs e)
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
                this.FindControl<TextBox>("pName").Text == ""
                || this.FindControl<TextBox>("pName").Text == null
            )
            {
                Popups.ShowPopup("Введите имя GIF");
                return;
            }

            if (
                double.TryParse(startTextBox.Text, out double startValue)
                && double.TryParse(endTextBox.Text, out double endValue)
            )
            {
                ExportResult result = await ImageExporter.ExportAsGif(
                    startValue,
                    endValue,
                    Path.Combine(
                        this.FindControl<TextBox>("path").Text,
                        $"{this.FindControl<TextBox>("pName").Text}.gif"
                    )
                );

                if (result == ExportResult.SUCCESS)
                {
                    Popups.ShowPopup("Успешно экспортировано", this);
                }
                else if (result == ExportResult.NO_FOLDER)
                {
                    Popups.ShowPopup("Файл не найдена", this);
                }
            }
            else
            {
                Popups.ShowPopup("Неверные значения времени", this);
            }
        }
    }
}
