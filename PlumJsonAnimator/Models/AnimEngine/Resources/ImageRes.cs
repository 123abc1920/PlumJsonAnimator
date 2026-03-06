using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using AnimEngine.Models;
using AnimEngine.Project;
using Common.Constants;

namespace AnimEngine.Resources
{
    /// <summary>
    /// Resources -- images, textures etc
    /// </summary>
    public class Res : IRenamable, INotifyPropertyChanged
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

        public string path;
        public string ext;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetName(string? name)
        {
            if (name != null)
            {
                this.Name = name;
                ProjectManager.RenameFile(
                    path,
                    Path.Combine(
                        ConstantsClass.currentProject.GetProjectPath(),
                        "res",
                        $"{this.Name}{ext}"
                    )
                );
            }
        }
    }

    /// <summary>
    /// Image resources, which are uploaded by user
    /// </summary>
    public class ImageRes : Res
    {
        public int width;
        public int height;

        public ImageRes() { }

        public ImageRes(string _path, string name, string _ext)
        {
            this.Name = name;
            this.ext = _ext;
            this.path = _path;
        }
    }
}
