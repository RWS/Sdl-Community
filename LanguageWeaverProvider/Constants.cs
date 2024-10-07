namespace LanguageWeaverProvider
{
	public class Constants
	{
        public const string METADATA_EVALUATED_AT_FORMAT = "M/d/yyyy HH:mm:ss";
        public const string METADATA_EVALUATED_AT_PREFIX = "tqe_evaluated_at_";
        public const string METADATA_DESCRIPTION_PREFIX = "tqe_description_";
        public const string METADATA_DESCRIPTION = "This was evaluated using the TQE system {0} with the {1} model";
        public const string METADATA_MODEL_PREFIX = "tqe_model_";
        public const string METADATA_SCORE_PREFIX = "tqe_score_";
        public const string METADATA_SYSTEM_PREFIX = "tqe_system_";
        public const string METADATA_SYSTEM_NAME = "lw-qe";

        public const string TraceAppKey = "Trace-App";
		public const string TraceAppValue = "Trados Studio";
		public const string TraceAppVersionKey = "Trace-App-Version";

		// Plugin Name
		public const string PluginShortName = "Language Weaver";
		public const string PluginName = "Language Weaver Provider";
		public const string PluginNameCloud = "Language Weaver Cloud";
		public const string PluginNameEdge = "Language Weaver Edge";

		// Scheme
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

		// Database
		public const string DatabaseName = "languageweaver";

		// LW Portal
		public const string LanguageWeaverEUPortal = "https://portal.languageweaver.com/login";
		public const string LanguageWeaverUSPortal = "https://us.portal.languageweaver.com/login";

		// LW Cloud API Region
		public static string CloudEUUrl = "https://api.languageweaver.com/";
		public static string CloudUSUrl = "https://us.api.languageweaver.com/";

		// Segment metadata
		public const string SegmentMetadata_QE = "quality_estimation";
		public const string SegmentMetadata_ShortModelName = "model";
		public const string SegmentMetadata_LongModelName = "nmt_model";
		public const string SegmentMetadata_Translation = "nmt_translation";
		public const string SegmentMetadata_Feedback = "autosend_feedback";

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

		// Window titles
		public const string PairMapping_MainWindow = "Language Weaver Provider";
		public const string PairMapping_SettingsWindow = "Language Weaver Provider - Settings";

		// Tell Me
		public static readonly string TellMe_Provider_Name = $"{PluginName} Tell Me";
		public static readonly string TellMe_Forum_Name = $"RWS Community AppStore Forum ";
		public static readonly string TellMe_Documentation_Name = $"{PluginName} Documentation";
		public static readonly string TellMe_SourceCode_Name = $"{PluginName} Source Code";
		public static readonly string TellMe_Settings_Name = $"{PluginName} Settings";
		public static readonly string TellMe_Documentation_Url = "https://appstore.rws.com/Plugin/240?tab=documentation";
		public static readonly string TellMe_Forum_Url = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f";
		public static readonly string TellMe_SourceCode_Url = "https://github.com/RWS/Sdl-Community/tree/master/LanguageWeaverProvider";
	}
}