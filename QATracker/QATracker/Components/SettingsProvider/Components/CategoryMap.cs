using QATracker.Components.SettingsProvider.Verifiers;
using QATracker.Components.SettingsProvider.Verifiers.Interface;
using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Components
{
    public static class CategoryMap
    {
        private static Dictionary<string, string> VerifierNamesMap { get; set; } = new()
        {
            { Constants.NumberVerifierSettings, "Trados Number Verifier" }
        };

        private static Dictionary<string, IVerificationSettings> Verifiers { get; } = new()
        {
            [Constants.QaVerificationSettings] = new QaVerificationSettings(),
            [Constants.SettingsTagVerifier] = new TagVerificationSettings(),
            [Constants.SettingsTermVerifier] = new TermVerificationSettings()
        };

        public static IVerificationSettings CreateVerificationSettings(string verifierId) => Verifiers[verifierId];
    }
}