using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Schema;
using Sdl.Community.MtEnhancedProvider.Annotations;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class ProviderControlViewModel : ModelBase, IProviderControlViewModel
	{
		private readonly IMtTranslationOptions _options;
		private TranslationOption _selectedTranslationOption;
		private GoogleApiVersion _selectedGoogleApiVersion;
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

			GoogleApiVersions = new List<GoogleApiVersion>
			{
				new GoogleApiVersion
				{
					Name = "V2 - Basic Translation",
					Version = Enums.GoogleApiVersion.V2
				},
				new GoogleApiVersion
				{
					Name = "V3 - Advanced Translation",
					Version = Enums.GoogleApiVersion.V3
				}
			};
			//TODO: Set the selected google api version from settings
		}

		public ModelBase ViewModel { get; set; }
		public ICommand ShowSettingsCommand { get; set; }
		public List<TranslationOption> TranslationOptions { get; set; }
		public List<GoogleApiVersion> GoogleApiVersions { get; set; }

		public GoogleApiVersion SelectedGoogleApiVersion
		{
			get => _selectedGoogleApiVersion;
			set
			{
				_selectedGoogleApiVersion = value;
				OnPropertyChanged(nameof(SelectedGoogleApiVersion));
			}
		}

		public TranslationOption SelectedTranslationOption
		{
			get => _selectedTranslationOption;
			set
			{
				_selectedTranslationOption = value;
				IsMicrosoftSelected = value.ProviderType == MtTranslationOptions.ProviderType.MicrosoftTranslator;
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
						case MtTranslationOptions.ProviderType.None:
							IsMicrosoftSelected = false;
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
