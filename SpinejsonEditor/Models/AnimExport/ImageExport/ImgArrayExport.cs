using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AnimExport.JsonExport;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Constants;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;

namespace AnimExport
{
    namespace ImageExport
    {
        public class ImageExporter
        {
            public static string ExportPath = "";
            public static Canvas canvas;

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
                string outputFolder
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
                string outputFolder
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

            public static async Task<ExportResult> ExportAsGif(
                double start,
                double end,
                string outputFile
            )
            {
                if (!File.Exists(outputFile))
                {
                    File.Create(outputFile).Close();
                }

                if (File.Exists(outputFile))
                {
                    ConstantsClass.currentProject.CurrentAnimation.currentTime = start;
                    double endTime = Math.Min(
                        ConstantsClass.currentProject.CurrentAnimation.MaxTime(),
                        end
                    );

                    List<Image<Rgba32>> frames = new List<Image<Rgba32>>();
                    while (ConstantsClass.currentProject.CurrentAnimation.currentTime <= endTime)
                    {
                        using (RenderTargetBitmap bitmap = CatchCanvas(canvas))
                        {
                            var pixelSize = bitmap.PixelSize;
                            var image = new Image<Rgba32>(pixelSize.Width, pixelSize.Height);

                            using (var stream = new MemoryStream())
                            {
                                bitmap.Save(stream);
                                stream.Position = 0;
                                var frameImage = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);
                                frames.Add(frameImage);
                            }
                        }

                        ConstantsClass.currentProject.CurrentAnimation.step();
                        canvas.InvalidateVisual();
                        await Task.Delay(30);
                    }

                    using (var gif = frames[0].CloneAs<Rgba32>())
                    {
                        gif.Metadata.GetGifMetadata().RepeatCount = 0;
                        gif.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = 10;

                        for (int i = 1; i < frames.Count; i++)
                        {
                            gif.Frames.AddFrame(frames[i].Frames.RootFrame);
                            gif.Frames[i].Metadata.GetGifMetadata().FrameDelay = 10;
                        }

                        gif.Save(outputFile, new GifEncoder());
                    }

                    foreach (var frame in frames)
                    {
                        frame.Dispose();
                    }

                    return ExportResult.SUCCESS;
                }

                return ExportResult.NO_FOLDER;
            }

            public static void ExportAsMp4() { }
        }
    }
}
