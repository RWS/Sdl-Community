using System.Collections.Generic;
using System.Collections.ObjectModel;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interface
{
	public interface IPrivateEndpointViewModel
	{
		public BaseModel ViewModel { get; }
		public string Endpoint { get; set; }
		public List<UrlMetadata> Headers { get; set; }
		public List<UrlMetadata> Parameters { get; set; }
	}
}