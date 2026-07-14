using System;
using System.Collections.Generic;
using System.Linq;
using VerifyFilesAuditReport.Components.SettingsProvider.Components;
using VerifyFilesAuditReport.Components.SettingsProvider.Verifiers;
using VerifyFilesAuditReport.Components.SettingsProvider.Verifiers.Interface;

namespace VerifyFilesAuditReport.Components.SettingsProvider;

/// <summary>
/// Single registry of every verifier the report can describe.
/// Supporting a new verifier means adding one registration here.
/// </summary>
public static class VerifierCatalog
{
    private sealed class Registration(string settingsId, Func<IVerificationSettings> createSettings, string pluginName = null)
    {
        public string SettingsId { get; } = settingsId;
        public Func<IVerificationSettings> CreateSettings { get; } = createSettings;

        /// <summary>Plugin name for third-party verifiers; null for built-in ones, which are always active.</summary>
        public string PluginName { get; } = pluginName;
    }

    private static readonly List<Registration> Registrations =
    [
        new(Constants.QaVerificationSettings, () => new QaVerificationSettings()),
        new(Constants.SettingsTagVerifier, () => new TagVerificationSettings()),
        new(Constants.SettingsTermVerifier, () => new TermVerificationSettings()),
        new(Constants.NumberVerifierSettings, () => new NumberVerificationSettings(), Constants.NumberVerifierName)
    ];

    public static List<string> GetActiveVerifierIds()
    {
        var installedPlugins = PluginManagerWrapper.GetInstalledPluginNames();

        return Registrations
            .Where(r => r.PluginName == null || installedPlugins.Contains(r.PluginName))
            .Select(r => r.SettingsId)
            .ToList();
    }

    public static IVerificationSettings CreateSettings(string verifierId)
    {
        var registration = Registrations.FirstOrDefault(r => r.SettingsId == verifierId)
            ?? throw new NotSupportedException($"No settings reader is registered for verifier '{verifierId}'.");

        return registration.CreateSettings();
    }
}
