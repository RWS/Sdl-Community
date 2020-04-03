using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationResponseStats
	{
		[JsonProperty("inputWordCount")]
		public long InputWordCount { get; set; }

		[JsonProperty("inputCharCount")]
		public long InputCharCount { get; set; }

		[JsonProperty("inputByteCount")]
		public string InputByteCount { get; set; }

		[JsonProperty("translationWordCount")]
		public string TranslationWordCount { get; set; }

		[JsonProperty("translationCharCount")]
		public string TranslationCharCount { get; set; }

		[JsonProperty("translationByteCount")]
		public string TranslationByteCount { get; set; }
	}
}
