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
            _projectManager = projectManager;
            _globalState = globalState;
        }

        public string GetName
        {
            get => Name;
            set
            {
                if (Name != value)
                {
                    Name = value;
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
                if (_preview == null && !string.IsNullOrEmpty(Path) && File.Exists(Path))
                {
                    try
                    {
                        _preview = new Bitmap(Path);
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
                if (_globalState.CurrentProject.IsUniqRes(name) == true)
                {
                    Name = name;
                    _projectManager.RenameFile(
                        Path,
                        System.IO.Path.Combine(
                            _globalState.CurrentProject!.GetProjectPath(),
                            "res",
                            $"{Name}{ext}"
                        )
                    );
                }
            }
        }

        public void SetPath(string newProjectPath)
        {
            Path = System.IO.Path.Combine(newProjectPath, "res", $"{Name}{ext}");
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
            Name = name;
            ext = _ext;
            Path = _path;
        }
    }
}
