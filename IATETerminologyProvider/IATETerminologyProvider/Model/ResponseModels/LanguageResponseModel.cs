using System.Collections.Generic;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class LanguageResponseModel
	{
		public MetaResponseModel Meta { get; set; }
		public string Definition { get; set; }

		[JsonProperty("tooltip_definition")]
		public string TooltipDefinition { get; set; }

		[JsonProperty("definition_references")]
		public List<DefinitionReferencesResponseModel> DefinitionReferences { get; set; }

		public NoteResponseModel Note { get; set; }
		public MetadataResponseModel Metadata { get; set; }

		[JsonProperty("term_entries")]
		public TermEntriesResponseModel TermEntries { get; set; }
	}
}