using LanguageMappingProvider;
using Sdl.Core.Globalization;

namespace GoogleCloudTranslationProvider.Helpers;

public static class LanguageCodeConverter
{
    public static string GetLanguageCode(this CultureCode cultureCode, ApiVersion targetVersion)
    {
        var targetDatabase = targetVersion == ApiVersion.V2
                           ? Constants.Database_PluginName_V2
                           : Constants.Database_PluginName_V3;

        var database = new LanguageMappingDatabase(targetDatabase, null);
        return database.TryGetLanguage(cultureCode, out var languageMapping)
             ? languageMapping.LanguageCode
             : string.Empty;
    }
}