using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Newtonsoft.Json;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Common.Constants
{
    /// <summary>
    /// Data exchange service
    /// </summary>
    public class GlobalState : INotifyable
    {
        private Project? _currentProject;

        public Project? CurrentProject
        {
            get => _currentProject;
            set
            {
                if (_currentProject != value)
                {
                    _currentProject = value;
                    OnPropertyChanged(nameof(CurrentProject));
                }
            }
        }

        public readonly string SettingsFileName;
        public readonly string AutoSaveFile;

        public JsonError jsonError;
        public JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        };
        public int FPS = 60;

        public event Action TimeUpdated;

        public void OnTimeUpdated()
        {
            TimeUpdated?.Invoke();
        }

        public int currentTab;

        public EasingTypes CurrentEasingType { get; set; } = EasingTypes.LINEAR;

        public Bone? CurrentBone { get; set; } = null;
        public string Theme { get; set; } = "light";
        public bool DrawBones { get; set; } = true;
        public bool SetBasePos { get; set; } = true;
        public bool CaptureMode { get; set; } = false;
        public string GlobalWorkspace { get; set; } = "PlumJsonAnimatorWorkspace";
        public string ProgramExt { get; set; } = ".plmjsn";

        public Canvas? Canvas { get; set; }
        public const int BaseCanvasSize = 1000;
        public int canvasHeight = 1000;
        public int canvasWidth = 1000;

        public double zoomCanvas = 1;

        public bool isAutoSave = true;
        public long autoSaveSec = 300;
        public DateTime lastSaveTime;
        public DateTime LastSaveTime
        {
            set
            {
                if (this.lastSaveTime != value)
                {
                    this.lastSaveTime = value;
                    OnPropertyChanged(nameof(LastSaveTime));
                }
            }
            get => this.lastSaveTime;
        }

        public CaptureArea? captureArea;

        public GlobalState(LocalizationService localizationService)
        {
            this.jsonError = new JsonError(localizationService);

            this.SettingsFileName = $"settings{this.ProgramExt}";
            this.AutoSaveFile = $"autosave{this.ProgramExt}";
        }

        public ParallelOptions GetParallelOptions()
        {
            int processorCount = Environment.ProcessorCount;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = processorCount };
            return parallelOptions;
        }

        public IImmutableBrush GetDotBoneColor(Bone b)
        {
            if (this.CurrentBone == b && this.CurrentBone.IsBone == true)
            {
                return AppColors.Red;
            }
            else
            {
                return AppColors.Green;
            }
        }

        public IImmutableBrush GetLineBoneColor(Bone b)
        {
            if (this.CurrentBone == b && this.CurrentBone.IsBone == true)
            {
                return AppColors.Blue;
            }
            else
            {
                return AppColors.Aqua;
            }
        }

        public bool IsSlotSelected(Slot slot)
        {
            if (this.CurrentBone != null)
            {
                if (!this.CurrentBone.IsBone && this.CurrentBone == slot)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
