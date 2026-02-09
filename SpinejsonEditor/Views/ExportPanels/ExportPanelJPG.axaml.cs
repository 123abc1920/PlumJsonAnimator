using AnimExport.ImageExport;
using AnimExport.JsonExport;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Constants;
using SpinejsonEditor.ViewModels;

namespace SpinejsonEditor.Views
{
    public partial class ExportPanelJPG : UserControl
    {
        public ExportPanelJPG()
        {
            InitializeComponent();
        }

        public ExportPanelJPG(MainWindowViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }

        private async void ExportAsJpg(object sender, RoutedEventArgs e)
        {
            var startTextBox = this.FindControl<TextBox>("start");
            var endTextBox = this.FindControl<TextBox>("end");

            if (
                double.TryParse(startTextBox.Text, out double startValue)
                && double.TryParse(endTextBox.Text, out double endValue)
            )
            {
                ExportResult result = await AnimExport.ImageExport.ImageExporter.ExportAsJpg(
                    startValue,
                    endValue,
                    ExportParams.folder,
                    ExportParams.Canvas
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
