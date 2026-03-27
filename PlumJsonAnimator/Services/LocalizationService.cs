using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Models.Interfaces;

// TODO: toggle btns for modes
namespace PlumJsonAnimator.Services
{
    public class LocalizationService : INotifyable
    {
        public const string START_LANG = "ru-RU";

        private string LocPath;

        public List<string> langs = new List<string>();
        public string currentLang = START_LANG;

        public ResourceDictionary LangResources = new();

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

        public void LoadLangResorce(string lang)
        {
            this.currentLang = lang;

            string filePath = Path.Combine(LocPath, $"{lang}.json");

            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(LocPath, $"{START_LANG}.json");
                if (!File.Exists(filePath))
                {
                    return;
                }
            }

            string jsonContent = File.ReadAllText(filePath);

            var translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                jsonContent
            );

            if (translations == null)
            {
                return;
            }

            LangResources.Clear();

            foreach (var (key, value) in translations)
            {
                LangResources[key] = value;
            }
        }
    }
}
