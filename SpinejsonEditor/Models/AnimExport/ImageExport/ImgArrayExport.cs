using System;
using System.IO;
using System.Threading.Tasks;
using AnimExport.JsonExport;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Constants;
using HarfBuzzSharp;

namespace AnimExport
{
    namespace ImageExport
    {
        public class ExportParams
        {
            public static Canvas Canvas;
            public static string folder;
        }

        public class ImageExporter
        {
            private static RenderTargetBitmap CatchCanvas(Canvas canvas)
            {
                var bounds = new Rect(0, 0, canvas.Bounds.Width, canvas.Bounds.Height);

                var pixelSize = new PixelSize((int)bounds.Width, (int)bounds.Height);
                var dpi = new Vector(96, 96);

                var renderTarget = new RenderTargetBitmap(pixelSize, dpi);

                canvas.Measure(bounds.Size);
                canvas.Arrange(bounds);
                renderTarget.Render(canvas);

                return renderTarget;
            }

            public static async Task<ExportResult> ExportAsPng(
                double start,
                double end,
                string outputFolder,
                Canvas canvas
            )
            {
                if (Directory.Exists(outputFolder))
                {
                    ConstantsClass.currentProject.CurrentAnimation.currentTime = start;
                    double endTime = Math.Min(
                        ConstantsClass.currentProject.CurrentAnimation.MaxTime(),
                        end
                    );

                    var i = 0;
                    while (ConstantsClass.currentProject.CurrentAnimation.currentTime <= endTime)
                    {
                        using (RenderTargetBitmap bitmap = CatchCanvas(canvas))
                        {
                            var fileName = Path.Combine(outputFolder, $"{i:D8}.png");
                            using (var fileStream = File.Create(fileName))
                            {
                                bitmap.Save(fileStream);
                            }
                        }

                        ConstantsClass.currentProject.CurrentAnimation.step();
                        canvas.InvalidateVisual();
                        await Task.Delay(30);
                        i++;
                    }
                    return ExportResult.SUCCESS;
                }

                return ExportResult.NO_FOLDER;
            }

            public static async Task<ExportResult> ExportAsJpg(
                double start,
                double end,
                string outputFolder,
                Canvas canvas
            )
            {
                if (Directory.Exists(outputFolder))
                {
                    ConstantsClass.currentProject.CurrentAnimation.currentTime = start;
                    double endTime = Math.Min(
                        ConstantsClass.currentProject.CurrentAnimation.MaxTime(),
                        end
                    );

                    var i = 0;
                    while (ConstantsClass.currentProject.CurrentAnimation.currentTime <= endTime)
                    {
                        using (RenderTargetBitmap bitmap = CatchCanvas(canvas))
                        {
                            var fileName = Path.Combine(outputFolder, $"{i:D8}.jpg");
                            using (var fileStream = File.Create(fileName))
                            {
                                bitmap.Save(fileStream);
                            }
                        }

                        ConstantsClass.currentProject.CurrentAnimation.step();
                        canvas.InvalidateVisual();
                        await Task.Delay(30);
                        i++;
                    }
                    return ExportResult.SUCCESS;
                }

                return ExportResult.NO_FOLDER;
            }

            public static void ExportAsGif() { }

            public static void ExportAsMp4() { }
        }
    }
}
