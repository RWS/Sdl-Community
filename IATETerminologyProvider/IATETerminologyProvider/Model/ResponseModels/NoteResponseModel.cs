using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class NoteResponseModel
	{
		public string Value { get; set; }

		[JsonProperty("tooltip_value")]
		public string TooltipValue { get; set; }
		public MetadataResponseModel Metadata { get; set; }
	}
}