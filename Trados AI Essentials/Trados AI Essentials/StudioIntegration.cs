using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Trados_AI_Essentials.Service;
using Trados_AI_Essentials.Studio;

namespace Trados_AI_Essentials;

public static class StudioIntegration
{
    private static SettingsService SettingsService => Dependencies.SettingsService;
    private static TranslationService TranslationService => Dependencies.TranslationService;

    public static bool EditTranslationProvider() => true;

    public static TranslationProvider GetTranslationProviderFromState(string state) => TranslationProvider.CreateFromState(state);

    public static TranslationProvider GetTranslationProviderFromUserSettings() => TranslationProvider.CreateFromSettings(SettingsService.GetSettingsFromUser());

    public static SearchResults[] Translate(TranslationUnit[] translationUnits, bool[] mask, string sourceLanguage,
        string targetLanguage) =>
        TranslationService.Translate(translationUnits, mask, sourceLanguage, targetLanguage);

    public static TranslationProviderLanguageDirection GetTranslationProviderLanguageDirection(
        LanguagePair languageDirection, TranslationProvider translationProvider) =>
        new()
        {
            SourceLanguage = languageDirection.SourceCulture,
            TargetLanguage = languageDirection.TargetCulture,
            TranslationProvider = translationProvider
        };
}