using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using VerifyFilesAuditReport.Components.SettingsProvider.Components;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport.Components.SettingsProvider;

public class VerificationSettingsDataProvider
{
    private ProjectSettingsReader ProjectSettingsReader { get; } = new();

    public VerificationProviderSettings GetVerificationSettings(IProject project)
    {
        // Loaded and resolved once; reused for the project-level and every per-language extraction.
        var projectDocument = ProjectSettingsReader.LoadProjectDocument(project);
        var activeVerifierIds = VerifierCatalog.GetActiveVerifierIds();

        return new VerificationProviderSettings
        {
            ProjectVerificationProviders = GetVerificationSettings(projectDocument, null, activeVerifierIds),
            LanguageVerificationProviders = project.GetProjectInfo().TargetLanguages.ToDictionary(
                language => language.DisplayName,
                language => GetVerificationSettings(projectDocument, language, activeVerifierIds))
        };
    }

    private VerificationSettingsTreeNode GetVerificationSettings(XDocument projectDocument, Language language,
        List<string> activeVerifierIds)
    {
        var projectVerificationSettings =
            ProjectSettingsReader.ReadProjectVerificationSettings(projectDocument, language?.ToString());

        var settingsRoot = new VerificationSettingsTreeNode { Name = Constants.VerificationSettings };
        foreach (var verifierId in activeVerifierIds)
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
