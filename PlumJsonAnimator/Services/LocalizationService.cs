using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Models.Interfaces;

// TODO: time textblock
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

        public string GetMessage(LocalizationConsts constStr)
        {
            var key = constStr switch
            {
                LocalizationConsts.EXPORT_AS_GIF => "export_as_gif",
                LocalizationConsts.EXPORT_AS_MP4 => "export_as_mp4",
                LocalizationConsts.EXPORT_AS_PNG => "export_as_png",
                LocalizationConsts.EXPORT_AS_JPG => "export_as_jpg",
                LocalizationConsts.SETTINGS => "settings",
                LocalizationConsts.NEW_PROJECT => "new_project",
                LocalizationConsts.ANIM_SUCCESS => "anim_success",
                LocalizationConsts.FOLDER_ERROR => "folder_error",
                LocalizationConsts.FILE_NOT_EXIST => "file_not_exist",
                LocalizationConsts.FILE_DAMAGED => "file_damaged",
                LocalizationConsts.IMPORT_SUCCESS => "import_success",
                LocalizationConsts.EXPORT_SUCCESS => "export_success",
                LocalizationConsts.INCORRECT_TIME => "incorrect_time",
                LocalizationConsts.INPUT_NAME => "input_name",
                LocalizationConsts.INPUT_FOLDER => "input_foler",
                LocalizationConsts.FOLDER_NOT_EXIST => "folder_not_exist",
                LocalizationConsts.FFMPEG_NOT_EXIST => "ffmpeg_not_exist",
                LocalizationConsts.INPUT_FFMPEG => "input_ffmpeg",

                _ => null,
            };

            return key != null && LangResources.TryGetValue(key, out var value)
                ? value?.ToString() ?? string.Empty
                : string.Empty;
        }
    }

    public enum LocalizationConsts
    {
        EXPORT_AS_GIF,
        EXPORT_AS_MP4,
        EXPORT_AS_PNG,
        EXPORT_AS_JPG,
        SETTINGS,
        NEW_PROJECT,
        ANIM_SUCCESS,
        FOLDER_ERROR,
        FILE_NOT_EXIST,
        FILE_DAMAGED,
        IMPORT_SUCCESS,
        EXPORT_SUCCESS,
        INCORRECT_TIME,
        INPUT_NAME,
        INPUT_FOLDER,
        FOLDER_NOT_EXIST,
        FFMPEG_NOT_EXIST,
        INPUT_FFMPEG,
    }
}
