using System;
using System.IO;

namespace GoogleCloudTranslationProvider;

public static class Constants
{
    // Database file
    public static string DatabaseLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Trados AppStore\\Language Mapping Provider";
    public static string DatabaseFilePath = Path.Combine(DatabaseLocation, "{0}data.sqlite3");
    public const string Database_PluginName_V2 = "googlecloudbasic";
    public const string Database_PluginName_V3 = "googlecloudadvanced";

    // Plugin naming
    public const string GoogleTranslatorString = "Google Translate";
    public const string GoogleNaming_FullName = "Google Cloud Translation Provider";
    public const string GoogleNaming_ShortName = "Google Cloud TP";
    public const string GoogleVersion_V2_FullName = "V2 Basic Translation";
    public const string GoogleVersion_V2_ShortName = "V2";
    public const string GoogleVersion_V3_FullName = "V3 Advanced Translation";
    public const string GoogleVersion_V3_ShortName = "V3";

    // Paths and files
    private const string PluginInAppData = "\\Trados AppStore\\Google Cloud TP";
    public const string DefaultDownloadedJsonFileName = "downloadedProject.json";
    public static readonly string AppDataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{PluginInAppData}";


    // Plugin connection
    public const string GoogleTranslationScheme = "googletranslationprovider";
    public const string GoogleTranslationFullScheme = "googletranslationprovider:///";
    public const string GoogleApiEnvironmentVariableName = "GOOGLE_APPLICATION_CREDENTIALS";

    // Provider
    public const string Provider_TranslationProviderFactory = "GoogleCloudTranslationProvider_Factory";
    public const string Provider_TranslationProviderWinFormsUi = "GoogleCloudTranslationProvider_WinFormsUI";

    // Google URIs
    public const string LanguagesUri = "https://www.googleapis.com/language/translate/v2/languages";
    public const string TranslationUri = "https://translation.googleapis.com/language/translate/v2";
    public const string GlossariesUri = "https://console.cloud.google.com/storage";
    public const string CustomModelsUri = "https://console.cloud.google.com/translation/datasets";
    public const string ProjectUri = "https://console.cloud.google.com/home/dashboard";
    public const string ResourceManagerUri = "https://console.cloud.google.com/cloud-resource-manager";
    public const string LanguageCodesUri = "https://cloud.google.com/translate/docs/languages";


    // Documentation
    public const string V3Documentation = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6575/v3-advanced-translation";
    public const string FullDocumentation = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6547/google-cloud-translation-provider";

    // Tell Me
    public const string TellMe_StoreUrl = "https://appstore.rws.com/plugin/174/";
    public const string TellMe_CommunityForumUrl = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/";
    public const string TellMe_HelpUrl = "https://appstore.rws.com/Plugin/174?tab=documentation";
}