using System.Collections.Generic;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using VerifyFilesAuditReport.Components.SettingsProvider.Components;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport.Components.SettingsProvider;

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

        var settingsRoot = new VerificationSettingsTreeNode { Name = Constants.VerificationSettings };
        foreach (var verifierId in VerifierCatalog.GetActiveVerifierIds())
        {
            var verifierSettings = VerifierCatalog.CreateSettings(verifierId);

            verifierSettings.LoadSettings(projectVerificationSettings.TryGetValue(verifierId, out var settings)
                ? settings
                : new Dictionary<string, string>());

            settingsRoot.Values.Add(verifierSettings.ToSettingsValue());
        }

        return settingsRoot;
    }
}
