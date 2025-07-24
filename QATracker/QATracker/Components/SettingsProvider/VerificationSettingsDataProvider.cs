using QATracker.Components.SettingsProvider.Components;
using QATracker.Components.SettingsProvider.Model;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Linq;

namespace QATracker.Components.SettingsProvider;

public class VerificationSettingsDataProvider
{
    private static List<string> DefaultVerifiers { get; } =
    [
        Constants.QaVerificationSettings,
        Constants.SettingsTagVerifier,
        Constants.SettingsTermVerifier
    ];

    private ProjectSettingsReader ProjectSettingsReader { get; } = new();

    public static List<string> GetVerifiersList() =>
            DefaultVerifiers.Concat(PluginManagerWrapper.GetInstalledThirdPartyVerifiers()).ToList();

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
        var settingsXml = new VerificationSettingsTreeNode();
        var projectVerificationSettings = ProjectSettingsReader.ReadProjectVerificationSettings(project, language);
        var verifiersList = GetVerifiersList();
        foreach (var verifier in verifiersList)
        {
            var verifierSettingsObject = CategoryMap.CreateVerificationSettings(verifier);
            verifierSettingsObject.LoadSettings(projectVerificationSettings?[verifier]);
            settingsXml.Values.Add(verifierSettingsObject.ToSettingsValue());
        }

        settingsXml.Name = "Verification Settings";
        return settingsXml;
    }
}