using System;
using System.IO;
using Avalonia;
using Avalonia.Styling;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;
using SukiUI;

// TODO: отделить настройки отдельного проекта от его json кода
namespace PlumJsonAnimator.Services
{
    public class AppSettings
    {
        private string AppSettingsPath;
        private string AppSettingsFile;
        public AppSettingsData? appSettings = null;

        private GlobalState globalState;

        public AppSettings(GlobalState globalState)
        {
            this.globalState = globalState;

            AppSettingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlumJsonAnimator"
            );
            AppSettingsFile = Path.Combine(
                AppSettingsPath,
                $"settings{this.globalState.programExt}"
            );

            this.appSettings = new AppSettingsData()
            {
                Lang = "ru",
                LastDir = "",
                Theme = "light",
                Workspace = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    this.globalState.workspace,
                    "NewProject"
                ),
                Ffmpeg = "",
                CaptureX = 0,
                CaptureY = 0,
                CaptureWidth = this.globalState.canvasWidth,
                CaptureHeight = this.globalState.canvasHeight,
            };
        }

        public void SetSettings(AppSettingsData appSettings)
        {
            this.appSettings = appSettings;
            SaveSettings();
        }

        public void SetCaptureArea(Rect rect)
        {
            this.appSettings!.CaptureX = (int)rect.X;
            this.appSettings.CaptureY = (int)rect.Y;
            this.appSettings.CaptureWidth = (int)rect.Width;
            this.appSettings.CaptureHeight = (int)rect.Height;

            SaveSettings();
        }

        public void ChangeProject(string newWorkspace)
        {
            this.appSettings!.Workspace = newWorkspace;
            SaveSettings();
        }

        public void SaveSettings()
        {
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }

            if (!File.Exists(AppSettingsFile))
            {
                File.Create(AppSettingsFile).Close();
            }

            File.WriteAllText(
                AppSettingsFile,
                JsonConvert.SerializeObject(this.appSettings, this.globalState.jsonSettings)
            );
        }

        public void ReadSettings()
        {
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }

            if (!File.Exists(AppSettingsFile))
            {
                File.Create(AppSettingsFile).Close();
                SaveSettings();
            }

            var newSettings = JsonConvert.DeserializeObject<AppSettingsData>(
                File.ReadAllText(AppSettingsFile)
            );

            if (newSettings != null)
            {
                if (
                    newSettings.Workspace != null
                    && newSettings.Workspace != ""
                    && Directory.Exists(newSettings.Workspace)
                )
                {
                    this.appSettings!.LastDir = newSettings.LastDir;
                    this.appSettings.Workspace = newSettings.Workspace;
                    this.appSettings.Theme = newSettings.Theme;
                    this.appSettings.Lang = newSettings.Lang;
                    this.appSettings.Ffmpeg =
                        (newSettings.Ffmpeg == null) ? this.appSettings.Ffmpeg : newSettings.Ffmpeg;
                    this.appSettings.CaptureX =
                        (newSettings.CaptureX == null)
                            ? this.appSettings.CaptureX
                            : newSettings.CaptureX;
                    this.appSettings.CaptureY =
                        (newSettings.CaptureY == null)
                            ? this.appSettings.CaptureY
                            : newSettings.CaptureY;
                    this.appSettings.CaptureWidth =
                        (newSettings.CaptureWidth == null)
                            ? this.appSettings.CaptureWidth
                            : newSettings.CaptureWidth;
                    this.appSettings.CaptureHeight =
                        (newSettings.CaptureHeight == null)
                            ? this.appSettings.CaptureHeight
                            : newSettings.CaptureHeight;

                    this.globalState.theme = newSettings.Theme;
                    var sukiTheme = SukiTheme.GetInstance();

                    if (this.globalState.theme == "dark")
                    {
                        sukiTheme.ChangeBaseTheme(ThemeVariant.Dark);
                    }
                    else
                    {
                        sukiTheme.ChangeBaseTheme(ThemeVariant.Light);
                    }

                    sukiTheme.ChangeColorTheme(
                        new SukiUI.Models.SukiColorTheme(
                            "PlumTheme",
                            AppColors.AppColor,
                            AppColors.AppColor
                        )
                    );
                }
            }
        }

        public string GetTheme()
        {
            return this.appSettings!.Theme;
        }

        public CaptureArea CreateCaptureArea(int canvasWidth, int canvasHeight)
        {
            return new CaptureArea(
                this.appSettings!.CaptureX > 0 ? this.appSettings.CaptureX : 0,
                this.appSettings.CaptureY > 0 ? this.appSettings.CaptureY : 0,
                this.appSettings.CaptureWidth > 0 ? this.appSettings.CaptureWidth : canvasWidth,
                this.appSettings.CaptureHeight > 0 ? this.appSettings.CaptureHeight : canvasHeight,
                this
            );
        }
    }

    public class AppSettingsData()
    {
        [JsonProperty("last_dir")]
        public required string LastDir { get; set; }

        [JsonProperty("workspace")]
        public required string Workspace { get; set; }

        [JsonProperty("theme")]
        public string Theme { get; set; } = "light";

        [JsonProperty("language")]
        public string Lang { get; set; } = "ru";

        [JsonProperty("ffmpeg")]
        public string Ffmpeg { get; set; } = "";

        [JsonProperty("capture_x")]
        public int CaptureX { get; set; } = 0;

        [JsonProperty("capture_y")]
        public int CaptureY { get; set; } = 0;

        [JsonProperty("capture_width")]
        public int CaptureWidth { get; set; }

        [JsonProperty("capture_height")]
        public int CaptureHeight { get; set; }
    }
}
