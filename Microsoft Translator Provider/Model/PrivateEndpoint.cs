using System.Collections.Generic;

namespace MicrosoftTranslatorProvider.Model
{
	public class PrivateEndpoint
    {
		public string Endpoint { get; set; }

		public List<UrlMetadata> Headers { get; set; }

		public List<UrlMetadata> Parameters { get; set; }
    }
}