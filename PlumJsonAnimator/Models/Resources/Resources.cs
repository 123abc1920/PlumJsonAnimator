using System.IO;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

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

        protected ProjectManager projectManager;
        protected GlobalState globalState;

        public Res(ProjectManager projectManager, GlobalState globalState)
        {
            this.projectManager = projectManager;
            this.globalState = globalState;
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

        public string path = "";
        public string ext = ".png";

        public void SetName(string? name)
        {
            if (name != null)
            {
                this.Name = name;
                this.projectManager.RenameFile(
                    path,
                    Path.Combine(
                        this.globalState.CurrentProject!.GetProjectPath(),
                        "res",
                        $"{this.Name}{ext}"
                    )
                );
            }
        }

        public void SetPath(string newProjectPath)
        {
            this.path = Path.Combine(newProjectPath, "res", $"{this.Name}{this.ext}");
        }
    }

    /// <summary>
    /// Image resources, which are uploaded by user
    /// </summary>
    public class ImageRes : Res
    {
        public int width;
        public int height;

        public ImageRes(ProjectManager projectManager, GlobalState globalState)
            : base(projectManager, globalState) { }

        public ImageRes(
            ProjectManager projectManager,
            GlobalState globalState,
            string _path,
            string name,
            string _ext
        )
            : base(projectManager, globalState)
        {
            this.Name = name;
            this.ext = _ext;
            this.path = _path;
        }
    }
}
