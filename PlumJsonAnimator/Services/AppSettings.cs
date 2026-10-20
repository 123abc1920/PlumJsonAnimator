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
    /// <summary>
    /// Provides app settings. It is in "C:\Users\username\AppData\Roaming\PlumJsonAnimator\settings.plmjsn" usually
    /// </summary>
    public class AppSettings
    {
        private string AppSettingsPath;
        private string AppSettingsFile;
        public AppSettingsData? _appSettings = null;

        private GlobalState _globalState;
        private LocalizationService _localizationService;

        public AppSettings(GlobalState globalState, LocalizationService localizationService)
        {
            _globalState = globalState;
            _localizationService = localizationService;

            AppSettingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlumJsonAnimator"
            );
            AppSettingsFile = Path.Combine(
                AppSettingsPath,
                $"settings{_globalState.ProgramExt}"
            );

            _appSettings = new AppSettingsData()
            {
                Lang = LocalizationService.START_LANG,
                LastDir = "",
                Theme = "light",
                Workspace = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    _globalState.GlobalWorkspace,
                    "NewProject"
                ),
                Ffmpeg = "",
                CaptureX = 0,
                CaptureY = 0,
                CaptureWidth = _globalState.canvasWidth,
                CaptureHeight = _globalState.canvasHeight,
            };
        }

        public void SetSettings(AppSettingsData appSettings)
        {
            _appSettings = appSettings;
            SaveSettings();
        }

        /// <summary>
        /// Updates capture area parameters and saves them into settingd file
        /// </summary>
        /// <param name="rect">New capture area form</param>
        public void SetCaptureArea(Rect rect)
        {
            _appSettings!.CaptureX = (int)rect.X;
            _appSettings.CaptureY = (int)rect.Y;
            _appSettings.CaptureWidth = (int)rect.Width;
            _appSettings.CaptureHeight = (int)rect.Height;

            SaveSettings();
        }

        public void ChangeProject(SettingsData newProjectData)
        {
            string newProjectWorkspace = Path.Combine(newProjectData.Path, newProjectData.Name);

            _appSettings!.Workspace = newProjectWorkspace;
            _appSettings!.LastDir = newProjectWorkspace;

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
                JsonConvert.SerializeObject(_appSettings, _globalState.jsonSettings)
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
                    _appSettings!.LastDir = newSettings.LastDir;
                    _appSettings.Workspace = newSettings.Workspace;
                    _appSettings.Theme = newSettings.Theme;
                    _appSettings.Lang = newSettings.Lang;
                    _appSettings.Ffmpeg =
                        (newSettings.Ffmpeg == null) ? _appSettings.Ffmpeg : newSettings.Ffmpeg;
                    _appSettings.CaptureX =
                        (newSettings.CaptureX == null)
                            ? _appSettings.CaptureX
                            : newSettings.CaptureX;
                    _appSettings.CaptureY =
                        (newSettings.CaptureY == null)
                            ? _appSettings.CaptureY
                            : newSettings.CaptureY;
                    _appSettings.CaptureWidth =
                        (newSettings.CaptureWidth == null)
                            ? _appSettings.CaptureWidth
                            : newSettings.CaptureWidth;
                    _appSettings.CaptureHeight =
                        (newSettings.CaptureHeight == null)
                            ? _appSettings.CaptureHeight
                            : newSettings.CaptureHeight;

                    _localizationService.LoadLangResorce(newSettings.Lang);

                    _globalState.Theme = newSettings.Theme;
                    var sukiTheme = SukiTheme.GetInstance();
                    if (_globalState.Theme == "dark")
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
            return _appSettings!.Theme;
        }

        /// <summary>
        /// Sets parameters for capture area
        /// </summary>
        /// <param name="canvasWidth">Width of main canvas</param>
        /// <param name="canvasHeight">Height of main canvas</param>
        public CaptureArea CreateCaptureArea(int canvasWidth, int canvasHeight)
        {
            return new CaptureArea(
                _appSettings!.CaptureX > 0 ? _appSettings.CaptureX : 0,
                _appSettings.CaptureY > 0 ? _appSettings.CaptureY : 0,
                _appSettings.CaptureWidth > 0 ? _appSettings.CaptureWidth : canvasWidth,
                _appSettings.CaptureHeight > 0 ? _appSettings.CaptureHeight : canvasHeight,
                this
            );
        }
    }

    /// <summary>
    /// Contains jsonifyed app settings
    /// </summary>
    public class AppSettingsData()
    {
        [JsonProperty("last_dir")]
        public required string LastDir { get; set; }

        [JsonProperty("workspace")]
        public required string Workspace { get; set; }

        [JsonProperty("theme")]
        public string Theme { get; set; } = "light";

        [JsonProperty("language")]
        public string Lang { get; set; } = LocalizationService.START_LANG;

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
