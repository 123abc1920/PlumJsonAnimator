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

        public JsonError JsonError { get; set; }
        public JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        };
        public int FPS { get; set; } = 60;

        public event Action TimeUpdated;

        public void OnTimeUpdated()
        {
            TimeUpdated?.Invoke();
        }

        public int CurrentTab { get; set; }

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
        public int CanvasHeight { get; set; } = 1000;
        public int CanvasWidth { get; set; } = 1000;

        public double ZoomCanvas { get; set; } = 1;

        public bool IsAutoSave { get; set; } = true;
        public long AutoSaveSec { get; set; } = 300;
        private DateTime _lastSaveTime;
        public DateTime LastSaveTime
        {
            set
            {
                if (_lastSaveTime != value)
                {
                    _lastSaveTime = value;
                    OnPropertyChanged(nameof(LastSaveTime));
                }
            }
            get => _lastSaveTime;
        }

        public CaptureArea? CaptureArea { get; set; }

        public GlobalState(LocalizationService localizationService)
        {
            JsonError = new JsonError(localizationService);

            SettingsFileName = $"settings{ProgramExt}";
            AutoSaveFile = $"autosave{ProgramExt}";
        }

        public ParallelOptions GetParallelOptions()
        {
            int processorCount = Environment.ProcessorCount;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = processorCount };
            return parallelOptions;
        }

        public IImmutableBrush GetDotBoneColor(Bone b)
        {
            if (CurrentBone == b && CurrentBone.IsBone == true)
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
            if (CurrentBone == b && CurrentBone.IsBone == true)
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
            if (CurrentBone != null)
            {
                if (!CurrentBone.IsBone && CurrentBone == slot)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
