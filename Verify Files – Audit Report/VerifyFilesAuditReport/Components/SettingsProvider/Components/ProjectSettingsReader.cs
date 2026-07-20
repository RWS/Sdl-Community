using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NLog;
using Sdl.ProjectAutomation.Core;
using VerifyFilesAuditReport.Logging;

namespace VerifyFilesAuditReport.Components.SettingsProvider.Components
{
    public class ProjectSettingsReader
    {
        private static readonly Logger Logger = Log.GetLogger(typeof(ProjectSettingsReader).FullName);

        /// <summary>
        /// Loads the .sdlproj file once; the returned document can be passed to
        /// <see cref="ReadProjectVerificationSettings"/> for every settings bundle in it.
        /// </summary>
        public XDocument LoadProjectDocument(IProject project)
        {
            var projectInfo = project.GetProjectInfo();
            var sdlprojFilePath = Uri.UnescapeDataString(projectInfo.Uri.LocalPath);

            Logger.Debug($"Loading project file '{sdlprojFilePath}'.");

            try
            {
                return XDocument.Load(sdlprojFilePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to load project file '{sdlprojFilePath}'.");
                throw;
            }
        }

        /// <summary>
        /// Reads all verification-related settings from the given project document (project-level bundle).
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ReadProjectVerificationSettings(
            XDocument projectDocument /*, string targetLanguageCode = null*/)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();

            var settingsBundleGuid = GetSettingsBundleGuid(projectDocument /*, targetLanguageCode*/);
            if (string.IsNullOrEmpty(settingsBundleGuid))
                return result;

            // Find the outer SettingsBundle with the matching Guid
            var outerSettingsBundle = projectDocument
                .Descendants("SettingsBundle")
                .FirstOrDefault(sb => (string)sb.Attribute("Guid") == settingsBundleGuid);

            // The actual groups are inside the nested SettingsBundle
            var innerSettingsBundle = outerSettingsBundle?.Element("SettingsBundle");
            if (innerSettingsBundle == null)
                return result;

            var settingsGroups = innerSettingsBundle
                .Elements("SettingsGroup")
                .Where(g => g.Attribute("Id") != null &&
                            g.Attribute("Id").Value.ToLower().Contains("verif"));

            foreach (var group in settingsGroups)
                result[group.Attribute("Id").Value] = ReadSettingsGroup(group);

            AddTermbaseName(projectDocument, result);

            return result;
        }

        private static string GetSettingsBundleGuid(XDocument projectDocument /*, string targetLanguageCode*/)
        {
            //if (targetLanguageCode == null)
            return projectDocument.Root?.Attribute("SettingsBundleGuid")?.Value;

            //var languageDirection = projectDocument.Descendants("LanguageDirection")
            //    .FirstOrDefault(ld => (string)ld.Attribute("TargetLanguageCode") == targetLanguageCode);

            //return languageDirection?.Attribute("SettingsBundleGuid")?.Value;
        }

        private static Dictionary<string, string> ReadSettingsGroup(XElement group)
        {
            var settings = new Dictionary<string, string>();

            foreach (var setting in group.Elements("Setting"))
            {
                var settingId = setting.Attribute("Id")?.Value ?? "";

                // Settings with child elements are kept as their XML string
                settings[settingId] = setting.HasElements
                    ? setting.ToString(SaveOptions.DisableFormatting)
                    : setting.Value?.Trim() ?? "";
            }

            return settings;
        }

        private static void AddTermbaseName(XDocument projectDocument,
            Dictionary<string, Dictionary<string, string>> result)
        {
            var termbaseName = projectDocument
                .Descendants("TermbaseConfiguration")
                .Descendants("Termbases")
                .Elements("Name")
                .FirstOrDefault();

            if (termbaseName == null)
                return;

            if (!result.ContainsKey(Constants.SettingsTermVerifier))
                result[Constants.SettingsTermVerifier] = new Dictionary<string, string>();

            result[Constants.SettingsTermVerifier]["TermbaseName"] = termbaseName.Value.Trim();
        }
    }
}
