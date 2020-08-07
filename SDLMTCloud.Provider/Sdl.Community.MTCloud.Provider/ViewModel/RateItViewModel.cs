﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel:BaseViewModel, IRatingService, IDisposable
	{		
		private IShortcutService _shortcutService;
		private ITranslationService _translationService;
		private readonly IActionProvider _actionProvider;
		private readonly List<ISDLMTCloudAction> _actions;
		private ICommand _sendFeedbackCommand;
		private ObservableCollection<FeedbackOption> _feedbackOptions;

		private FeedbackOption _wordsOmissionChecked;
		private FeedbackOption _grammarChecked;
		private FeedbackOption _unintelligenceChecked;
		private FeedbackOption _wordChoiceChecked;
		private FeedbackOption _wordsAdditionChecked;
		private FeedbackOption _spellingChecked;
		private FeedbackOption _capitalizationChecked;
		private int _rating;
		private string _feedback;
		private bool _sendFeedback;

		public RateItViewModel(IShortcutService shortcutService,IActionProvider actionProvider)
		{
			_actionProvider = actionProvider;
			SetShortcutService(shortcutService);
			InitializeFeedbackOptions();
			//_feedbackOptions = new ObservableCollection<FeedbackOption>(
			//	);
			ClearCommand = new CommandHandler(ClearFeedbackBox);

			_actions = InitializeActions();
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

		public bool SendFeedback
		{
			get => _sendFeedback;
			set
			{
				if (_sendFeedback == value) return;
				_sendFeedback = value;
				OnPropertyChanged(nameof(SendFeedback));
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

		public ICommand ClearCommand { get; }
		public ICommand SendFeedbackCommand => _sendFeedbackCommand ?? (_sendFeedbackCommand = new AsyncCommand(SendFeedbackToService));


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
			option.Tooltip = !string.IsNullOrEmpty(tooltip) ? tooltip : Resources.RateItViewModel_SetOptionTooltip_No_shortcut_was_set;
			propertyInfo.SetValue(this, option);
		}

		private async Task SendFeedbackToService()
		{
			if (_translationService != null)
			{
				var accountId = _translationService.ConnectionService.Credential.AccountId;
				var test = new FeedbackRequest();
				await _translationService.CreateTranslationFeedback(test, accountId);
			}
			else
			{
				//TODO: Log that translation service is null
			}
		}

		private void ClearFeedbackBox(object obj)
		{
			Feedback = string.Empty;
		}

		private void _translationService_TranslationReceived(FeedbackRequest translationFeedback)
		{

		}

		private void SetShortcutService(IShortcutService shortcutService)
		{
			if (_shortcutService != null)
			{
				_shortcutService.StudioShortcutChanged -= _shortcutService_ShortcutChanged;
			}

			_shortcutService = shortcutService;

			if (_shortcutService != null)
			{
				_shortcutService.StudioShortcutChanged += _shortcutService_ShortcutChanged;
			}
		}
		private void _shortcutService_ShortcutChanged()
		{
			UpdateActionTooltips();
		}

		private List<ISDLMTCloudAction> InitializeActions()
		{
			return _actionProvider.GetActions();
		}

		private void InitializeFeedbackOptions()
		{
			_wordsOmissionChecked = new FeedbackOption();
			_grammarChecked = new FeedbackOption();
			_unintelligenceChecked = new FeedbackOption();
			_wordChoiceChecked = new FeedbackOption();
			_wordsAdditionChecked = new FeedbackOption();
			_spellingChecked = new FeedbackOption();
			_capitalizationChecked = new FeedbackOption();
		}

		private void UpdateActionTooltips()
		{
			if (_actions is null) return;
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

			if (_shortcutService != null)
			{
				_shortcutService.StudioShortcutChanged -= _shortcutService_ShortcutChanged;
			}
		}

		public void SetTranslationService(ITranslationService translationService)
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

		public void SetSendFeedback(bool sendFeedback)
		{
			SendFeedback = sendFeedback;
		}
	}
}
