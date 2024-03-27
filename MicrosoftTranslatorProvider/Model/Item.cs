using System.Text.Json.Serialization;

namespace MicrosoftTranslatorProvider.Model
{
    public class Item
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }
	}
}