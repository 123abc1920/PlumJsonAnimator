using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimEngine.Resources;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Common.Constants;

namespace AnimEngine.Project
{
    public class ProjectManager
    {
        public static async Task<string?> OpenProject(Window window)
        {
            var storageProvider = window.StorageProvider;

            var fileTypeFilter = new FilePickerFileType[]
            {
                new($"*{ConstantsClass.programExt}")
                {
                    Patterns = new[] { $"*{ConstantsClass.programExt}" },
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

        public static bool NewProject(string? projectName, string? projectPath)
        {
            if (projectName != null && projectPath != null)
            {
                ProjectSettings.WriteAllSettings();
                ConstantsClass.currentProject = new Project(projectName, projectPath);
                ProjectSettings.WriteAllSettings();
                AppSettings.SaveSettings();
                LoadRes();
                return true;
            }
            return false;
        }

        public static string CreateProjectDir()
        {
            string projectPath = Path.Combine(
                ConstantsClass.currentProject!.ProjectPath,
                ConstantsClass.currentProject.Name
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

        public static string CopyRes(string resName, string filePath)
        {
            string projectPath = Path.Combine(
                ConstantsClass.currentProject!.ProjectPath,
                ConstantsClass.currentProject.Name
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

        public static bool DeleteResource(string name, string ext)
        {
            try
            {
                string projectPath = Path.Combine(
                    ConstantsClass.currentProject!.ProjectPath,
                    ConstantsClass.currentProject.Name
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

        public static void RenameProject(string oldDir, string newDir)
        {
            if (Directory.Exists(oldDir) && oldDir != newDir)
            {
                Directory.Move(oldDir, newDir);
            }
        }

        public static void RenameFile(string oldFile, string newFile)
        {
            if (File.Exists(oldFile) && oldFile != newFile)
            {
                File.Move(oldFile, newFile);
            }
        }

        public static void CopyDir(string oldDir, string newDir)
        {
            if (Directory.Exists(oldDir) && oldDir != newDir)
            {
                Directory.CreateDirectory(newDir);

                var files = Directory.GetFiles(oldDir);
                var options = ConstantsClass.GetParallelOptions();
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

        public static void LoadRes()
        {
            string directoryPath = Path.Combine(
                ConstantsClass.currentProject!.ProjectPath,
                ConstantsClass.currentProject.Name,
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
                            filePath,
                            Path.GetFileNameWithoutExtension(filePath),
                            Path.GetExtension(filePath)
                        );
                    })
                    .ToList();

                foreach (var imageRes in allFiles)
                {
                    ConstantsClass.currentProject.Resources.Add(imageRes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
