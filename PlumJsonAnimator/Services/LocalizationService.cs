using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Models.Interfaces;

// TODO: setup animation key frame when 1 keyframe
namespace PlumJsonAnimator.Services
{
    /// <summary>
    /// Provides methods for localize application
    /// </summary>
    public class LocalizationService : INotifyable
    {
        public const string START_LANG = "ru-RU";

        private string LocalizationFilesPath;

        public List<string> langs = new List<string>();
        public string currentLang = START_LANG;

        public ResourceDictionary LangResources = new();

        public LocalizationService()
        {
            LocalizationFilesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlumJsonAnimator",
                "langs"
            );

            if (!Directory.Exists(LocalizationFilesPath))
            {
                Directory.CreateDirectory(LocalizationFilesPath);
                CopyDefaultTranslations();
            }
        }

        /// <summary>
        /// Copy default ru-RU and en-US files to user PC
        /// </summary>
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

                    string destPath = Path.Combine(LocalizationFilesPath, fileName);
                    File.WriteAllText(destPath, content);
                }
            }
        }

        /// <summary>
        /// Reads all files in LocalizationFilesPath and collect them into this.langs
        /// </summary>
        public void LoadLangs()
        {
            var jsonFiles = Directory.GetFiles(LocalizationFilesPath, "*.json");

            foreach (var filePath in jsonFiles)
            {
                var languageCode = Path.GetFileNameWithoutExtension(filePath);
                this.langs.Add(languageCode);
            }
        }

        /// <summary>
        /// Sets current language resources into ResourceDictionary
        /// </summary>
        /// <param name="lang">Lang that has to be loaded</param>
        private void loadLangRes(string lang)
        {
            this.currentLang = lang;

            string filePath = Path.Combine(LocalizationFilesPath, $"{lang}.json");

            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(LocalizationFilesPath, $"{START_LANG}.json");
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

        public void LoadLangResorce(string lang)
        {
            loadLangRes(lang);
        }

        public void LoadLangResorce()
        {
            loadLangRes(START_LANG);
        }

        /// <summary>
        /// Returns localizated data, needs word code
        /// </summary>
        /// <param name="constStr">Word code</param>
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
                LocalizationConsts.INPUT_FOLDER => "input_folder",
                LocalizationConsts.FOLDER_NOT_EXIST => "folder_not_exist",
                LocalizationConsts.FFMPEG_NOT_EXIST => "ffmpeg_not_exist",
                LocalizationConsts.INPUT_FFMPEG => "input_ffmpeg",
                LocalizationConsts.ERROR => "error",
                LocalizationConsts.JSON_VALID => "json_valid",
                LocalizationConsts.JSON_ERROR_STR => "json_error_str",
                LocalizationConsts.JSON_ERROR_POS => "json_error_pos",
                LocalizationConsts.JSON_CONTEXT => "json_context",
                LocalizationConsts.RENAME => "rename",
                LocalizationConsts.INFO_MESSAGE => "info_message",
                LocalizationConsts.SAVED => "saved",
                LocalizationConsts.REGENERATE_ERROR => "regenerate_error",
                LocalizationConsts.BONE_NULL_NAME_ERROR => "bone_null_name_error",
                LocalizationConsts.BONE_PARENT_NULL_ERROR => "bone_parent_null_error",
                LocalizationConsts.SEVERAL_ROOT_BONES => "several_root_bones",
                LocalizationConsts.NO_ROOT_BONE => "no_root_bone",
                LocalizationConsts.DELETE_ERROR => "delete_error",
                LocalizationConsts.SELECT_SETTINGS_FILE => "select_settings_file",
                LocalizationConsts.SELECT_FOLDER => "select_folder",
                LocalizationConsts.SELECT_FFMPEG => "select_ffmpeg",
                LocalizationConsts.SLOT_NULL_NAME => "slot_null_name",
                LocalizationConsts.EMPTY_JSON => "empty_json",
                LocalizationConsts.BASE_POS => "base_pos",

                _ => null,
            };

            return key != null && LangResources.TryGetValue(key, out var value)
                ? value?.ToString() ?? string.Empty
                : string.Empty;
        }
    }

    /// <summary>
    /// All word codes
    /// </summary>
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
        ERROR,
        JSON_VALID,
        JSON_ERROR_STR,
        JSON_ERROR_POS,
        JSON_CONTEXT,
        RENAME,
        INFO_MESSAGE,
        SAVED,
        REGENERATE_ERROR,
        BONE_NULL_NAME_ERROR,
        BONE_PARENT_NULL_ERROR,
        SEVERAL_ROOT_BONES,
        NO_ROOT_BONE,
        DELETE_ERROR,
        SELECT_SETTINGS_FILE,
        SELECT_FOLDER,
        SELECT_FFMPEG,
        SLOT_NULL_NAME,
        EMPTY_JSON,
        BASE_POS,
    }
}
