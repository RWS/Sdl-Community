using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Extensions;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
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
		private ICommand _clearCommand;
		private string _feedback;
		private IDisposable _onActiveSegmentQeChangedHandler;
		private bool _qeEnabled;
		private int _rating;
		private ICommand _sendFeedbackCommand;
		private IFeedbackService _feedbackService;

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

		private IStudioDocument ActiveDocument => _editorController.ActiveDocument;

		public Evaluations ActiveDocumentEvaluations
		{
			get
			{
				if (ActiveDocument == null)
					return null;

				var activeFileId = ActiveDocument.ActiveFile.Id;
				if (!Evaluations.ContainsKey(activeFileId))
				{
					Evaluations[activeFileId] = new Evaluations();
				}

				return Evaluations[activeFileId];
			}
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

		public ICommand ClearCommand => _clearCommand ??= new CommandHandler(ClearFeedbackBox);

		public string FeedbackMessage
		{
			get => _feedback ?? string.Empty;
			set
			{
				if (_feedback == value)
					return;
				_feedback = value;
				OnPropertyChanged(nameof(FeedbackMessage));
			}
		}

		public List<FeedbackOption> FeedbackOptions { get; set; }
		public FeedbackSendingStatus FeedbackSendingStatus { get; set; } = new();

		public bool IsSendFeedbackEnabled
		{
			get
			{
				return _feedbackService != null && _feedbackService.Settings.SendFeedback;
			}
			set
			{
				if (_feedbackService.Settings.SendFeedback == value)
					return;
				_feedbackService.Settings.SendFeedback = value;
				OnPropertyChanged(nameof(IsSendFeedbackEnabled));
			}
		}

		public bool QeEnabled
		{
			get => _qeEnabled;
			set
			{
				_qeEnabled = value;
				OnPropertyChanged(nameof(QeEnabled));
			}
		}

		public int Rating
		{
			get => _rating;
			set
			{
				if (_rating == value)
					return;
				_rating = value;

				OnPropertyChanged(nameof(Rating));
			}
		}

		public ICommand SendFeedbackCommand
			=> _sendFeedbackCommand ??= new AsyncCommand(() => SendFeedback(null));

		private SegmentId? ActiveSegmentId => ActiveDocument.ActiveSegmentPair?.Properties.Id;
		private ConcurrentDictionary<Guid, Evaluations> Evaluations { get; set; } = new();
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
				_shortcutService.StudioShortcutChanged -= ShortcutService_ShortcutChanged;
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
			if (!IsSendFeedbackEnabled)
				return;
			if (string.IsNullOrWhiteSpace(optionName))
				return;

			var option = FeedbackOptions.FirstOrDefault(fo => fo.OptionName == optionName);
			if (option != null)
			{
				option.IsChecked = !option.IsChecked;
			}
		}

		public void SetTranslationService(IFeedbackService feedbackService)
		{
			_feedbackService = feedbackService;

			ToggleSupervisingQe();

			OnPropertyChanged(nameof(IsSendFeedbackEnabled));
			AutoSendFeedback ??= _feedbackService.Settings.AutoSendFeedback;
		}

		private void ShortcutService_ShortcutChanged()
		{
			UpdateActionTooltips();
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			if (!IsSendFeedbackEnabled)
				return;
			if (AutoSendFeedback ?? false)
			{
				BackupFeedback();
			}

			ResetFeedback();
			ResetFeedbackSendingStatus(sender,
				new PropertyChangedEventArgs(nameof(ActiveDocument.ActiveSegmentChanged)));
		}

		private void AddEvaluationForCurrentSegment(string data)
		{
			if (!ActiveSegmentId.HasValue || string.IsNullOrWhiteSpace(data))
				return;

			var evaluationPerSegment = ActiveDocumentEvaluations.EvaluationPerSegment;
			if (!evaluationPerSegment.TryGetValue(ActiveSegmentId.Value, out _))
			{
				evaluationPerSegment[ActiveSegmentId.Value] = new QualityEstimation { OriginalEstimation = data };
			}
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
			ToggleSupervisingQe();

			if (ActiveDocument == null)
			{
				ResetFeedbackSendingStatus(null, null);
				return;
			}

			ResetFeedback();

			ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
		}

		/// <summary>
		/// We are doing this when the user forces the sending of the feedback(clicks on SendFeedback) without any improvement, rating or feedback message
		/// because a feedback cannot be sent without any info so we're adding the original target itself as a suggestion
		/// </summary>
		/// <param name="segmentId">When this is null the user clicked on SendFeedback instead of it being sent automatically</param>
		/// <param name="feedbackInfo">The feedbackInfo that must be validated</param>
		/// <param name="segmentPair">Segment pair to be processed</param>
		private void EnsureFeedbackWillGetThrough(SegmentId? segmentId, FeedbackInfo feedbackInfo, ISegmentPair segmentPair)
		{
			if (feedbackInfo is null || feedbackInfo.Suggestion is not null)
				return;

			var activeDocument = _editorController?.ActiveDocument;
			if (activeDocument is null)
				return;

			if (segmentId == null)
			{
				feedbackInfo.Suggestion = activeDocument.ActiveSegmentPair.Target.ToString();
			}
			else if (feedbackInfo.Rating is not null || feedbackInfo.Evaluation is not null)
			{
				feedbackInfo.Suggestion = segmentPair.Target.ToString();
			}
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
			if (score > 0)
				rating.Score = score;

			var comments = isFeedbackForPreviousSegment ? PreviousRating.Comments : GetCommentsAndFeedbackFromUi();
			if (comments?.Count > 0)
				rating.Comments = comments;

			PreviousRating.Empty();

			if (!((ExpandoObject)rating).Any())
				rating = null;
			return rating;
		}

		private string GetSourceSegment(SegmentId? segmentId)
		{
			var currentSegmentId = segmentId ?? ActiveSegmentId;
			return
				ActiveDocument.SegmentPairs.FirstOrDefault(
					sp => sp.Properties.Id == currentSegmentId && sp.GetProjectFile().Id == ActiveDocument.ActiveFile.Id)?.Source?
					.ToString();
		}

		private void Initialize()
		{
			SetShortcutService();

			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
			EditorController_ActiveDocumentChanged(null, null);

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

		private void MetadataSupervisor_ActiveSegmentQeChanged(ActiveSegmentQeChanged data)
		{
			AddEvaluationForCurrentSegment(data.Estimation);
			if (!ActiveSegmentId.HasValue)
			{
				return;
			}

			var currentEvaluation = ActiveDocumentEvaluations.EvaluationPerSegment.TryGetValue(ActiveSegmentId.Value, out var qualityEstimation);
			var isLwOrigin = ActiveDocument.ActiveSegmentPair.Properties.TranslationOrigin.OriginSystem.IsLanguageWeaverOrigin();
			qualityEstimation = isLwOrigin && currentEvaluation ? qualityEstimation : null;
			ActiveDocumentEvaluations.CurrentSegmentEvaluation = qualityEstimation;
			OnPropertyChanged(nameof(ActiveDocumentEvaluations));
		}

		private void OnFeedbackSendingStatusChanged()
		{
			OnPropertyChanged(nameof(FeedbackSendingStatus));
			SwitchListeningForPropertyChanges(true);
		}

		private async void OnShouldSendFeedback(SegmentId confirmedSegment)
		{
			if (!IsSendFeedbackEnabled)
				return;
			await SendFeedback(confirmedSegment);
		}

		private void RateItViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AutoSendFeedback))
			{
				if (_autoSendFeedback != null)
				{
					_feedbackService.Settings.AutoSendFeedback = _autoSendFeedback.Value;
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
			FeedbackOptions?.ForEach(fb => fb.IsChecked = false);
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
			if (!IsSendFeedbackEnabled)
			{
				return;
			}

			var suggestion = GetImprovement(segmentId);
			var regexPattern = @"(<(?:""[^""]*""['""]*|'[^']*'['""]*|[^'"">])+>)";
			var originalSourceText = Regex.Replace(suggestion?.OriginalSource, regexPattern, string.Empty);
			var sourceSegmentText = Regex.Replace(GetSourceSegment(segmentId), regexPattern, string.Empty);

			//Checking for consistency: whether translation corresponds to source
			if (suggestion is null
			 || originalSourceText != sourceSegmentText)
			{
				_messageBoxService.ShowWarningMessage(string.Format(PluginResources.SourceModifiedTextAndAdvice,
																	PluginResources.SDLMTCloud_Provider_Name),
													  PluginResources.SourceModified);
				return;
			}

			var segmentPairInProcessing = ActiveDocument.SegmentPairs.ToList().FirstOrDefault(sp => sp.Properties.Id.Equals(segmentId));
			var segmentSource = segmentId != null
				? segmentPairInProcessing?.Source.ToString()
				: ActiveDocument.ActiveSegmentPair.Source.ToString();

			var currentSegmentId = segmentId ?? ActiveSegmentId.Value;
			var hasEstimation = ActiveDocumentEvaluations.EvaluationPerSegment.TryGetValue(currentSegmentId, out var estimation);
			estimation = hasEstimation ? estimation.UserChoseDifferently ? estimation : null : null;

			var feedbackInfo = new FeedbackInfo
			{
				Evaluation = estimation,
				Rating = GetRatingObject(segmentId),
				SegmentSource = segmentSource,
				Suggestion = suggestion?.Improvement,
				OriginalMtCloudTranslation = suggestion?.OriginalMtCloudTranslation
			};

			EnsureFeedbackWillGetThrough(segmentId, feedbackInfo, segmentPairInProcessing);
			var responseMessage = await _feedbackService.SendFeedback(feedbackInfo);
			await FeedbackSendingStatus.ChangeStatus(responseMessage);
			OnFeedbackSendingStatusChanged();
		}

		private void SetFeedbackOptionTooltip(string mtCloudActionName, string tooltipText)
		{
			var feedBackOption =
				FeedbackOptions.FirstOrDefault(fo => fo.OptionName == mtCloudActionName);
			if (feedBackOption != null)
			{
				feedBackOption.Tooltip = tooltipText ?? PluginResources.RateItViewModel_SetOptionTooltip_No_shortcut_was_set;
			}
		}

		private void SetShortcutService()
		{
			if (_shortcutService == null)
				return;
			_shortcutService.StudioShortcutChanged -= ShortcutService_ShortcutChanged;
			_shortcutService.StudioShortcutChanged += ShortcutService_ShortcutChanged;
		}

		private void StartSendingOnConfirmationLevelChanged()
		{
			if (_segmentSupervisor == null)
				return;

			_segmentSupervisor.ShouldSendFeedback -= OnShouldSendFeedback;
			_segmentSupervisor.ShouldSendFeedback += OnShouldSendFeedback;
		}

		private void StopSendingOnConfirmationLevelChanged()
		{
			if (_segmentSupervisor == null)
				return;
			_segmentSupervisor.ShouldSendFeedback -= OnShouldSendFeedback;
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

		private void ToggleSupervisingQe(object sender = null, EventArgs e = null)
		{
			_onActiveSegmentQeChangedHandler?.Dispose();
			if (_feedbackService?.IsActiveModelQeEnabled ?? false)
			{
				QeEnabled = true;
				_onActiveSegmentQeChangedHandler = MtCloudApplicationInitializer.Subscribe<ActiveSegmentQeChanged>(MetadataSupervisor_ActiveSegmentQeChanged);

				if (ActiveSegmentId is not null)
				{
					var estimation = MtCloudApplicationInitializer.MetadataSupervisor.GetSegmentQe(ActiveSegmentId.Value);
					MetadataSupervisor_ActiveSegmentQeChanged(new ActiveSegmentQeChanged { Estimation = estimation });
				}
			}
			else
			{
				QeEnabled = false;
			}
		}

		private void UpdateActionTooltips()
		{
			if (_actions is null)
				return;
			foreach (var feedbackOption in FeedbackOptions)
			{
				var tooltipText = _shortcutService.GetShortcutDetails(feedbackOption.StudioActionId);
				SetFeedbackOptionTooltip(feedbackOption.OptionName, tooltipText);
			}
		}
	}
}