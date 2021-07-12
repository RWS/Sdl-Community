using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model.RateIt
{
	public class TranslationFeedbackRequest
	{
		public string Model { get; set; }
		public string SourceLanguageId { get; set; }
		public string SourceText { get; set; }
		public string TargetLanguageId { get; set; }

		[JsonProperty("TargetMTText")]
		public string TargetMtText { get; set; }
	}
}