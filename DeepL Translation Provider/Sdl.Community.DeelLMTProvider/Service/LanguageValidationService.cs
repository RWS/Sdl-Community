using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class LanguageValidationService : ILanguageValidationService
    {
        public async Task<LanguagePairValidationResult> ValidateAsync(
            LanguagePair languagePair,
            string apiKey,
            string baseUrl)
        {
            var (sourceCode, isSourceFallback, sourceFallbackMsg) = GetDeepLLanguageCode(languagePair.SourceCulture, isSourceLanguage: true);
            var (targetCode, isTargetFallback, targetFallbackMsg) = GetDeepLLanguageCode(languagePair.TargetCulture, isSourceLanguage: false);

            System.Diagnostics.Debug.WriteLine($"[DeepL Validation] Validating source: '{sourceCode}', target: '{targetCode}'");

            var result = new LanguagePairValidationResult();

            if (isSourceFallback && !string.IsNullOrEmpty(sourceFallbackMsg))
                result.Messages.Add($"ℹ️ {sourceFallbackMsg}");
            if (isTargetFallback && !string.IsNullOrEmpty(targetFallbackMsg))
                result.Messages.Add($"ℹ️ {targetFallbackMsg}");

            result.IsSourceLanguageSupported = await LanguageClientV3.IsLanguageSupportedAsync(
                sourceCode, "source", apiKey, baseUrl, "translate_text");
            result.IsTargetLanguageSupported = await LanguageClientV3.IsLanguageSupportedAsync(
                targetCode, "target", apiKey, baseUrl, "translate_text");

            if (!result.IsSourceLanguageSupported)
                result.Messages.Add($"Source language '{languagePair.SourceCulture}' ({sourceCode}) is not supported by DeepL API for text translation.");
            if (!result.IsTargetLanguageSupported)
                result.Messages.Add($"Target language '{languagePair.TargetCulture}' ({targetCode}) is not supported by DeepL API for text translation.");

            if (!result.IsSourceLanguageSupported || !result.IsTargetLanguageSupported)
                return result;

            var targetInfo = await LanguageClientV3.GetLanguageV3InfoAsync(targetCode, "translate_text", apiKey, baseUrl);

            result.SupportsFormality = targetInfo?.Features?.Contains("formality") == true;

            var sourceGlossaryInfo = await LanguageClientV3.GetLanguageV3InfoAsync(sourceCode, "glossary", apiKey, baseUrl);
            var targetGlossaryInfo = await LanguageClientV3.GetLanguageV3InfoAsync(targetCode, "glossary", apiKey, baseUrl);

            result.SupportsGlossaries = sourceGlossaryInfo?.UsableAsSource == true
                                     && targetGlossaryInfo?.UsableAsTarget == true;

            return result;
        }

        private static (string code, bool isFallback, string fallbackMessage) GetDeepLLanguageCode(
            Sdl.Core.Globalization.CultureCode cultureCode, bool isSourceLanguage)
        {
            var cultureName = cultureCode.Name.ToUpperInvariant();
            var regionNeutralName = cultureCode.RegionNeutralName.ToLowerInvariant();

            var result = cultureName switch
            {
                "EN-US" when !isSourceLanguage => ("en-US", false, (string)null),
                "EN-GB" when !isSourceLanguage => ("en-GB", false, null),
                "EN-US" or "EN-GB" when isSourceLanguage =>
                    ("en", true, $"English variant '{cultureName}' will use generic 'en' for source language (variants only supported as target)"),

                "PT-BR" when !isSourceLanguage => ("pt-BR", false, null),
                "PT-PT" when !isSourceLanguage => ("pt-PT", false, null),
                "PT-BR" or "PT-PT" when isSourceLanguage =>
                    ("pt", true, $"Portuguese variant '{cultureName}' will use generic 'pt' for source language (variants only supported as target)"),

                "ES-419" when !isSourceLanguage => ("es-419", false, null),
                "ES-419" when isSourceLanguage =>
                    ("es", true, "Latin American Spanish (es-419) will use generic Spanish (es) for source language"),

                "ZH-CN" or "ZH-HANS" when !isSourceLanguage => ("zh-Hans", false, null),
                "ZH-TW" or "ZH-HANT" when !isSourceLanguage => ("zh-Hant", false, null),
                "ZH-CN" or "ZH-HANS" or "ZH-TW" or "ZH-HANT" when isSourceLanguage =>
                    ("zh", true, $"Chinese variant '{cultureName}' will use generic 'zh' for source language (variants only supported as target)"),

                _ when cultureName != regionNeutralName.ToUpperInvariant() =>
                    (regionNeutralName, true, $"Regional variant '{cultureCode.Name}' will use generic '{regionNeutralName}' language code"),

                _ => (regionNeutralName, false, null)
            };

            System.Diagnostics.Debug.WriteLine($"[DeepL] Converting culture '{cultureCode.Name}' (regionNeutral: '{cultureCode.RegionNeutralName}') to DeepL code: '{result.Item1}' (fallback: {result.Item2}, isSource: {isSourceLanguage})");

            return result;
        }
    }
}

