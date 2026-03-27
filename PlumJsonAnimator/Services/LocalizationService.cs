using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// TODO: toggle btns for modes
namespace PlumJsonAnimator.Services
{
    public class LocalizationService
    {
        public const string START_LANG = "ru-RU";

        private string LocPath;

        public List<string> langs = new List<string>();
        public string currentLang = START_LANG;

        public LocalizationService()
        {
            LocPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlumJsonAnimator",
                "langs"
            );

            if (!Directory.Exists(LocPath))
            {
                Directory.CreateDirectory(LocPath);
                CopyDefaultTranslations();
            }
        }

        private void CopyDefaultTranslations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly
                .GetManifestResourceNames()
                .Where(name => name.EndsWith(".json") && name.Contains("Translations"));

            foreach (var resourceName in resourceNames)
            {
                var fileName = resourceName
                    .Replace("PlumJsonAnimator.Translations.", "")
                    .Replace("PlumJsonAnimator.", "")
                    .TrimStart('.');

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();

                    string destPath = Path.Combine(LocPath, fileName);
                    File.WriteAllText(destPath, content);
                }
            }
        }

        public void LoadLangs()
        {
            var jsonFiles = Directory.GetFiles(LocPath, "*.json");

            foreach (var filePath in jsonFiles)
            {
                var languageCode = Path.GetFileNameWithoutExtension(filePath);
                this.langs.Add(languageCode);
            }
        }
    }
}
