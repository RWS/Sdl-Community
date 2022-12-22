using System;

namespace GoogleTranslatorProvider
{
	public static class Constants
	{
		public const string GoogleNaming_FullName = "Google Cloud Translation Provider";
		public const string GoogleNaming_ShortName = "Google Cloud TP";
		public const string GoogleVersion_V2_FullName = "V2 Basic Translation";
		public const string GoogleVersion_V2_ShortName = "V2";
		public const string GoogleVersion_V3_FullName = "V3 Advanced Translation";
		public const string GoogleVersion_V3_ShortName = "V3";

		public static readonly string DefaultDownloadableLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Trados AppStore\\Google Cloud TP\\";
		public const string DefaultDownloadedJsonFileName = "\\downloadedProject.json";

		public const string GoogleTranslatorString = "Google Translate";
		public const string GooglePluginName = "Google Translator Provider";

		public const string GoogleTranslationScheme = "googletranslationprovider";
		public const string GoogleTranslationFullScheme = "googletranslationprovider:///";
		public const string GoogleApiEnvironmentVariableName = "GOOGLE_APPLICATION_CREDENTIALS";
		public const string LanguagesUri = "https://www.googleapis.com/language/translate/v2/languages";
		public const string TranslationUri = "https://translation.googleapis.com/language/translate/v2";

		// Documentation
		public const string V3Documentation = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6575/v3-advanced-translation";
		public const string FullDocumentation = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6547/google-cloud-translation-provider";

		// Tell Me
		public const string TellMe_HelpUrl = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6547/google-translator-provider";
		public const string TellMe_StoreUrl = "https://appstore.rws.com/plugin/174/";
		public const string TellMe_CommunityForumUrl = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/";
	}
}