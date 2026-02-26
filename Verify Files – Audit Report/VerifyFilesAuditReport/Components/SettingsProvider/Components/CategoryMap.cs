using VerifyFilesAuditReport.Components.SettingsProvider.Verifiers;
using VerifyFilesAuditReport.Components.SettingsProvider.Verifiers.Interface;

namespace VerifyFilesAuditReport.Components.SettingsProvider.Components
{
    public static class CategoryMap
    {
        public static IVerificationSettings CreateVerificationSettings(string verifierId) =>
            verifierId switch
            {
                Constants.QaVerificationSettings => new QaVerificationSettings(),
                Constants.SettingsTagVerifier => new TagVerificationSettings(),
                Constants.SettingsTermVerifier => new TermVerificationSettings(),

                //3rd party verifiers
                Constants.NumberVerifierSettings => new NumberVerificationSettings(),
            };
    }
}