using System.Collections.ObjectModel;
using System.Linq;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LanguageMappingsViewModel : BaseViewModel
	{
		private TranslationModel _selectedModel;
		private bool _reSendChecked;
		private string _messageVisibility;

		public LanguageMappingsViewModel(BeGlobalTranslationOptions options)
		{
			MessageVisibility = "Collapsed";

			Options = options;
			TranslationOptions = new ObservableCollection<TranslationModel>();

			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;
				if (options.Model != null)
				{
					var model = TranslationOptions.FirstOrDefault(m => m.Model.Equals(options.Model));
					if (model != null)
					{
						var selectedModelIndex = TranslationOptions.IndexOf(model);
						SelectedModelOption = TranslationOptions[selectedModelIndex];
					}
				}
			}
		}

		public BeGlobalTranslationOptions Options { get; set; }
		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }

		public string MessageVisibility
		{
			get => _messageVisibility;
			set
			{
				_messageVisibility = value;
				OnPropertyChanged();
			}
		}

		public TranslationModel SelectedModelOption
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;				
				OnPropertyChanged(nameof(SelectedModelOption));
			}
		}

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				if (Options?.Model != null)
				{
					Options.ResendDrafts = value;
				}
				OnPropertyChanged(nameof(ReSendChecked));
			}
		}		
	}
}
