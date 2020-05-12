using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel:BaseViewModel
	{
		private ITranslationService _translationService;
		private bool _wordsOmissionChecked;
		private bool _grammarChecked;
		private bool _unintelligenceChecked;
		private bool _wordChoiceChecked;
		private bool _wordsAdditionChecked;
		private bool _spellingChecked;
		private bool _capitalizationChecked;

		public RateItViewModel(ITranslationService translationService)
		{
			_translationService = translationService;
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= _translationService_TranslationReceived;
				_translationService.TranslationReceived += _translationService_TranslationReceived;
			}
		}

		public bool WordsOmissionChecked
		{
			get => _wordsOmissionChecked;
			set
			{
				if (_wordsOmissionChecked == value) return;
				_wordsOmissionChecked = value;
				OnPropertyChanged(nameof(WordsOmissionChecked));
			}
		}

		public bool GrammarChecked
		{
			get => _grammarChecked;
			set
			{
				if (_grammarChecked == value) return;
				_grammarChecked = value;
				OnPropertyChanged(nameof(GrammarChecked));
			}
		}

		public bool UnintelligenceChecked
		{
			get => _unintelligenceChecked;
			set
			{
				if (_unintelligenceChecked == value) return;
				_unintelligenceChecked = value;
				OnPropertyChanged(nameof(UnintelligenceChecked));
			}
		}

		public bool WordChoiceChecked
		{
			get => _wordChoiceChecked;
			set
			{
				if (_wordChoiceChecked == value) return;
				_wordChoiceChecked = value;
				OnPropertyChanged(nameof(WordChoiceChecked));
			}
		}

		public bool WordsAdditionChecked
		{
			get => _wordsAdditionChecked;
			set
			{
				if (_wordsAdditionChecked == value) return;
				_wordsAdditionChecked = value;
				OnPropertyChanged(nameof(WordsAdditionChecked));
			}
		}

		public bool SpellingChecked
		{
			get => _spellingChecked;
			set
			{
				if (_spellingChecked == value) return;
				_spellingChecked = value;
				OnPropertyChanged(nameof(SpellingChecked));
			}
		}

		public bool CapitalizationChecked
		{
			get => _capitalizationChecked;
			set
			{
				if (_capitalizationChecked == value) return;
				_capitalizationChecked = value;
				OnPropertyChanged(nameof(CapitalizationChecked));
			}
		}

		private void _translationService_TranslationReceived(Feedback translationFeedback)
		{
			
		}
	}
}
