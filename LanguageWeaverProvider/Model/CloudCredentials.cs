using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model
{
	public class CloudCredentials
    {
		public string AccountId { get; set; }

		public string AccountRegion { get; set; }

		public string UserID { get; set; }

		public string UserPassword { get; set; }

		public string ClientID { get; set; }

		public string ClientSecret { get; set; }
    }
}