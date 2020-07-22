using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationFeedbackRequest
	{
		public string SourceLanguageId { get; set; }
		public string TargetLanguageId { get; set; }
		public string Model { get; set; }
		[JsonProperty("TargetMTText")]
		public string TargetMtText { get; set; }
		public string SourceText { get; set; }
		public string SelectedText { get; set; }
		public string Metadata { get; set; }
	}
}
