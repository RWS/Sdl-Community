using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QATracker.Components.SettingsProvider.Components
{
    public class SettingsReader
    {
        private readonly XDocument _doc;

        private static List<string> DefaultVerifiers { get; } =
        [
            Constants.SettingsTagVerifier,
            Constants.SettingsTermVerifier,
            Constants.QaVerificationSettings
        ];

        public SettingsReader(string sdlprojFilePath)
        {
            _doc = XDocument.Load(sdlprojFilePath);
        }

        /// <summary>
        /// Reads all verification-related settings for the project (project-level SettingsBundle).
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ReadProjectVerificationSettings(Language language = null)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            string settingsBundleGuid = null;

            if (language == null)
            {
                // Project-level
                var projectElement = _doc.Root;
                settingsBundleGuid = projectElement?.Attribute("SettingsBundleGuid")?.Value;
            }
            else
            {
                // Language-specific
                var langFile = _doc.Descendants("LanguageDirection")
                    .FirstOrDefault(lf => (string)lf.Attribute("TargetLanguageCode") == language.ToString());
                settingsBundleGuid = langFile?.Attribute("SettingsBundleGuid")?.Value;
            }

            if (string.IsNullOrEmpty(settingsBundleGuid))
                return result;

            var settingsBundle = _doc
                .Descendants("SettingsBundle")
                .FirstOrDefault(sb => (string)sb.Attribute("Guid") == settingsBundleGuid);

            if (settingsBundle == null)
                return result;

            var settingsGroups = settingsBundle
                .Descendants("SettingsGroup")
                .Where(g => g.Attribute("Id") != null &&
                            g.Attribute("Id").Value.ToLower().Contains("verif"));

            foreach (var group in settingsGroups)
            {
                var groupId = group.Attribute("Id").Value;
                var settings = group.Elements("Setting")
                    .ToDictionary(
                        s => s.Attribute("Id")?.Value ?? "",
                        s => s.Value?.Trim() ?? ""
                    );
                result[groupId] = settings;
            }

            AddMissingCategories(result);

            return result;
        }


        private static void AddMissingCategories(Dictionary<string, Dictionary<string, string>> result)
        {
            if (!result.ContainsKey(Constants.SettingsTagVerifier))
                result[Constants.SettingsTagVerifier] = new();
            if (!result.ContainsKey(Constants.SettingsTermVerifier))
                result[Constants.SettingsTermVerifier] = new();
            if (!result.ContainsKey(Constants.QaVerificationSettings))
                result[Constants.QaVerificationSettings] = new();
        }
    }
}
