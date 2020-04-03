using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationResponseStatus
	{
		[JsonProperty("translationStatus")]
		public string TranslationStatus { get; set; }

		[JsonProperty("inputFormat")]
		public string InputFormat { get; set; }

		[JsonProperty("outputFormat")]
		public string OutputFormat { get; set; }

		[JsonProperty("translationStats")]
		public TranslationResponseStats TranslationStats { get; set; }		
	}
}
