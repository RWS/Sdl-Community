namespace LanguageWeaverProvider
{
	public class Constants
	{
		public const string PluginShortName = "Language Weaver";
		public const string PluginName = "Language Weaver Provider";
		public const string TranslationScheme = "languageweaver";
		public const string TranslationFullScheme = "languageweaver:///";

		public const string CloudFullScheme = "languageweavercloud:///";
		public const string EdgeFullScheme = "languageweaveredge:///";

		// Provider
		public const string Provider_TranslationProviderFactory = "LanguageWeaverProvider_Factory";
		public const string Provider_TranslationProviderWinFormsUi = "LanguageWeaverProvider_WinFormsUI";

		// X
		public const string CloudService = "Cloud";
		public const string EdgeService = "Edge";

		// LW Portal
		public const string LanguageWeaverEUPortal = "https://portal.languageweaver.com/login";

		// Segment metadata
		public const string SegmentMetadata_QE = "quality_estimation";
		public const string SegmentMetadata_ShortModelName = "model";
		public const string SegmentMetadata_LongModelName = "nmt_model";
		public const string SegmentMetadata_Translation = "nmt_translation";
	}
}