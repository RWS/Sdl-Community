using System.Collections.Generic;
using System.Runtime.Serialization;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	[DataContract]
	public class LanguageMappingModel : BaseViewModel
	{
		private MTCloudLanguage _selectedSource;
		private MTCloudLanguage _selectedTarget;
		private TranslationModel _selectedModel;
		private MTCloudDictionary _selectedDictionary;

		private List<MTCloudLanguage> _sourceLanguages;
		private List<MTCloudLanguage> _targetLanguages;
		private List<TranslationModel> _models;
		private List<MTCloudDictionary> _dictionaries;
		private List<LinguisticOption> _linguisticOptions;

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string SourceTradosCode { get; set; }

		[DataMember]
		public string TargetTradosCode { get; set; }

		[DataMember]
		public MTCloudLanguage SelectedSource
		{
			get => _selectedSource;
			set
			{
				if (_selectedSource == value)
				{
					return;
				}

				_selectedSource = value;
				OnPropertyChanged(nameof(SelectedSource));
			}
		}

		[DataMember]
		public MTCloudLanguage SelectedTarget
		{
			get => _selectedTarget;
			set
			{
				if (_selectedTarget == value)
				{
					return;
				}

				_selectedTarget = value;
				OnPropertyChanged(nameof(SelectedTarget));
			}
		}

		[DataMember]
		public TranslationModel SelectedModel
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				OnPropertyChanged(nameof(SelectedModel));
				LinguisticOptions = value?.LinguisticOptions;
			}
		}

		[DataMember]
		public MTCloudDictionary SelectedDictionary
		{
			get => _selectedDictionary;
			set
			{
				_selectedDictionary = value;
				OnPropertyChanged(nameof(SelectedDictionary));
			}
		}

		[DataMember]
		public List<MTCloudLanguage> SourceLanguages
		{
			get => _sourceLanguages;
			set
			{
				_sourceLanguages = value;
				OnPropertyChanged(nameof(SourceLanguages));
			}
		}

		[DataMember]
		public List<MTCloudLanguage> TargetLanguages
		{
			get => _targetLanguages;
			set
			{
				_targetLanguages = value;
				OnPropertyChanged(nameof(TargetLanguages));
			}
		}


		public List<TranslationModel> Models
		{
			get => _models;
			set
			{
				_models = value;
				OnPropertyChanged(nameof(Models));
			}
		}


		public List<MTCloudDictionary> Dictionaries
		{
			get => _dictionaries;
			set
			{
				_dictionaries = value;
				OnPropertyChanged(nameof(Dictionaries));
			}
		}

		public List<LinguisticOption> LinguisticOptions
		{
			get => _linguisticOptions;
			set
			{
				_linguisticOptions = value;
				OnPropertyChanged(nameof(LinguisticOptions));
				OnPropertyChanged(nameof(HasLinguisticOptions));
			}
		}

		public bool HasLinguisticOptions => LinguisticOptions?.Count >= 1;
	}
}