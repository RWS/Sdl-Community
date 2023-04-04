using System.Collections.Generic;
using System.Collections.ObjectModel;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		bool IsMicrosoftSelected { get; set; }
		
		bool UseCategoryID { get; set; }
		
		bool PersistMicrosoftKey { get; set; }

		bool PersistPrivateEndpoint { get; set; }

		bool UsePrivateEndpoint { get; set; }
		
		bool IsTellMeAction { get; set; }
		
		string CategoryID { get; set; }
		
		string ClientID { get; set; }

		string PrivateEndpoint { get; set; }

		BaseModel ViewModel { get; set; }
		
		RegionSubscription Region { get; set; }

		ObservableCollection<RegionSubscription> Regions { get; set; }

		List<TranslationOption> TranslationOptions { get; set; }
		
		TranslationOption SelectedTranslationOption { get; set; }
	}
}