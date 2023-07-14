using System.Collections.Generic;
using System.Linq;
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

		[DataMember]
		public List<TranslationModel> Models
		{
			get => _models;
			set
			{
				_models = value;
				OnPropertyChanged(nameof(Models));
			}
		}


		[DataMember]
		public List<MTCloudDictionary> Dictionaries
		{
			get => _dictionaries;
			set
			{
				_dictionaries = value;
				OnPropertyChanged(nameof(Dictionaries));
			}
		}

		[DataMember]
		public List<LinguisticOption> LinguisticOptions
		{
			get
			{
				var output = new List<LinguisticOption>();
				if (_linguisticOptions is null) return _linguisticOptions;
				for (var i = _linguisticOptions.Count - 1; i >= 0; i--)
				{
					var lo = _linguisticOptions[i];
					if (output.Any(x => x.Id == lo.Id))
					{
						continue;
					}

					output.Add(lo);
				}

				_linguisticOptions = output;
				return _linguisticOptions;
			}
			set
			{
				_linguisticOptions = value;
				OnPropertyChanged(nameof(LinguisticOptions));
				OnPropertyChanged(nameof(HasLinguisticOptions));
			}
		}

		[DataMember]
		public bool HasLinguisticOptions => LinguisticOptions?.Count >= 1;
	}
}