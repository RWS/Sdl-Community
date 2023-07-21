using System.Text.Json.Serialization;

namespace MicrosoftTranslatorProvider.Model
{
	public class LanguageDetails
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("nativeName")]
		public string NativeName { get; set; }
	}
}