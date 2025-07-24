using QATracker.Components.SettingsProvider.Verifiers;
using QATracker.Components.SettingsProvider.Verifiers.Interface;

namespace QATracker.Components.SettingsProvider.Components
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