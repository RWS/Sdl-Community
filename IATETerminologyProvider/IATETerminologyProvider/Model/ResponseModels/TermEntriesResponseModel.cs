using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class TermEntriesResponseModel
	{
		public MetaResponseModel Meta { get; set; }
		public string Code { get; set; }
		public int Id { get; set; }
		public int Type { get; set; }

		[JsonProperty("term_value")]
		public string TermValue { get; set; }

		[JsonProperty("highlighted_term_value")]
		public string HighlightedTermValue { get; set; }

		[JsonProperty("term_references")]
		public TermReferencesResponseModel TermReferences { get; set; }
		public MetadataResponseModel Metadata { get; set; }

		[JsonProperty("initial_source_id")]
		public string InitialSourceId { get; set; }

		[JsonProperty("legacy_id")]
		public string LegacyId { get; set; }
	}
}