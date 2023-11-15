namespace LanguageWeaverProvider
{
	public class Constants
	{
		// Plugin Name
		public const string PluginShortName = "Language Weaver";
		public const string PluginName = "Language Weaver Provider";
		public const string PluginNameCloud = "Language Weaver Cloud";
		public const string PluginNameEdge = "Language Weaver Edge";


		public const string BaseTranslationScheme = "languageweaver";
		public const string CloudScheme = "languageweavercloud";
		public const string CloudFullScheme = "languageweavercloud:///";
		public const string EdgeScheme = "languageweaveredge";
		public const string EdgeFullScheme = "languageweaveredge:///";

		// Provider
		public const string Provider_TranslationProviderFactory = "LanguageWeaverProvider_Factory";
		public const string Provider_TranslationProviderWinFormsUi = "LanguageWeaverProvider_WinFormsUI";

		// Services
		public const string CloudService = "Cloud";
		public const string EdgeService = "Edge";

		// LW Portal
		public const string LanguageWeaverEUPortal = "https://portal.languageweaver.com/login";

		// Segment metadata
		public const string SegmentMetadata_QE = "quality_estimation";
		public const string SegmentMetadata_ShortModelName = "model";
		public const string SegmentMetadata_LongModelName = "nmt_model";
		public const string SegmentMetadata_Translation = "nmt_translation";

		// Feedback - translation errors
		public const string WordsOmission = "Words omission";
		public const string WordsAddition = "Words addition";
		public const string WordChoice = "Word choice";
		public const string Unintelligible = "Unintelligible";
		public const string Grammar = "Grammar";
		public const string Spelling = "Spelling";
		public const string Punctuation = "Punctuation";
		public const string Capitalization = "Capitalization";
		public const string CapitalizationPunctuation = "Capitalization, punctuation";

	}
}