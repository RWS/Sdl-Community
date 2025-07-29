using CaptureQARuleState.Components.SettingsProvider.Verifiers;
using CaptureQARuleState.Components.SettingsProvider.Verifiers.Interface;

namespace CaptureQARuleState.Components.SettingsProvider.Components
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