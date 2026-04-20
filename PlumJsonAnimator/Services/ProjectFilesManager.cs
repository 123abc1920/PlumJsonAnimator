using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models;
using PlumJsonAnimator.Models.Resources;

namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Provides methods for work with project files
    /// </summary>
    public class ProjectFilesManager
    {
        private ProjectSettings _projectSettings;
        private AppSettings _appSettings;
        private GlobalState _globalState;
        private Interpolation _interpolation;
        private LocalizationService _localizationService;

        public ProjectFilesManager(
            ProjectSettings projectSettings,
            AppSettings appSettings,
            GlobalState globalState,
            Interpolation interpolation,
            LocalizationService localizationService
        )
        {
            this._projectSettings = projectSettings;
            this._appSettings = appSettings;
            this._globalState = globalState;
            this._interpolation = interpolation;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Open dialog for open existing projects
        /// </summary>
        /// <param name="window">Width of main canvas</param>
        public async Task<string?> OpenProjectDialog(Window window)
        {
            var storageProvider = window.StorageProvider;

            var fileTypeFilter = new FilePickerFileType[]
            {
                new($"*{this._globalState.programExt}")
                {
                    Patterns = new[] { $"*{this._globalState.programExt}" },
                },
            };

            var result = await storageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title =
                        $"{this._localizationService.GetMessage(LocalizationConsts.SELECT_SETTINGS_FILE)}",
                    AllowMultiple = false,
                    FileTypeFilter = fileTypeFilter,
                }
            );

            return result?.FirstOrDefault()?.Path.LocalPath;
        }

        public Project? OpenProject(string? path)
        {
            if (path != null && path != "")
            {
                this._projectSettings.ReadSettings(path);
                SettingsData settingsData = this._projectSettings.GetSettingsData();
                this._appSettings.ChangeProject(Path.Combine(settingsData.Path, settingsData.Name));

                Project newProject = new Project(
                    settingsData.Name,
                    settingsData.Path,
                    this._globalState,
                    this._interpolation,
                    this._localizationService
                );

                this._projectSettings.WriteSettings();
                this._appSettings.SaveSettings();
                LoadRes(newProject);

                return newProject;
            }
            return null;
        }

        public Project? NewProject(string? projectName, string? projectPath)
        {
            if (projectName != null && projectPath != null)
            {
                this._projectSettings.WriteSettings();
                this._appSettings.ChangeProject(Path.Combine(projectPath, projectName));

                Project newProject = new Project(
                    projectName,
                    projectPath,
                    this._globalState,
                    this._interpolation,
                    this._localizationService
                );

                this._projectSettings.UpdateSettings(newProject);
                this._projectSettings.WriteSettings();
                this._appSettings.SaveSettings();
                LoadRes(newProject);

                return newProject;
            }
            return null;
        }

        /// <summary>
        /// Return real project directory in file system
        /// </summary>
        /// <param name="project">Project</param>
        public string GetProjectDir(Project? project)
        {
            if (project == null)
            {
                return "";
            }

            string projectPath = Path.Combine(project.ProjectPath, project.Name);

            if (!Directory.Exists(projectPath))
            {
                Directory.CreateDirectory(projectPath);
            }
            if (!Directory.Exists(Path.Combine(projectPath, "res")))
            {
                Directory.CreateDirectory(Path.Combine(projectPath, "res"));
            }

            return projectPath;
        }

        public string CopyRes(string resName, string filePath, Project? project)
        {
            if (project == null)
            {
                return "";
            }

            string projectPath = Path.Combine(
                this._globalState.CurrentProject!.ProjectPath,
                this._globalState.CurrentProject.Name
            );
            string resDir = Path.Combine(projectPath, "res");

            if (File.Exists(filePath))
            {
                string ext = Path.GetExtension(filePath);
                string resPath = Path.Combine(resDir, $"{resName}{ext}");
                File.Copy(filePath, resPath, true);

                return ext;
            }

            return "";
        }

        public bool DeleteResource(string name, string ext, Project? project)
        {
            if (project == null)
            {
                return false;
            }

            try
            {
                string projectPath = Path.Combine(project.ProjectPath, project.Name);
                string resDir = Path.Combine(projectPath, "res");
                string resPath = Path.Combine(resDir, $"{name}{ext}");

                if (File.Exists(resPath))
                {
                    File.Delete(resPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{this._localizationService.GetMessage(LocalizationConsts.DELETE_ERROR)}: {ex.Message}"
                );
                return false;
            }
        }

        public void RenameProject(string oldDir, string newDir)
        {
            if (Directory.Exists(oldDir) && oldDir != newDir)
            {
                Directory.Move(oldDir, newDir);
            }
        }

        /// <summary>
        /// Sets new pathes to project resource objects after moving project into another directory
        /// </summary>
        /// <param name="project"></param>
        public void MoveRes(Project project)
        {
            foreach (Res res in project.Resources)
            {
                res.SetPath(Path.Combine(project.ProjectPath, project.Name));
            }
        }

        public void RenameFile(string oldFile, string newFile)
        {
            if (File.Exists(oldFile) && oldFile != newFile)
            {
                File.Move(oldFile, newFile);
            }
        }

        /// <summary>
        /// Cipies all project files into new directory
        /// </summary>
        /// <param name="oldDir"></param>
        /// <param name="newDir"></param>
        public void CopyDir(string oldDir, string newDir)
        {
            if (Directory.Exists(oldDir) && oldDir != newDir)
            {
                Directory.CreateDirectory(newDir);

                var files = Directory.GetFiles(oldDir);
                var options = this._globalState.GetParallelOptions();
                Parallel.ForEach(
                    files,
                    options,
                    file =>
                    {
                        try
                        {
                            string fileName = Path.GetFileName(file);
                            string destFile = Path.Combine(newDir, fileName);
                            File.Copy(file, destFile, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error copying {file}: {ex.Message}");
                        }
                    }
                );

                foreach (string subDir in Directory.GetDirectories(oldDir))
                {
                    string dirName = Path.GetFileName(subDir);
                    string destDir = Path.Combine(newDir, dirName);
                    CopyDir(subDir, destDir);
                }

                Directory.Delete(oldDir, true);
            }
        }

        /// <summary>
        /// Load all resources into project
        /// </summary>
        /// <param name="project"></param>
        public void LoadRes(Project project)
        {
            string directoryPath = Path.Combine(project.ProjectPath, project.Name, "res");
            string[] extensions = { "*.png", "*.jpg", "*.jpeg" };

            try
            {
                var allFiles = extensions
                    .SelectMany(ext => Directory.GetFiles(directoryPath, ext))
                    .Select(filePath =>
                    {
                        return new ImageRes(
                            this,
                            this._globalState,
                            filePath,
                            Path.GetFileNameWithoutExtension(filePath),
                            Path.GetExtension(filePath)
                        );
                    })
                    .ToList();

                foreach (var imageRes in allFiles)
                {
                    project.Resources.Add(imageRes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{this._localizationService.GetMessage(LocalizationConsts.ERROR)}: {ex.Message}"
                );
            }
        }
    }
}
