using Newtonsoft.Json;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class GlossaryLanguagePair
	{
		[JsonProperty("source_lang")]
		public string SourceLanguage { get; set; }

		[JsonProperty("target_lang")]
		public string TargetLanguage { get; set; }
	}
}