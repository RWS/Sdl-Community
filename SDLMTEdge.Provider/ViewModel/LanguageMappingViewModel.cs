using System.Collections.ObjectModel;
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
	}
}