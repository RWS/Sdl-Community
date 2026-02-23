using LanguageMappingProvider;
using NLog;
using Sdl.Core.Globalization;
using System;
using System.IO;

namespace GoogleCloudTranslationProvider.Helpers;

public static class LanguageCodeConverter
{
    public static string GetLanguageCode(this CultureCode cultureCode, ApiVersion targetVersion)
    {
        var targetDatabase = targetVersion == ApiVersion.V2
            ? Constants.Database_PluginName_V2
            : Constants.Database_PluginName_V3;

        var dbPath = string.Format(Constants.DatabaseFilePath, targetDatabase);

        // ✅ Never attempt to create DB here — just return empty if not ready yet
        // DB creation is exclusively handled by DatabaseExtensions.CreateDatabase
        if (!File.Exists(dbPath))
        {
            LogManager.GetCurrentClassLogger()
                .Warn($"GetLanguageCode: Database not found at '{dbPath}'. " +
                      $"Cannot resolve code for '{cultureCode}' ({targetVersion}). " +
                      $"DB will be created by CreateDatabase call.");
            return string.Empty;
        }

        try
        {
            // Pass null safely — DB file exists so it won't try to create/init
            var database = new LanguageMappingDatabase(targetDatabase, null);
            return database.TryGetLanguage(cultureCode, out var languageMapping)
                ? languageMapping.LanguageCode
                : string.Empty;
        }
        catch (Exception ex)
        {
            LogManager.GetCurrentClassLogger()
                .Error($"GetLanguageCode: Failed to read database for " +
                       $"'{cultureCode}' ({targetVersion}): {ex.Message}");
            return string.Empty;
        }
    }
}