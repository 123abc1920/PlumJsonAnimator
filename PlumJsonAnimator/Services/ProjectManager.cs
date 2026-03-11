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
    public class ProjectManager
    {
        private ProjectSettings projectSettings;
        private AppSettings appSettings;
        private GlobalState globalState;
        private Interpolation interpolation;

        public ProjectManager(
            ProjectSettings projectSettings,
            AppSettings appSettings,
            GlobalState globalState,
            Interpolation interpolation
        )
        {
            this.projectSettings = projectSettings;
            this.appSettings = appSettings;
            this.globalState = globalState;
            this.interpolation = interpolation;
        }

        public async Task<string?> OpenProject(Window window)
        {
            var storageProvider = window.StorageProvider;

            var fileTypeFilter = new FilePickerFileType[]
            {
                new($"*{this.globalState.programExt}")
                {
                    Patterns = new[] { $"*{this.globalState.programExt}" },
                },
            };

            var result = await storageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Выберите файл настроек проекта",
                    AllowMultiple = false,
                    FileTypeFilter = fileTypeFilter,
                }
            );

            return result?.FirstOrDefault()?.Path.LocalPath;
        }

        public Project? NewProject(string? projectName, string? projectPath)
        {
            if (projectName != null && projectPath != null)
            {
                this.appSettings.NewProject(Path.Combine(projectPath, projectName));
                this.projectSettings.WriteAllSettings();

                Project newProject = new Project(
                    projectName,
                    projectPath,
                    this.globalState,
                    this.interpolation
                );

                this.projectSettings.WriteAllSettings();
                this.appSettings.SaveSettings();
                LoadRes();

                Console.WriteLine("kkk");

                return newProject;
            }
            return null;
        }

        public string CreateProjectDir()
        {
            string projectPath = Path.Combine(
                this.globalState.currentProject!.ProjectPath,
                this.globalState.currentProject.Name
            );

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

        public string CopyRes(string resName, string filePath)
        {
            string projectPath = Path.Combine(
                this.globalState.currentProject!.ProjectPath,
                this.globalState.currentProject.Name
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

        public bool DeleteResource(string name, string ext)
        {
            try
            {
                string projectPath = Path.Combine(
                    this.globalState.currentProject!.ProjectPath,
                    this.globalState.currentProject.Name
                );
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
                Console.WriteLine($"Ошибка удаления: {ex.Message}");
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

        public void RenameFile(string oldFile, string newFile)
        {
            if (File.Exists(oldFile) && oldFile != newFile)
            {
                File.Move(oldFile, newFile);
            }
        }

        public void CopyDir(string oldDir, string newDir)
        {
            if (Directory.Exists(oldDir) && oldDir != newDir)
            {
                Directory.CreateDirectory(newDir);

                var files = Directory.GetFiles(oldDir);
                var options = this.globalState.GetParallelOptions();
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

        public void LoadRes()
        {
            string directoryPath = Path.Combine(
                this.globalState.currentProject!.ProjectPath,
                this.globalState.currentProject.Name,
                "res"
            );
            string[] extensions = { "*.png", "*.jpg", "*.jpeg" };

            try
            {
                var allFiles = extensions
                    .SelectMany(ext => Directory.GetFiles(directoryPath, ext))
                    .Select(filePath =>
                    {
                        return new ImageRes(
                            this,
                            this.globalState,
                            filePath,
                            Path.GetFileNameWithoutExtension(filePath),
                            Path.GetExtension(filePath)
                        );
                    })
                    .ToList();

                foreach (var imageRes in allFiles)
                {
                    this.globalState.currentProject.Resources.Add(imageRes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
