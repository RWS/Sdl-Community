using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model
{
	public class DictionaryTerm
	{
		[JsonProperty("source")]
		public string Source { get; set; }

		[JsonProperty("target")]
		public string Target { get; set; }

		[JsonProperty("comment")]
		public string Comment { get; set; }
	}
}