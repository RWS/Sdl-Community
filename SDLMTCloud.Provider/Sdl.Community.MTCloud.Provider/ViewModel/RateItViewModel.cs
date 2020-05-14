using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Studio.ShortcutActions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel:BaseViewModel,IRatingService
	{
		private ITranslationService _translationService;
		private IShortcutService _shortcutService;
		private ICommand _ratingCommand;
		private ICommand _sendFeedbackCommand;
		private ICommand _clearCommand;
		private bool _wordsOmissionChecked;
		private bool _grammarChecked;
		private bool _unintelligenceChecked;
		private bool _wordChoiceChecked;
		private bool _wordsAdditionChecked;
		private bool _spellingChecked;
		private bool _capitalizationChecked;
		private int _rating;
		private string _feedback;
		private string _wordsOmissionTooltip;

		public RateItViewModel(ITranslationService translationService,IShortcutService shortcutService)
		{
			_translationService = translationService;
			_shortcutService = shortcutService;
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= _translationService_TranslationReceived;
				_translationService.TranslationReceived += _translationService_TranslationReceived;
			}
			_rating = 0;
			SetOptionsTooltips();
			RatingCommand = new CommandHandler(RatingChanged);
			SendFeedbackCommand = new CommandHandler(SendFeedback);
			ClearCommand = new CommandHandler(ClearFeedbackBox);
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

		public int Rating
		{
			get => _rating;
			set
			{
				if (_rating == value) return;
				_rating = value;
				OnPropertyChanged(nameof(Rating));
			}
		}

		public string Feedback
		{
			get => _feedback;
			set
			{
				if (_feedback == value) return;
				_feedback = value;
				OnPropertyChanged(nameof(Feedback));
			}
		}

		public string WordsOmissionTooltip
		{
			get => _wordsOmissionTooltip;
			set
			{
				if (_wordsOmissionTooltip == value) return;
				_wordsOmissionTooltip = value;
				OnPropertyChanged(nameof(WordsOmissionTooltip));
			}
		}

		public ICommand RatingCommand { get; }
		public ICommand SendFeedbackCommand { get;}
		public ICommand ClearCommand { get; }


		public void IncreaseRating()
		{
			if (Rating < 5)
			{
				Rating++;
			}
		}

		public void DecreaseRating()
		{
			if (Rating>0)
			{
				Rating--;
			}
		}

		public void SetRateOptionFromShortcuts(string optionName)
		{
			if (!string.IsNullOrWhiteSpace(optionName))
			{
				var propertyInfo = GetType().GetProperty(optionName);
				if (propertyInfo == null) return;
				var currentCheckboxState = (bool)propertyInfo.GetValue(this);
				propertyInfo.SetValue(this,!currentCheckboxState);
			}
		}

		public void SetTooltipsDinamically(string optionName,string tooltip)
		{
			if (string.IsNullOrWhiteSpace(optionName)) return;
			var propertyInfo = GetType().GetProperty(optionName);
			if (propertyInfo == null) return;
			tooltip =!string.IsNullOrEmpty(tooltip) ? tooltip : "No shortcut was set in Studio for this option";
			propertyInfo.SetValue(this, tooltip);
		}

		private void RatingChanged(object obj)
		{
			
		}
		private void SendFeedback(object obj)
		{
		}
		private void ClearFeedbackBox(object obj)
		{
			Feedback = string.Empty;
		}
		private void _translationService_TranslationReceived(Feedback translationFeedback)
		{

		}
		private void SetOptionsTooltips()
		{
			var wordsOmissionActionId = SdlTradosStudio.Application.GetAction<SetWordsOmissionAction>().Id;
			var wordsOmissionShortcut = _shortcutService.GetShotcutDetails(wordsOmissionActionId);
			SetTooltipsDinamically(nameof(WordsOmissionTooltip),wordsOmissionShortcut);

			//TODO: Add tooltips for the rest of options.
		}
	}
}
