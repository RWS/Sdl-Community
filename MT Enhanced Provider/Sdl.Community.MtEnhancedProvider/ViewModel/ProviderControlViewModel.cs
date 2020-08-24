using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Schema;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class ProviderControlViewModel : ModelBase, IProviderControlViewModel
	{
		private readonly IMtTranslationOptions _options;
		private TranslationOption _selectedTranslationOption;
		private bool _isMicrosoftSelected;
		private string _apiKey;

		public ProviderControlViewModel(IMtTranslationOptions options)
		{
			ViewModel = this;
			_options = options;

			TranslationOptions = new List<TranslationOption>
			{
				new TranslationOption
				{
					Name = PluginResources.Microsoft,
					ProviderType = MtTranslationOptions.ProviderType.MicrosoftTranslator
				},
				new TranslationOption
				{
					Name = PluginResources.Google,
					ProviderType = MtTranslationOptions.ProviderType.GoogleTranslate
				}
			};
			SetTranslationOption();
		}

		public IModelBase ViewModel { get; set; }
		public ICommand ShowSettingsCommand { get; set; }
		public List<TranslationOption> TranslationOptions { get; set; }

		public TranslationOption SelectedTranslationOption
		{
			get => _selectedTranslationOption;
			set
			{
				_selectedTranslationOption = value;
				OnPropertyChanged(nameof(SelectedTranslationOption));
			}
		}

		public bool IsMicrosoftSelected
		{
			get => _isMicrosoftSelected;
			set
			{
				if (_isMicrosoftSelected == value) return;
				_isMicrosoftSelected = value;
				OnPropertyChanged(nameof(IsMicrosoftSelected));
			}
		}

		public string ApiKey
		{
			get => _apiKey;
			set
			{
				if (_apiKey == value) return;
				_apiKey = value;
				OnPropertyChanged(nameof(ApiKey));
			}
		}

		private void SetTranslationOption()
		{
			if (_options?.SelectedProvider != null)
			{
				var selectedProvider = TranslationOptions.FirstOrDefault(t => t.ProviderType.Equals(_options.SelectedProvider));

				if (selectedProvider == null)
				{
					SelectMicrosoftTranslation();
				}
				else
				{
					switch (selectedProvider.ProviderType)
					{
						case MtTranslationOptions.ProviderType.GoogleTranslate:
							IsMicrosoftSelected = false;
							break;
						case MtTranslationOptions.ProviderType.MicrosoftTranslator:
							IsMicrosoftSelected = true;
							break;
					}
					SelectedTranslationOption = selectedProvider;
				}
			}
			else
			{
				//Bydefault we'll select Microsoft translator option
				SelectMicrosoftTranslation();
			}
		}

		private void SelectMicrosoftTranslation()
		{
			SelectedTranslationOption = TranslationOptions[0];
			IsMicrosoftSelected = true;
		}
	}
}
