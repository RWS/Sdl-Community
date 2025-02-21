using System.Text.Json.Serialization;

namespace MicrosoftTranslatorProvider.Service.Model
{
	public class TranslationLanguageResponse
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
	}
}