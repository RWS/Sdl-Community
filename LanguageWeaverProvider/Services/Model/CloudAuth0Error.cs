using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudAuth0Error
	{
		public string Error { get; set; }

		[JsonProperty("error_description")]
		public string ErrorDescription { get; set; }

		public override string ToString()
		{
			return $"{Error}\n{ErrorDescription}";
		}
	}
}