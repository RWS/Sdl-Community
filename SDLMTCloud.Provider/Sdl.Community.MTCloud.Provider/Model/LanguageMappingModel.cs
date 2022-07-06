using System.Collections.Generic;
using System.Runtime.Serialization;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	[DataContract]
	public class LanguageMappingModel : BaseViewModel
	{
		private List<MTCloudDictionary> _dictionaries;
		private List<TranslationModel> _models;
		private MTCloudDictionary _selectedDictionary;
		private TranslationModel _selectedModel;
		private MTCloudLanguage _selectedSource;
		private MTCloudLanguage _selectedTarget;
		private List<MTCloudLanguage> _sourceLanguages;
		private List<MTCloudLanguage> _targetLanguages;

		public List<MTCloudDictionary> Dictionaries
		{
			get => _dictionaries;
			set
			{
				_dictionaries = value;
				OnPropertyChanged(nameof(Dictionaries));
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

		[DataMember]
		public string Name { get; set; }

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
		public TranslationModel SelectedModel
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				OnPropertyChanged(nameof(SelectedModel));
			}
		}

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
		public string SourceTradosCode { get; set; }

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
		public string TargetTradosCode { get; set; }
	}
}