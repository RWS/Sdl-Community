using NLog;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class LanguageValidationService
    {
        private static readonly Logger Logger = Log.GetLogger(nameof(LanguageValidationService));

        public static (string code, bool isFallback, string fallbackMessage) GetDeepLLanguageCode(
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

                "ZH-CN" or "ZH-SG" or "ZH-HANS" or "ZH-HANS-HK" or "ZH-HANS-MO" when !isSourceLanguage => ("zh-Hans", false, null),
                "ZH-TW" or "ZH-HK" or "ZH-MO" or "ZH-HANT" when !isSourceLanguage => ("zh-Hant", false, null),
                "ZH-CN" or "ZH-SG" or "ZH-HANS" or "ZH-HANS-HK" or "ZH-HANS-MO"
                or "ZH-TW" or "ZH-HK" or "ZH-MO" or "ZH-HANT" when isSourceLanguage =>
                    ("zh", true, $"Chinese variant '{cultureName}' will use generic 'zh' for source language (variants only supported as target)"),

                _ when cultureName != regionNeutralName.ToUpperInvariant() =>
                    (regionNeutralName, true, $"Regional variant '{cultureCode.Name}' will use generic '{regionNeutralName}' language code"),

                _ => (regionNeutralName, false, null)
            };

            Logger.Info($"Converting culture '{cultureCode.Name}' (regionNeutral: '{cultureCode.RegionNeutralName}') to DeepL code: '{result.Item1}' (fallback: {result.Item2}, isSource: {isSourceLanguage})");

            return result;
        }
    }
}