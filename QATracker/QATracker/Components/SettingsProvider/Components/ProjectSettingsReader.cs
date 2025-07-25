using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace QATracker.Components.SettingsProvider.Components
{
    public class ProjectSettingsReader
    {
        /// <summary>
        /// Reads all verification-related settings for the project (project-level SettingsBundle).
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ReadProjectVerificationSettings(IProject project, Language language = null)
        {
            var projectInfo = project.GetProjectInfo();
            var sdlprojFilePath = Path.Combine(projectInfo.LocalProjectFolder, $"{projectInfo.Name}.sdlproj");

            var doc = XDocument.Load(sdlprojFilePath);

            var result = new Dictionary<string, Dictionary<string, string>>();
            AddMissingCategories(result);

            string settingsBundleGuid;
            if (language == null)
            {
                // Project-level
                var projectElement = doc.Root;
                settingsBundleGuid = projectElement?.Attribute("SettingsBundleGuid")?.Value;
            }
            else
            {
                // Language-specific
                var langFile = doc.Descendants("LanguageDirection")
                    .FirstOrDefault(lf => (string)lf.Attribute("TargetLanguageCode") == language.ToString());
                settingsBundleGuid = langFile?.Attribute("SettingsBundleGuid")?.Value;
            }

            if (string.IsNullOrEmpty(settingsBundleGuid))
                return result;

            var settingsBundle = doc
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
                var settings = new Dictionary<string, string>();

                foreach (var setting in group.Elements("Setting"))
                {
                    var settingId = setting.Attribute("Id")?.Value ?? "";
                    string value;

                    // If the setting has child elements, serialize them as XML string
                    if (setting.HasElements)
                    {
                        value = setting.ToString(SaveOptions.DisableFormatting);
                    }
                    else
                    {
                        value = setting.Value?.Trim() ?? "";
                    }

                    settings[settingId] = value;
                }

                result[groupId] = settings;
            }


            return result;
        }

        //TODO Remove hard-coding
        private static void AddMissingCategories(Dictionary<string, Dictionary<string, string>> result)
        {
            if (!result.ContainsKey(Constants.SettingsTagVerifier))
                result[Constants.SettingsTagVerifier] = new();
            if (!result.ContainsKey(Constants.SettingsTermVerifier))
                result[Constants.SettingsTermVerifier] = new();
            if (!result.ContainsKey(Constants.QaVerificationSettings))
                result[Constants.QaVerificationSettings] = new();
            if (!result.ContainsKey(Constants.NumberVerifierSettings))
                result[Constants.NumberVerifierSettings] = new();
        }
    }
}