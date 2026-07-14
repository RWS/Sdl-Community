using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NLog;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using VerifyFilesAuditReport.Logging;

namespace VerifyFilesAuditReport.Components.SettingsProvider.Components
{
    public class ProjectSettingsReader
    {
        private static readonly Logger Logger = Log.GetLogger(typeof(ProjectSettingsReader).FullName);

        /// <summary>
        /// Reads all verification-related settings for the project (project-level SettingsBundle).
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ReadProjectVerificationSettings(IProject project, Language language = null)
        {
            var projectInfo = project.GetProjectInfo();
            var sdlprojFilePath = Uri.UnescapeDataString(projectInfo.Uri.LocalPath);

            Logger.Debug($"Reading verification settings from '{sdlprojFilePath}' ({(language == null ? "project-level" : language.ToString())}).");

            XDocument doc;
            try
            {
                doc = XDocument.Load(sdlprojFilePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to load project file '{sdlprojFilePath}'.");
                throw;
            }

            var result = new Dictionary<string, Dictionary<string, string>>();

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

            // Find the outer SettingsBundle with the matching Guid
            var outerSettingsBundle = doc
                .Descendants("SettingsBundle")
                .FirstOrDefault(sb => (string)sb.Attribute("Guid") == settingsBundleGuid);

            if (outerSettingsBundle == null)
                return result;

            // The actual groups are inside the nested SettingsBundle
            var innerSettingsBundle = outerSettingsBundle.Element("SettingsBundle");
            if (innerSettingsBundle == null)
                return result;

            var settingsGroups = innerSettingsBundle
                .Elements("SettingsGroup")
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

            // Add termbase name to SettingsTermVerifier group if available
            var termbaseName = doc
                .Descendants("TermbaseConfiguration")
                .Descendants("Termbases")
                .Elements("Name")
                .FirstOrDefault();

            if (termbaseName != null)
            {
                if (!result.ContainsKey(Constants.SettingsTermVerifier))
                    result[Constants.SettingsTermVerifier] = new Dictionary<string, string>();

                result[Constants.SettingsTermVerifier]["TermbaseName"] = termbaseName.Value.Trim();
            }

            return result;
        }
    }
}