using QATracker.Components.SettingsProvider.Model;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QATracker.Components.SettingsProvider.Components;
using Sdl.Core.Settings;

namespace QATracker.Components.SettingsProvider;

public class VerificationSettingsDataProvider
{
    public VerificationProviderSettings GetVerificationSettings(IProject project)
    {
        return new VerificationProviderSettings
        {
            ProjectVerificationProviders = GetVerificationSettings(project, null),
            LanguageVerificationProviders = GetLanguageVerificationSettings(project)
        };
    }

    private Dictionary<string, List<dynamic>> GetLanguageVerificationSettings(IProject project)
    {
        var languageVerificationSettings = new Dictionary<string, List<dynamic>>();
        foreach (var language in project.GetProjectInfo().TargetLanguages)
            languageVerificationSettings.Add(language.DisplayName, GetVerificationSettings(project, language));
        return languageVerificationSettings;
    }

    private List<dynamic> GetVerificationSettings(IProject project, Language language)
    {
        var projectInfo = project.GetProjectInfo();
        var settingsReader =
            new SettingsReader(Path.Combine(projectInfo.LocalProjectFolder, $"{projectInfo.Name}.sdlproj"));

        var verifiers = language == null
            ? settingsReader.ReadProjectVerificationSettings()
            : settingsReader.ReadProjectVerificationSettings(language);

        var settingsXml = new VerificationSettingsTreeNode();
        foreach (var settings in verifiers)
        {
//            var category = CategoryMap.GetCategoryName(settings.Key);

            var verifierSettings = DefaultSettingsProvider.GetDefaultSettingsForVerifier(settings.Key);

            if (verifiers[settings.Key].Keys.Count > 0)
            {
                foreach (var settingsCategory in verifiers[settings.Key])
                {
                    
                }
            }

            

        }

        return null;
        //var projectSettings = language is null ? project.GetSettings() : project.GetSettings(language);
        //var verificationSettingsGroups = projectSettings.GetSettingsGroupIds().Where(id => id.ToLower().Contains("verif")).Select(id => projectSettings.GetSettingsGroup(id));
        //return verificationSettingsGroups.Select(settingsGroup => VerificationSettingsBuilder.Create(settingsGroup)).ToList();
    }
}