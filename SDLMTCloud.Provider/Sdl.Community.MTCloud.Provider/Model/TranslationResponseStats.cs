using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationResponseStats
	{
		[JsonProperty("inputByteCount")]
		public string InputByteCount { get; set; }

		[JsonProperty("inputCharCount")]
		public long InputCharCount { get; set; }

		[JsonProperty("inputWordCount")]
		public long InputWordCount { get; set; }

		[JsonProperty("translationByteCount")]
		public string TranslationByteCount { get; set; }

		[JsonProperty("translationCharCount")]
		public string TranslationCharCount { get; set; }

		[JsonProperty("translationWordCount")]
		public string TranslationWordCount { get; set; }
	}
}