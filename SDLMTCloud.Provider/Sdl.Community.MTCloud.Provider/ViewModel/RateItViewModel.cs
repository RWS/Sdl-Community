using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel : BaseViewModel, IRatingService, IDisposable
	{
		private IShortcutService _shortcutService;
		private ITranslationService _translationService;
		private readonly IActionProvider _actionProvider;
		private List<ISDLMTCloudAction> _actions;
		private ICommand _sendFeedbackCommand;

		private int _rating;
		private string _feedback;
		private bool _isSendFeedbackEnabled;

		public RateItViewModel(IShortcutService shortcutService, IActionProvider actionProvider)
		{
			_actionProvider = actionProvider;
			SetShortcutService(shortcutService);
			ClearCommand = new CommandHandler(ClearFeedbackBox);

			Initialize();
			UpdateActionTooltips();

			_rating = 0;
		}

		public ObservableCollection<FeedbackOption> FeedbackOptions { get; set; }

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

		public bool IsSendFeedbackEnabled
		{
			get => _isSendFeedbackEnabled;
			set
			{
				if (_isSendFeedbackEnabled == value) return;
				_isSendFeedbackEnabled = value;
				OnPropertyChanged(nameof(IsSendFeedbackEnabled));
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
			if (Rating > 0)
			{
				Rating--;
			}
		}

		public void SetRateOptionFromShortcuts(string optionName)
		{
			if (string.IsNullOrWhiteSpace(optionName)) return;

			var option = FeedbackOptions.FirstOrDefault(fo => fo.OptionName == optionName);
			if (option != null)
			{
				option.IsChecked = !option.IsChecked;
			}
		}

		private async Task SendFeedbackToService()
		{
			if (_translationService != null)
			{
				var comments = new List<string>{Feedback};
				comments.AddRange(FeedbackOptions.Where(fo => fo.IsChecked).Select(fo => fo.OptionName).ToList());

				await _translationService.SendFeedback(new Rating
				{
					Score = Rating,
					Comments = comments
				});
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

		private void Initialize()
		{
			_actions = _actionProvider.GetActions();
			var feedbackOptions = _actions.Where(action => IsFeedbackOption(action.GetType().Name));

			FeedbackOptions = new ObservableCollection<FeedbackOption>();
			foreach (var feedbackOption in feedbackOptions)
			{
				FeedbackOptions.Add(new FeedbackOption
				{
					OptionName = feedbackOption.Text,
					StudioActionId = feedbackOption.Id
				});
			}
		}

		private void UpdateActionTooltips()
		{
			if (_actions is null) return;
			foreach (var feedbackOption in FeedbackOptions)
			{
				var tooltipText = _shortcutService.GetShotcutDetails(feedbackOption.StudioActionId);
				SetFeedbackOptionTooltip(feedbackOption.OptionName, tooltipText);
			}
		}

		private void SetFeedbackOptionTooltip(string mtCloudActionName, string tooltipText)
		{
			var feedBackOption =
				FeedbackOptions.FirstOrDefault(fo => fo.OptionName == mtCloudActionName);
			if (feedBackOption != null)
			{
				feedBackOption.Tooltip = tooltipText ?? Resources.RateItViewModel_SetOptionTooltip_No_shortcut_was_set;
			}
		}

		private bool IsFeedbackOption(string optionName)
		{
			return optionName.ToLower().Contains("set");
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
			IsSendFeedbackEnabled = sendFeedback;
		}
	}
}
