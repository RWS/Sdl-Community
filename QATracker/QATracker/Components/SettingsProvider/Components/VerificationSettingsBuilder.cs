//using QATracker.Components.SettingsProvider.QA_Checker;
//using Sdl.Core.Values;

//namespace QATracker.Components.SettingsProvider.Components;

//public static class VerificationSettingsBuilder
//{
//    private const string QaVerificationSettings = "QAVerificationSettings";
//    private const string SettingsTagVerifier = "SettingsTagVerifier";
//    private const string SettingsTermVerifier = "SettingsTermVerifier";

//    public static dynamic Create(string verifierName) =>
//        verifierName switch
//        {
//            QaVerificationSettings => new QaCheckerSettings(verifierName),
//            SettingsTagVerifier => (dynamic)null,
//            SettingsTermVerifier => (dynamic)null,
//            _ => null
//        };
//}