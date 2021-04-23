using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
//using Sdl.ProjectAutomation.Settings.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel : BaseViewModel, IRatingService, IDisposable
	{
		private readonly IActionProvider _actionProvider;
		private readonly EditorController _editorController;
		private readonly IMessageBoxService _messageBoxService;
		private readonly ISegmentSupervisor _segmentSupervisor;
		private readonly IShortcutService _shortcutService;
		private List<ISDLMTCloudAction> _actions;
		private bool? _autoSendFeedback;
		private QualityEstimation _evaluation = new QualityEstimation();
		private ICommand _clearCommand;
		private string _feedback;
		private int _rating;
		private ICommand _sendFeedbackCommand;
		private ITranslationService _translationService;

		public RateItViewModel(IShortcutService shortcutService, IActionProvider actionProvider, ISegmentSupervisor segmentSupervisor, IMessageBoxService messageBoxService, EditorController editorController)
		{
			_actionProvider = actionProvider;
			_segmentSupervisor = segmentSupervisor;
			_messageBoxService = messageBoxService;
			_editorController = editorController;
			_shortcutService = shortcutService;

			Initialize();
			UpdateActionTooltips();
		}

		public bool? AutoSendFeedback
		{
			get => _autoSendFeedback;
			set
			{
				_autoSendFeedback = value;
				OnPropertyChanged(nameof(AutoSendFeedback));
			}
		}

		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(ClearFeedbackBox));

		public QualityEstimation Evaluation
		{
			get => _evaluation;
			set
			{
				_evaluation = value;
				OnPropertyChanged(nameof(Evaluation));
			}
		}

		public string FeedbackMessage
		{
			get => _feedback ?? string.Empty;
			set
			{
				if (_feedback == value) return;
				_feedback = value;
				OnPropertyChanged(nameof(FeedbackMessage));
			}
		}

		public List<FeedbackOption> FeedbackOptions { get; set; }

		public FeedbackSendingStatus FeedbackSendingStatus { get; set; } = new FeedbackSendingStatus();

		public bool IsSendFeedbackEnabled
		{
			get
			{
				return _translationService != null && _translationService.Options.SendFeedback;
			}
			set
			{
				if (_translationService.Options.SendFeedback == value) return;
				_translationService.Options.SendFeedback = value;
				OnPropertyChanged(nameof(IsSendFeedbackEnabled));
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

		public ICommand SendFeedbackCommand
			=> _sendFeedbackCommand ?? (_sendFeedbackCommand = new AsyncCommand(() => SendFeedback(null)));

		private SegmentId? ActiveSegmentId => _editorController.ActiveDocument.ActiveSegmentPair?.Properties.Id;

		private Rating PreviousRating { get; set; } = new Rating();

		private List<string> RateItControlProperties { get; set; }

		public void DecreaseRating()
		{
			if (Rating > 0)
			{
				Rating--;
			}
		}

		public void Dispose()
		{
			if (_shortcutService != null)
			{
				_shortcutService.StudioShortcutChanged -= _shortcutService_ShortcutChanged;
			}
		}

		public void IncreaseRating()
		{
			if (Rating < 5)
			{
				Rating++;
			}
		}

		public void SetRateOptionFromShortcuts(string optionName)
		{
			if (!IsSendFeedbackEnabled) return;
			if (string.IsNullOrWhiteSpace(optionName)) return;

			var option = FeedbackOptions.FirstOrDefault(fo => fo.OptionName == optionName);
			if (option != null)
			{
				option.IsChecked = !option.IsChecked;
			}
		}

		public void SetTranslationService(ITranslationService translationService)
		{
			_translationService = translationService;
			_segmentSupervisor.StartSupervising(_translationService);

			OnPropertyChanged(nameof(IsSendFeedbackEnabled));

			if (AutoSendFeedback == null)
			{
				AutoSendFeedback = _translationService.Options.AutoSendFeedback;
			}
		}

		private void _shortcutService_ShortcutChanged()
		{
			UpdateActionTooltips();
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			if (!IsSendFeedbackEnabled) return;
			if (AutoSendFeedback ?? false)
			{
				BackupFeedback();
			}

			ResetFeedback();
			ResetFeedbackSendingStatus(sender,
				new PropertyChangedEventArgs(nameof(_editorController.ActiveDocument.ActiveSegmentChanged)));
		}

		private void BackupFeedback()
		{
			PreviousRating = new Rating
			{
				Score = Rating,
				Comments = GetCommentsAndFeedbackFromUi()
			};
		}

		private void ClearFeedbackBox(object obj)
		{
			FeedbackMessage = string.Empty;
		}

		private void DefaultFeedbackSendingStatus()
		{
			FeedbackSendingStatus.Status = Status.Default;
			OnPropertyChanged(nameof(FeedbackSendingStatus));
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (_editorController.ActiveDocument == null)
			{
				ResetFeedbackSendingStatus(null, null);
				return;
			}

			ResetFeedback();

			_editorController.ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			_editorController.ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
		}

		private List<string> GetCommentsAndFeedbackFromUi()
		{
			var comments = new List<string>();
			if (!string.IsNullOrWhiteSpace(FeedbackMessage))
			{
				comments.Add(FeedbackMessage);
			}
			comments.AddRange(FeedbackOptions.Where(fo => fo.IsChecked).Select(fo => fo.OptionName).ToList());
			return comments;
		}

		private ImprovementFeedback GetImprovement(SegmentId? segmentId = null)
		{
			return _segmentSupervisor.GetImprovement(segmentId);
		}

		private dynamic GetRatingObject(SegmentId? segmentId)
		{
			dynamic rating = new ExpandoObject();

			var isFeedbackForPreviousSegment = (AutoSendFeedback ?? false) && segmentId != null && segmentId != ActiveSegmentId;

			var score = isFeedbackForPreviousSegment ? PreviousRating.Score : _rating;
			if (score > 0) rating.Score = score;

			var comments = isFeedbackForPreviousSegment ? PreviousRating.Comments : GetCommentsAndFeedbackFromUi();
			if (comments?.Count > 0) rating.Comments = comments;

			PreviousRating.Empty();

			if (!((ExpandoObject)rating).Any()) rating = null;
			return rating;
		}

		private string GetSourceSegment(SegmentId? segmentId)
		{
			var currentSegmentId = segmentId ?? ActiveSegmentId;
			return _editorController.ActiveDocument.SegmentPairs.FirstOrDefault(sp => sp.Properties.Id == currentSegmentId)?.Source?
				.ToString();
		}

		private void Initialize()
		{
			SetShortcutService();

			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
			MtCloudApplicationInitializer.MetadataSupervisor.ActiveSegmentQeChanged += MetadataSupervisor_ActiveSegmentQeChanged;

			_actions = _actionProvider.GetActions();
			var feedbackOptions = _actions.Where(action => IsFeedbackOption(action.GetType().Name));

			RateItControlProperties = new List<string>();
			FeedbackOptions = new List<FeedbackOption>();
			foreach (var feedbackOption in feedbackOptions)
			{
				RateItControlProperties.Add(feedbackOption.Text);
				FeedbackOptions.Add(new FeedbackOption
				{
					OptionName = feedbackOption.Text,
					StudioActionId = feedbackOption.Id
				});
			}

			RateItControlProperties.Add(nameof(FeedbackMessage));
			RateItControlProperties.Add(nameof(Rating));

			PropertyChanged += RateItViewModel_PropertyChanged;
		}

		private bool IsFeedbackOption(string optionName)
		{
			return optionName.ToLower().Contains("set");
		}

		private bool IsResetNeeded(object sender, PropertyChangedEventArgs e)
		{
			var isResetNeeded = false;
			switch (sender)
			{
				case FeedbackOption feedbackOption:
				{
					if (RateItControlProperties.Contains(feedbackOption.OptionName))
					{
						isResetNeeded = true;
					}
					break;
				}
				case RateItViewModel _:
				{
					if (RateItControlProperties.Contains(e.PropertyName))
					{
						isResetNeeded = true;
					}
					break;
				}
				case Document _:
				{
					if (e.PropertyName == nameof(Document.ActiveSegmentChanged))
					{
						isResetNeeded = true;
					}
					break;
				}
			}
			return isResetNeeded;
		}

		private void MetadataSupervisor_ActiveSegmentQeChanged(string qualityEstimation)
		{
			Evaluation.OriginalEstimation = qualityEstimation;
		}

		private async void OnConfirmationLevelChanged(SegmentId confirmedSegment)
		{
			if (!IsSendFeedbackEnabled) return;
			await SendFeedback(confirmedSegment);
		}

		private void OnFeedbackSendingStatusChanged()
		{
			OnPropertyChanged(nameof(FeedbackSendingStatus));
			SwitchListeningForPropertyChanges(true);
		}

		private void RateItViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AutoSendFeedback))
			{
				if (_autoSendFeedback != null)
				{
					_translationService.Options.AutoSendFeedback = _autoSendFeedback.Value;
				}

				if (AutoSendFeedback ?? false)
				{
					StartSendingOnConfirmationLevelChanged();
				}
				else
				{
					StopSendingOnConfirmationLevelChanged();
				}
			}
			if (e.PropertyName == nameof(IsSendFeedbackEnabled))
			{
				if (!IsSendFeedbackEnabled)
				{
					ResetRateIt();
				}
			}
		}

		private void ResetFeedback()
		{
			FeedbackOptions.ForEach(fb => fb.IsChecked = false);
			FeedbackMessage = string.Empty;
			Rating = 0;
		}

		private void ResetFeedbackSendingStatus(object sender, PropertyChangedEventArgs e)
		{
			var isResetNeeded = IsResetNeeded(sender, e);
			var isDocumentClosingEvent = sender == null && e == null;
			if (isDocumentClosingEvent || isResetNeeded)
			{
				DefaultFeedbackSendingStatus();

				if (isResetNeeded)
				{
					SwitchListeningForPropertyChanges(false);
				}
			}
		}

		private void ResetRateIt()
		{
			ResetFeedback();
			AutoSendFeedback = false;
			FeedbackSendingStatus.Status = Status.Default;
			OnPropertyChanged(nameof(FeedbackSendingStatus));
		}

		private async Task SendFeedback(SegmentId? segmentId)
		{
			DefaultFeedbackSendingStatus();
			if (!IsSendFeedbackEnabled) return;
			var suggestion = GetImprovement(segmentId);

			//Checking for consistency: whether translation corresponds to source
			if (suggestion != null && suggestion.OriginalSource != GetSourceSegment(segmentId))
			{
				_messageBoxService.ShowWarningMessage(
					string.Format(PluginResources.SourceModifiedTextAndAdvice, PluginResources.SDLMTCloudName), PluginResources.SourceModified);

				return;
			}

			string suggestionReplacement = null;
			if (segmentId == null && suggestion != null && suggestion.Improvement == null)
			{
				suggestionReplacement = _editorController?.ActiveDocument?.ActiveSegmentPair.Target.ToString();
			}

			var rating = GetRatingObject(segmentId);

			var feedbackInfo = new FeedbackInfo
			{
				Evaluation = Evaluation,
				Rating = rating,
				SegmentId = segmentId,
				Suggestion = suggestionReplacement ?? suggestion?.Improvement,
				OriginalMtCloudTranslation = suggestion?.OriginalMtCloudTranslation
			};

			var responseMessage = await _translationService.SendFeedback(feedbackInfo);

			await FeedbackSendingStatus.ChangeStatus(responseMessage);
			OnFeedbackSendingStatusChanged();
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

		private void SetShortcutService()
		{
			if (_shortcutService == null) return;
			_shortcutService.StudioShortcutChanged -= _shortcutService_ShortcutChanged;
			_shortcutService.StudioShortcutChanged += _shortcutService_ShortcutChanged;
		}

		private void StartSendingOnConfirmationLevelChanged()
		{
			if (_segmentSupervisor == null) return;

			_segmentSupervisor.SegmentConfirmed -= OnConfirmationLevelChanged;
			_segmentSupervisor.SegmentConfirmed += OnConfirmationLevelChanged;
		}

		private void StopSendingOnConfirmationLevelChanged()
		{
			if (_segmentSupervisor == null) return;
			_segmentSupervisor.SegmentConfirmed -= OnConfirmationLevelChanged;
		}

		private void SwitchListeningForPropertyChanges(bool listen)
		{
			if (listen)
			{
				FeedbackOptions.ForEach(
					fo => fo.PropertyChanged += ResetFeedbackSendingStatus);
				PropertyChanged += ResetFeedbackSendingStatus;
			}
			else
			{
				FeedbackOptions.ForEach(fo => fo.PropertyChanged -= ResetFeedbackSendingStatus);
				PropertyChanged -= ResetFeedbackSendingStatus;
			}
		}

		private void UpdateActionTooltips()
		{
			if (_actions is null) return;
			foreach (var feedbackOption in FeedbackOptions)
			{
				var tooltipText = _shortcutService.GetShortcutDetails(feedbackOption.StudioActionId);
				SetFeedbackOptionTooltip(feedbackOption.OptionName, tooltipText);
			}
		}
	}
}