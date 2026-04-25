using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

// TODO: fix Name GetName SetName
namespace PlumJsonAnimator.Models.Resources
{
    /// <summary>
    /// Resources -- images, textures etc
    /// </summary>
    public class Res : INotifyable, IRenamable
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        protected ProjectFilesManager _projectManager;
        protected GlobalState _globalState;

        public Res(ProjectFilesManager projectManager, GlobalState globalState)
        {
            this._projectManager = projectManager;
            this._globalState = globalState;
        }

        public string GetName
        {
            get => this.Name;
            set
            {
                if (this.Name != value)
                {
                    this.Name = value;
                }
            }
        }

        protected string _path = "";
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    _path = _path.Replace('\\', '/');
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        private IImage? _preview;
        public IImage? Preview
        {
            get
            {
                if (_preview == null && !string.IsNullOrEmpty(this.Path) && File.Exists(this.Path))
                {
                    try
                    {
                        _preview = new Bitmap(this.Path);
                    }
                    catch { }
                }
                return _preview;
            }
        }

        public string ext = ".png";

        // TODO: show toast about names
        public void SetName(string? name)
        {
            if (name != null)
            {
                if (this._globalState.CurrentProject.IsUniqRes(name) == true)
                {
                    this.Name = name;
                    this._projectManager.RenameFile(
                        Path,
                        System.IO.Path.Combine(
                            this._globalState.CurrentProject!.GetProjectPath(),
                            "res",
                            $"{this.Name}{ext}"
                        )
                    );
                }
            }
        }

        public void SetPath(string newProjectPath)
        {
            this.Path = System.IO.Path.Combine(newProjectPath, "res", $"{this.Name}{this.ext}");
        }
    }

    /// <summary>
    /// Image resources, which are uploaded by user
    /// </summary>
    public class ImageRes : Res
    {
        public int width;
        public int height;

        public ImageRes(ProjectFilesManager projectManager, GlobalState globalState)
            : base(projectManager, globalState) { }

        public ImageRes(
            ProjectFilesManager projectManager,
            GlobalState globalState,
            string _path,
            string name,
            string _ext
        )
            : base(projectManager, globalState)
        {
            this.Name = name;
            this.ext = _ext;
            this.Path = _path;
        }
    }
}
