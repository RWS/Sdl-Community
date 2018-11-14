using System.Collections.Generic;
using System.Linq;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		private bool _reSendChecked;
		private TranslationModel _selectedModel;

		public SettingsViewModel(BeGlobalTranslationOptions options)
		{
			TranslationOptions = new List<TranslationModel>
			{
				new TranslationModel
				{
					DisplayName = "Generic",
					Model = "generic"
				},
				new TranslationModel
				{
					DisplayName = "Generic MT",
					Model = "genericmt"
				},
				new TranslationModel
				{
					DisplayName = "Informal",
					Model = "informal"
				},
				new TranslationModel
				{
					DisplayName = "trvlvrt",
					Model = "trvlvrt"
				}
			};

			if (options != null)
			{
				ReSendChecked = options.ResendDrafts;
				if (options.Model != null)
				{
					var selectedModelIndex = TranslationOptions.FindIndex(m => m.Model.Equals(options.Model));
					SelectedModelOption = TranslationOptions[selectedModelIndex];
				}
				else
				{
					SelectedModelOption = TranslationOptions[0];
				}  
			}
			else
			{
				SelectedModelOption = TranslationOptions[0];
			}  
		}

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				OnPropertyChanged();
			}
		}

		public TranslationModel SelectedModelOption
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				OnPropertyChanged();
			}
		}

		public List<TranslationModel> TranslationOptions { get; set; }
	}
}
