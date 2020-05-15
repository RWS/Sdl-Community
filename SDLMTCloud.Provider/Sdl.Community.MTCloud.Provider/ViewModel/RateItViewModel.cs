using System;
using System.Collections.Generic;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Studio.ShortcutActions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel:BaseViewModel, IRatingService, IDisposable
	{		
		private readonly IShortcutService _shortcutService;
		private readonly List<ISDLMTCloudAction> _actions;
		private ITranslationService _translationService;

		private ICommand _ratingCommand;
		private ICommand _sendFeedbackCommand;
		private ICommand _clearCommand;

		private FeedbackOption _wordsOmissionChecked;
		private FeedbackOption _grammarChecked;
		private FeedbackOption _unintelligenceChecked;
		private FeedbackOption _wordChoiceChecked;
		private FeedbackOption _wordsAdditionChecked;
		private FeedbackOption _spellingChecked;
		private FeedbackOption _capitalizationChecked;

		private int _rating;
		private string _feedback;
		private string _wordsOmissionTooltip;

		public RateItViewModel(ITranslationService translationService, IShortcutService shortcutService)
		{
			SetTranslationService(translationService);

			_shortcutService = shortcutService;
			_wordsOmissionChecked = new FeedbackOption();
			_grammarChecked = new FeedbackOption();
			_unintelligenceChecked = new FeedbackOption();
			_wordChoiceChecked = new FeedbackOption();
			_wordsAdditionChecked = new FeedbackOption();
			_spellingChecked = new FeedbackOption();
			_capitalizationChecked = new FeedbackOption();

			RatingCommand = new CommandHandler(RatingChanged);
			SendFeedbackCommand = new CommandHandler(SendFeedback);
			ClearCommand = new CommandHandler(ClearFeedbackBox);

			_actions = GetActions();
			UpdateActionTooltips();

			_rating = 0;
		}

		public FeedbackOption WordsOmissionOption
		{
			get => _wordsOmissionChecked;
			set
			{
				if (_wordsOmissionChecked == value) return;
				_wordsOmissionChecked = value;
				OnPropertyChanged(nameof(WordsOmissionOption));
			}
		}

		public FeedbackOption GrammarOption
		{
			get => _grammarChecked;
			set
			{
				if (_grammarChecked == value) return;
				_grammarChecked = value;
				OnPropertyChanged(nameof(GrammarOption));
			}
		}

		public FeedbackOption UnintelligenceOption
		{
			get => _unintelligenceChecked;
			set
			{
				if (_unintelligenceChecked == value) return;
				_unintelligenceChecked = value;
				OnPropertyChanged(nameof(UnintelligenceOption));
			}
		}

		public FeedbackOption WordChoiceOption
		{
			get => _wordChoiceChecked;
			set
			{
				if (_wordChoiceChecked == value) return;
				_wordChoiceChecked = value;
				OnPropertyChanged(nameof(WordChoiceOption));
			}
		}

		public FeedbackOption WordsAdditionOption
		{
			get => _wordsAdditionChecked;
			set
			{
				if (_wordsAdditionChecked == value) return;
				_wordsAdditionChecked = value;
				OnPropertyChanged(nameof(WordsAdditionOption));
			}
		}

		public FeedbackOption SpellingOption
		{
			get => _spellingChecked;
			set
			{
				if (_spellingChecked == value) return;
				_spellingChecked = value;
				OnPropertyChanged(nameof(SpellingOption));
			}
		}

		public FeedbackOption CapitalizationOption
		{
			get => _capitalizationChecked;
			set
			{
				if (_capitalizationChecked == value) return;
				_capitalizationChecked = value;
				OnPropertyChanged(nameof(CapitalizationOption));
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
			if (string.IsNullOrWhiteSpace(optionName)) return;
			var propertyInfo = GetType().GetProperty(optionName);
			if (propertyInfo == null) return;

			dynamic option = propertyInfo.GetValue(this);
			var currentCheckboxState = option.IsChecked;
			option.IsChecked = !currentCheckboxState;
			propertyInfo.SetValue(this, option);
		}

		public void SetOptionTooltip(string optionName, string tooltip)
		{
			if (string.IsNullOrWhiteSpace(optionName)) return;
			var propertyInfo = GetType().GetProperty(optionName);
			if (propertyInfo == null) return;

			dynamic option = propertyInfo.GetValue(this);
			option.Tooltip = !string.IsNullOrEmpty(tooltip) ? tooltip : "No shortcut was set in Studio for this option";
			propertyInfo.SetValue(this, option);
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

		private void SetTranslationService(ITranslationService translationService)
		{
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= _translationService_TranslationReceived;
			}

			_translationService = translationService;

			if (_translationService != null)
			{
				_translationService.TranslationReceived += _translationService_TranslationReceived;
			}
		}

		private List<ISDLMTCloudAction> GetActions()
		{
			return new List<ISDLMTCloudAction>
			{
				SdlTradosStudio.Application.GetAction<DecreaseRatingAction>(),
				SdlTradosStudio.Application.GetAction<IncreaseRatingAction>(),
				SdlTradosStudio.Application.GetAction<SetCapitalizationAction>(),
				SdlTradosStudio.Application.GetAction<SetGrammarAction>(),
				SdlTradosStudio.Application.GetAction<SetSpellingAction>(),
				SdlTradosStudio.Application.GetAction<SetUnintelligenceAction>(),
				SdlTradosStudio.Application.GetAction<SetWordChoiceAction>(),
				SdlTradosStudio.Application.GetAction<SetWordsAdditionAction>(),
				SdlTradosStudio.Application.GetAction<SetWordsOmissionAction>()
			};
		}

		private void UpdateActionTooltips()
		{
			foreach (var mtCloudAction in _actions)
			{
				var tooltipText = _shortcutService.GetShotcutDetails(mtCloudAction.Id);
				SetOptionTooltip(mtCloudAction.OptionName, tooltipText);
			}
		}

		public void Dispose()
		{
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= _translationService_TranslationReceived;
			}
		}
	}
}
