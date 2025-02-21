using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class AccountCategoryFeature
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("value")]
		public double Value { get; set; }
	}
}