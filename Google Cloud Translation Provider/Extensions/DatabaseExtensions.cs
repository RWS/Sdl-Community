using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GoogleCloudTranslationProvider.GoogleAPI;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using LanguageMappingProvider;
using NLog;
using Sdl.Core.Globalization.LanguageRegistry;

namespace GoogleCloudTranslationProvider.Extensions;

public static class DatabaseExtensions
{
    private static LanguageMappingDatabase _v3DatabaseCache;
    private static LanguageMappingDatabase _v2DatabaseCache;

    public static List<LanguageMapping> GetGoogleDefaultMapping(ITranslationOptions translationOptions)
    {
        var databaseFilePath = GetDatabaseFilePath(translationOptions.SelectedGoogleVersion);
        if (!File.Exists(databaseFilePath))
        {
            return null;
        }

        return translationOptions.SelectedGoogleVersion switch
        {
            ApiVersion.V2 => CreateDatabase(translationOptions, CreateV2Database),
            ApiVersion.V3 => CreateDatabase(translationOptions, CreateV3Database),
            _ => null
        };
    }

    public static LanguageMappingDatabase CreateDatabase(ITranslationOptions translationOptions)
    {
        var isV3 = translationOptions.SelectedGoogleVersion == ApiVersion.V3;

        // ✅ Return cached instance if already built
        if (isV3 && _v3DatabaseCache is not null) return _v3DatabaseCache;
        if (!isV3 && _v2DatabaseCache is not null) return _v2DatabaseCache;

        var languageMappings = CreateLanguageMappings(translationOptions);

        // Deduplicate before passing to LanguageMappingDatabase
        var deduplicated = languageMappings
            .GroupBy(x => new { x.Name, x.Region })
            .Select(g => g.OrderByDescending(x => !string.IsNullOrEmpty(x.Region))
                .First())
            .ToList();

        var databaseName = isV3
            ? Constants.Database_PluginName_V3
            : Constants.Database_PluginName_V2;

        var database = new LanguageMappingDatabase(databaseName, deduplicated);

        // Cache it
        if (isV3) _v3DatabaseCache = database;
        else _v2DatabaseCache = database;

        return database;
    }

    private static List<LanguageMapping> CreateDatabase(ITranslationOptions translationOptions, Func<ITranslationOptions, List<LanguageMapping>> createDatabaseFunc)
    {
        var languageMappings = createDatabaseFunc(translationOptions);
        return languageMappings;
    }

    private static List<LanguageMapping> CreateV2Database(ITranslationOptions translationOptions)
    {
        var v2Languages = translationOptions.V2SupportedLanguages;

        var mainMappings = v2Languages
            .Where(language => IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
            .Select(language => ParseLanguageMapping(language.LanguageName, language.LanguageCode))
            .ToList();

        var chineseMappings = CreateChineseMapping();

        // ✅ Same deduplication strategy
        return mainMappings
            .Union(chineseMappings)
            .GroupBy(x => new { x.Name, x.Region })
            .Select(g => g.OrderByDescending(x => !string.IsNullOrEmpty(x.Region))
                .First())
            .ToList();
    }

    private static List<LanguageMapping> CreateV3Database(ITranslationOptions translationOptions)
    {
        if (translationOptions.V3SupportedLanguages is null
            || !translationOptions.V3SupportedLanguages.Any())
        {
            var v3Connector = new V3Connector(translationOptions);
            translationOptions.V3SupportedLanguages = v3Connector.GetLanguages();
        }

        var v3Languages = translationOptions.V3SupportedLanguages;

        var mainMappings = v3Languages
            .Where(language => IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
            .Select(language => new LanguageMapping
            {
                Name = language.CultureInfo.EnglishName,
                LanguageCode = language.GoogleLanguageCode
            })
            .ToList();

        var chineseMappings = CreateChineseMapping();

        // ✅ Merge with explicit deduplication — Chinese entries win over any 
        // generic entries since they have proper Region set
        return mainMappings
            .Union(chineseMappings)
            .GroupBy(x => new { x.Name, x.Region })
            .Select(g => g.OrderByDescending(x => !string.IsNullOrEmpty(x.Region))
                .First())  // prefer entries with Region set
            .ToList();
    }

    private static List<LanguageMapping> CreateChineseMapping()
    {
        var tradosChinese = LanguageRegistryApi.Instance
            .GetAllLanguages()
            .Where(x => x.EnglishName.StartsWith("Chinese"))
            .ToList();

        var chineseLanguageMapping = new List<LanguageMapping>();
        foreach (var language in tradosChinese)
        {
            var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
            var match = regex.Match(language.EnglishName);

            var languageName = match.Groups[1].Value;
            var languageRegion = match.Groups[2].Success ? match.Groups[2].Value : null;

            if (chineseLanguageMapping.Any(x => x.Name == languageName && x.Region == languageRegion)
                || languageRegion is null)
            {
                continue;
            }

            var languageCode = languageRegion.StartsWith("Simplified") ? "zh-CN"
                : languageRegion.StartsWith("Traditional") ? "zh-TW"
                : "zh";

            chineseLanguageMapping.Add(new LanguageMapping
            {
                Name = languageName,
                Region = languageRegion,
                LanguageCode = languageCode
            });
        }

        return chineseLanguageMapping;
    }

    private static List<LanguageMapping> CreateLanguageMappings(ITranslationOptions translationOptions)
    {

        return translationOptions.SelectedGoogleVersion switch
        {
            ApiVersion.V2 => CreateV2Database(translationOptions),
            ApiVersion.V3 => CreateV3Database(translationOptions),
            _ => new List<LanguageMapping>()
        };
    }

    private static bool IsValidLanguage(ApiVersion apiVersion, object targetLanguage)
    {
        return apiVersion switch
        {
            ApiVersion.V2 when targetLanguage is V2LanguageModel v2Language =>
                !(v2Language.LanguageCode == "zh"
                  || v2Language.LanguageCode == "iw"
                  || v2Language.LanguageCode == "jw"
                  || v2Language.LanguageName.StartsWith("Chinese")),

            ApiVersion.V3 when targetLanguage is V3LanguageModel v3Language =>
                // ✅ Use EnglishName - locale-independent
                !(v3Language.CultureInfo.EnglishName.StartsWith("Unknown")
                  || v3Language.CultureInfo.EnglishName.StartsWith("Chinese")
                  || v3Language.GoogleLanguageCode == "ckb"),

            _ => false
        };
    }

    private static LanguageMapping ParseLanguageMapping(string languageName, string languageCode)
    {
        var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
        var match = regex.Match(languageName);

        return new LanguageMapping
        {
            Name = match.Groups[1].Value,
            Region = match.Groups[2].Success ? match.Groups[2].Value : null,
            LanguageCode = languageCode
        };
    }

    private static string GetDatabaseFilePath(ApiVersion apiVersion)
    {
        var pluginName = apiVersion == ApiVersion.V2
            ? Constants.Database_PluginName_V2
            : Constants.Database_PluginName_V3;
        return string.Format(Constants.DatabaseFilePath, pluginName);
    }
}