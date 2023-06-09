﻿using System.Collections.ObjectModel;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interface
{
	public interface IPrivateEndpointViewModel
	{
		public BaseModel ViewModel { get; set; }
		public string Endpoint { get; set; }
		public ObservableCollection<UrlMetadata> Headers { get; set; }
		public ObservableCollection<UrlMetadata> Parameters { get; set; }
	}
}