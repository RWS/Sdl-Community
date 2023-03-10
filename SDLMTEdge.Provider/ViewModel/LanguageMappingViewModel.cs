using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Sdl.Community.MTEdge.Provider.Interface;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.ViewModel
{
	class LanguageMappingViewModel : BaseModel, ILanguageMappingViewModel
	{
		private ITranslationOptions _translationOptions;
		private ObservableCollection<TradosToMTEdgeLanguagePair> _languageMapping;

		public LanguageMappingViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_translationOptions = options;
			LanguageMapping = new(options.LanguageMapping ?? new());
		}

		public BaseModel ViewModel { get; set; }

		public ITranslationOptions TranslationOptions
		{
			get => _translationOptions;
			set => _translationOptions = value;
		}

		public ObservableCollection<TradosToMTEdgeLanguagePair> LanguageMapping
		{
			get => _languageMapping;
			set
			{
				_languageMapping = value;
				OnPropertyChanged(nameof(LanguageMapping));
			}
		}

		/*public void linguisticOptionsList_Loaded(object sender, RoutedEventArgs e)
		{
			if (_translationOptions.LanguageMapping is null) return;
			var languageMapping = new List<TradosToMTEdgeLanguagePair>();
			var savedMapping = _translationOptions?.LanguageMapping;
			LanguageMapping ??= new();
			foreach (var mappedLanguage in savedMapping)
			{
				var languagePair = mappedLanguage.LanguagePair;
				var culture = mappedLanguage.TradosCulture;
				var models = mappedLanguage.MtEdgeLanguagePairs;
				var dictionaries = mappedLanguage.Dictionaries;
				var selectedDictionary = mappedLanguage.SelectedDictionary;
				var selectedModel = mappedLanguage.SelectedModel;
				var newMapped = new TradosToMTEdgeLanguagePair(languagePair, culture, models)
				{
					Dictionaries = dictionaries,
					SelectedDictionary = selectedDictionary,
					SelectedModel = selectedModel
				};

				languageMapping.Add(newMapped);
			}

			LanguageMapping = new(languageMapping);
		}*/
	}
}