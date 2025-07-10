using QATracker.Components.SettingsProvider.Components;
using QATracker.Components.SettingsProvider.Model;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Linq;
using QATracker.Components.SettingsProvider.Extension;

namespace QATracker.Components.SettingsProvider;

public class VerificationSettingsDataProvider
{
    private ProjectSettingsReader ProjectSettingsReader { get; } = new();

    public VerificationProviderSettings GetVerificationSettings(IProject project) =>
        new()
        {
            ProjectVerificationProviders = GetVerificationSettings(project, null),
            LanguageVerificationProviders = GetLanguageVerificationSettings(project)
        };

    private Dictionary<string, VerificationSettingsTreeNode> GetLanguageVerificationSettings(IProject project) =>
        project.GetProjectInfo().TargetLanguages.ToDictionary(language => language.DisplayName,
            language => GetVerificationSettings(project, language));

    private VerificationSettingsTreeNode GetVerificationSettings(IProject project, Language language)
    {
        var projectVerificationSettings = ProjectSettingsReader.ReadProjectVerificationSettings(project, language);

        var verifiersList = DefaultSettingsProvider.GetVerifiersList();


        var settingsXml = new VerificationSettingsTreeNode();
        foreach (var verifier in verifiersList)
        {
            var verifierSettings = DefaultSettingsProvider.GetDefaultSettingsForVerifier(verifier);

            if (projectVerificationSettings.ContainsKey(verifier))
            {
                if (projectVerificationSettings[verifier].Keys.Count > 0)
                {
                    foreach (var settingsCategory in projectVerificationSettings[verifier])
                    {
                        if (settingsCategory.Key == "Enabled")
                        {
                            if (bool.Parse(settingsCategory.Value)) continue;

                            verifierSettings.Children = null;
                            verifierSettings.Values = [new() { Name = "Enabled", Value = "False" }];
                            break;
                        }

                        verifierSettings.Set(settingsCategory.Key, settingsCategory.Value);
                    }
                }
            }

            settingsXml.Children.Add(verifierSettings);
        }

        settingsXml.Name = "Verification Settings";
        return settingsXml;
    }
}