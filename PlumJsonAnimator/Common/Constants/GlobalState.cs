using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Newtonsoft.Json;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Common.Constants
{
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

        public Dialogs.Dialogs dialogs;

        public Bone? currentBone = null;
        public string theme = "light";
        public bool drawBones = true;
        public bool captureMode = false;
        public string workspace = "PlumJsonAnimatorWorkspace";
        public string programExt = ".plmjsn";

        public const int BASE_CANVAS_SIZE = 1000;
        public int canvasHeight = 1000;
        public int canvasWidth = 1000;

        public double zoomCanvas = 1;

        public CaptureArea? captureArea;

        public ParallelOptions GetParallelOptions()
        {
            int processorCount = Environment.ProcessorCount;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = processorCount };
            return parallelOptions;
        }

        public IImmutableBrush getDotBoneColor(Bone b)
        {
            if (this.currentBone == b && this.currentBone.IsBone == true)
            {
                return AppColors.Red;
            }
            else
            {
                return AppColors.Green;
            }
        }

        public IImmutableBrush getLineBoneColor(Bone b)
        {
            if (this.currentBone == b && this.currentBone.IsBone == true)
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
            if (this.currentBone != null)
            {
                if (!this.currentBone.IsBone && this.currentBone == slot)
                {
                    return true;
                }
            }
            return false;
        }

        public GlobalState(Dialogs.Dialogs dialogs, LocalizationService localizationService)
        {
            this.dialogs = dialogs;

            this.jsonError = new JsonError(localizationService);
        }
    }
}
