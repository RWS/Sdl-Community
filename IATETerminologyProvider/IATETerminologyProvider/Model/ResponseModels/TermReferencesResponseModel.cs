using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class TermReferencesResponseModel
	{
		public string Code { get; set; }
		public string Text { get; set; }

		[JsonProperty("tooltip_value")]
		public string TooltipValue { get; set; }

		public MetadataResponseModel Metadata { get; set; }
	}
}