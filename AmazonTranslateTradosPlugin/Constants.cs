using System;
using System.IO;
using System.Web.UI.WebControls;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
    public static class Constants
    {
        public const string PluginName = "Amazon Translate MT provider";
        public const string BaseTranslationScheme = "amazontranslateprovider";


        public readonly static string JsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Trados AppStore\AmazonTranslationProvider");
        public readonly static string JsonFileName = "AmazonProviderSettings.json";

        // Tell Me
        public static readonly string TellMe_Provider_Name = $"{PluginName} Tell Me";
        public static readonly string TellMe_Forum_Name = $"RWS Community AppStore Forum ";
        public static readonly string TellMe_Documentation_Name = $"{PluginName} Documentation";
        public static readonly string TellMe_SourceCode_Name = $"{PluginName} Source Code";
        public static readonly string TellMe_Settings_Name = $"{PluginName} Settings";
        public static readonly string TellMe_Documentation_Url = "https://appstore.rws.com/Plugin/18?tab=documentation";
        public static readonly string TellMe_Forum_Url = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f";
        public static readonly string TellMe_SourceCode_Url = "https://github.com/RWS/Sdl-Community/tree/master/AmazonTranslateTradosPlugin";
        public static readonly string TellMe_ThirdPartyRedirect_Name = $"Official Amazon Translate AWS Website";
        public static readonly string TellMe_ThirdPartyRedirect_Url = "https://aws.amazon.com/translate/";

    }
}