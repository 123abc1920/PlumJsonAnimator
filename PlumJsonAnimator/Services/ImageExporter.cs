using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;

namespace PlumJsonAnimator.Services
{
    // TODO: gif
    // TODO: image size
    // TODO: ffmpeg ui binding
    public class ImageExporter
    {
        public string ExportPath = "";
        public Canvas? canvas = null;

        private GlobalState globalState;

        public ImageExporter(GlobalState globalState)
        {
            this.globalState = globalState;
        }

        private RenderTargetBitmap CatchCanvas(Canvas canvas)
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

        public async Task<ExportResult> ExportAsPng(double start, double end, string outputFolder)
        {
            if (Directory.Exists(outputFolder))
            {
                this.globalState.currentProject!.CurrentAnimation.currentTime = start;
                double endTime = Math.Min(
                    this.globalState.currentProject.CurrentAnimation.MaxTime(),
                    end
                );

                var i = 0;
                var drawBones = this.globalState.drawBones;
                this.globalState.drawBones = false;
                while (this.globalState.currentProject.CurrentAnimation.currentTime <= endTime)
                {
                    using (RenderTargetBitmap bitmap = CatchCanvas(canvas))
                    {
                        var fileName = Path.Combine(outputFolder, $"{i:D8}.png");
                        using (var fileStream = File.Create(fileName))
                        {
                            bitmap.Save(fileStream);
                        }
                    }

                    this.globalState.currentProject.CurrentAnimation.step();
                    canvas.InvalidateVisual();
                    await Task.Delay(30);
                    i++;
                }
                this.globalState.drawBones = drawBones;
                return ExportResult.SUCCESS;
            }

            return ExportResult.NO_FOLDER;
        }

        public async Task<ExportResult> ExportAsJpg(double start, double end, string outputFolder)
        {
            if (Directory.Exists(outputFolder))
            {
                this.globalState.currentProject!.CurrentAnimation.currentTime = start;
                double endTime = Math.Min(
                    this.globalState.currentProject.CurrentAnimation.MaxTime(),
                    end
                );

                var i = 0;
                var drawBones = this.globalState.drawBones;
                this.globalState.drawBones = false;
                while (this.globalState.currentProject.CurrentAnimation.currentTime <= endTime)
                {
                    using (RenderTargetBitmap bitmap = CatchCanvas(canvas))
                    {
                        var fileName = Path.Combine(outputFolder, $"{i:D8}.jpg");
                        using (var fileStream = File.Create(fileName))
                        {
                            bitmap.Save(fileStream);
                        }
                    }

                    this.globalState.currentProject.CurrentAnimation.step();
                    canvas.InvalidateVisual();
                    await Task.Delay(30);
                    i++;
                }
                this.globalState.drawBones = drawBones;
                return ExportResult.SUCCESS;
            }

            return ExportResult.NO_FOLDER;
        }

        public async Task<ExportResult> ExportAsGif(double start, double end, string outputFile)
        {
            if (!File.Exists(outputFile))
            {
                File.Create(outputFile).Close();
            }

            if (File.Exists(outputFile))
            {
                this.globalState.currentProject!.CurrentAnimation.currentTime = start;
                double endTime = Math.Min(
                    this.globalState.currentProject.CurrentAnimation.MaxTime(),
                    end
                );

                var drawBones = this.globalState.drawBones;
                this.globalState.drawBones = false;
                List<Image<Rgba32>> frames = new List<Image<Rgba32>>();
                while (this.globalState.currentProject.CurrentAnimation.currentTime <= endTime)
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

                    this.globalState.currentProject.CurrentAnimation.step();
                    canvas.InvalidateVisual();
                    await Task.Delay(30);
                }

                using (var gif = frames[0].CloneAs<Rgba32>())
                {
                    gif.Metadata.GetGifMetadata().RepeatCount = 0;

                    for (int i = 1; i < frames.Count; i++)
                    {
                        gif.Frames.AddFrame(frames[i].Frames.RootFrame);
                    }

                    gif.Save(outputFile, new GifEncoder());
                }

                foreach (var frame in frames)
                {
                    frame.Dispose();
                }

                this.globalState.drawBones = drawBones;
                return ExportResult.SUCCESS;
            }

            return ExportResult.NO_FOLDER;
        }

        public async Task<ExportResult> ExportAsMp4(
            double start,
            double end,
            string outputFile,
            string ffmpegPath
        )
        {
            if (!File.Exists(ffmpegPath))
                return ExportResult.NO_FFMPEG;

            using (var testBitmap = CatchCanvas(canvas))
            {
                if (testBitmap == null)
                {
                    return ExportResult.FFMPEG_ERROR;
                }

                var size = testBitmap.PixelSize;

                int width = size.Width;
                int height = size.Height;

                if (width <= 0 || height <= 0)
                {
                    return ExportResult.FFMPEG_ERROR;
                }

                string arguments =
                    $"-f rawvideo -pix_fmt bgra "
                    + $"-s {width}x{height} "
                    + $"-r {this.globalState.FPS} "
                    + $"-i - "
                    + $"-c:v libx264 "
                    + $"-preset fast "
                    + $"-crf 23 "
                    + $"-pix_fmt yuv420p "
                    + $"-movflags +faststart "
                    + $"-y "
                    + $"\"{outputFile}\"";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    },
                };

                StringBuilder errorOutput = new StringBuilder();
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorOutput.AppendLine(e.Data);
                        Console.WriteLine($"FFmpeg: {e.Data}");
                    }
                };

                process.Start();
                process.BeginErrorReadLine();

                var drawBones = this.globalState.drawBones;
                this.globalState.drawBones = false;
                using (var stdin = process.StandardInput.BaseStream)
                {
                    this.globalState.currentProject!.CurrentAnimation.currentTime = start;
                    double endTime = Math.Min(
                        this.globalState.currentProject.CurrentAnimation.MaxTime(),
                        end
                    );

                    int frameCount = 0;
                    bool error = false;

                    while (this.globalState.currentProject.CurrentAnimation.currentTime <= endTime)
                    {
                        using (RenderTargetBitmap bitmap = CatchCanvas(canvas))
                        {
                            var currentSize = bitmap.PixelSize;

                            if (currentSize.Width != width || currentSize.Height != height)
                            {
                                Console.WriteLine(
                                    $"Размер кадра не совпадает: {currentSize.Width}x{currentSize.Height}, ожидалось: {width}x{height}"
                                );
                                error = true;
                                break;
                            }

                            int stride = width * 4;
                            int bufferSize = height * stride;

                            var buffer = new byte[bufferSize];

                            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                            try
                            {
                                bitmap.CopyPixels(
                                    new PixelRect(0, 0, width, height),
                                    handle.AddrOfPinnedObject(),
                                    bufferSize,
                                    stride
                                );
                            }
                            finally
                            {
                                handle.Free();
                            }

                            try
                            {
                                await stdin.WriteAsync(buffer);
                                frameCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка записи: {ex.Message}");
                                error = true;
                                break;
                            }
                        }

                        this.globalState.currentProject.CurrentAnimation.step();
                        canvas.InvalidateVisual();
                        await Task.Delay(30);
                    }

                    if (!error)
                    {
                        stdin.Flush();
                    }
                }
                this.globalState.drawBones = drawBones;

                if (!process.WaitForExit(10000))
                {
                    process.Kill();
                    Console.WriteLine("FFmpeg timeout");
                    return ExportResult.FFMPEG_ERROR;
                }

                Console.WriteLine($"FFmpeg exit code: {process.ExitCode}");

                if (
                    process.ExitCode == 0
                    && File.Exists(outputFile)
                    && new FileInfo(outputFile).Length > 0
                )
                {
                    return ExportResult.SUCCESS;
                }
                else
                {
                    return ExportResult.FFMPEG_ERROR;
                }
            }
        }
    }
}
