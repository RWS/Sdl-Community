using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class EdgeFeedbackItem
	{
		[JsonProperty("sourceText")]
		public string SourceText { get; set; }

		[JsonProperty("languagePairId")]
		public string LanguagePairId { get; set; }

		[JsonProperty("machineTranslation")]
		public string MachineTranslation { get; set; }

		[JsonProperty("comment")]
		public string Comment { get; set; }

		[JsonProperty("suggestedTranslation")]
		public string SuggestedTranslation { get; set; }
	}
}