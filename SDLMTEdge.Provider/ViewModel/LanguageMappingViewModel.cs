using System.Collections.Generic;
using Sdl.Community.MTEdge.Provider.Interface;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.ViewModel
{
	class LanguageMappingViewModel : BaseModel, ILanguageMappingViewModel
    {
		private readonly ITranslationOptions _translationOptions;
		private List<TradosToMTEdgeLanguagePair> _languageMapping;
		private TradosToMTEdgeLanguagePair _selectedLanguageMapping;

		public LanguageMappingViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_translationOptions = options;
			LanguageMapping = options?.LanguageMapping;
		}

		public BaseModel ViewModel { get; set; }

		public List<TradosToMTEdgeLanguagePair> LanguageMapping
		{
			get => _languageMapping;
			set
			{
				if (_languageMapping == value) return;
				_languageMapping = value;
				OnPropertyChanged(nameof(LanguageMapping));
			}
		}

		public TradosToMTEdgeLanguagePair SelectedLanguageMapping
		{
			get => _selectedLanguageMapping;
			set
			{
				if (_selectedLanguageMapping == value) return;
				_selectedLanguageMapping = value;
				OnPropertyChanged(nameof(SelectedLanguageMapping));
			}
		}
	}
}