using System.Collections.Generic;
using System.Collections.ObjectModel;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.ViewModel;

namespace MicrosoftTranslatorProvider.Interface
{
	public interface IPrivateEndpointViewModel
	{
		public BaseViewModel ViewModel { get; }
		public string Endpoint { get; set; }
		public ObservableCollection<UrlMetadata> Headers { get; set; }
		public ObservableCollection<UrlMetadata> Parameters { get; set; }
	}
}