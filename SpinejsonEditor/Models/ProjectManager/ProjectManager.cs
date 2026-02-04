using System;
using System.IO;
using Constants;

namespace ProjectManager
{
    public class ProjectManager
    {
        public static string CreateProjectDir()
        {
            string projectPath = Path.Combine(
                ConstantsClass.currentProject.ProjectPath,
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
                ConstantsClass.currentProject.ProjectPath,
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
                    ConstantsClass.currentProject.ProjectPath,
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
            if (Directory.Exists(oldDir))
            {
                Directory.Move(oldDir, newDir);
            }
        }

        public static void CopyDir(string oldDir, string newDir)
        {
            if (Directory.Exists(oldDir))
            {
                Directory.CreateDirectory(newDir);

                foreach (string file in Directory.GetFiles(oldDir))
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(newDir, fileName);
                    File.Copy(file, destFile, true);
                }

                foreach (string subDir in Directory.GetDirectories(oldDir))
                {
                    string dirName = Path.GetFileName(subDir);
                    string destDir = Path.Combine(newDir, dirName);
                    CopyDir(subDir, destDir);
                }

                Directory.Delete(oldDir, true);
            }
        }
    }
}
