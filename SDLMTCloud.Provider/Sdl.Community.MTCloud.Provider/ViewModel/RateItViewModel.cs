﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel : BaseViewModel, IRatingService, IDisposable
	{
		private readonly IShortcutService _shortcutService;
		private ITranslationService _translationService;
		private readonly IActionProvider _actionProvider;
		private readonly ISegmentSupervisor _segmentSupervisor;
		private readonly IMessageBoxService _messageBoxService;
		private readonly EditorController _editorController;
		private List<ISDLMTCloudAction> _actions;
		private ICommand _sendFeedbackCommand;

		private int _rating;
		private string _feedback;
		private bool _isSendFeedbackEnabled;
		private bool _autoSendFeedback;
		private ICommand _clearCommand;

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

		private void SetSegmentSupervisor()
		{
			if (_segmentSupervisor == null) return;

			_segmentSupervisor.SegmentConfirmed -= OnConfirmationLevelChanged;
			_segmentSupervisor.SegmentConfirmed += OnConfirmationLevelChanged;
		}

		private void ResetSegmentSupervisor()
		{
			if (_segmentSupervisor == null) return;
			_segmentSupervisor.SegmentConfirmed -= OnConfirmationLevelChanged;
		}

		private async void OnConfirmationLevelChanged(SegmentId confirmedSegment)
		{
			if (!IsSendFeedbackEnabled) return;
			await SendFeedbackToService(confirmedSegment);
		}

		public List<FeedbackOption> FeedbackOptions { get; set; }

		public bool AutoSendFeedback
		{
			get => _autoSendFeedback;
			set
			{
				_autoSendFeedback = value;
				if (value)
				{
					SetSegmentSupervisor();
				}
				else
				{
					ResetSegmentSupervisor();
				}

				OnPropertyChanged(nameof(AutoSendFeedback));
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

		public bool IsSendFeedbackEnabled
		{
			get => _isSendFeedbackEnabled;
			set
			{
				if (_isSendFeedbackEnabled == value) return;
				if (!value)
				{
					ResetRateIt();
				}

				_isSendFeedbackEnabled = value;
				OnPropertyChanged(nameof(IsSendFeedbackEnabled));
			}
		}

		private void ResetRateIt()
		{
			ResetFeedback();
			AutoSendFeedback = false;
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

		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(ClearFeedbackBox));

		public ICommand SendFeedbackCommand
			=> _sendFeedbackCommand ?? (_sendFeedbackCommand = new AsyncCommand(() => SendFeedbackToService()));

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
			if (!IsSendFeedbackEnabled) return;
			if (string.IsNullOrWhiteSpace(optionName)) return;

			var option = FeedbackOptions.FirstOrDefault(fo => fo.OptionName == optionName);
			if (option != null)
			{
				option.IsChecked = !option.IsChecked;
			}
		}

		private async Task SendFeedbackToService(SegmentId? segmentId = null)
		{
			if (!IsSendFeedbackEnabled) return;
			var improvement = GetImprovement(segmentId);
			if (improvement == null)
			{
				_messageBoxService.ShowWarningMessage(
					string.Format(PluginResources.OriginalTranslationMissingMessage, PluginResources.SDLMTCloudName,
						PluginResources.SDLMTCloudName), PluginResources.OriginalTranslationMissingTitle);

				return;
			}

			//Checking for consistency: whether translation corresponds to source
			if (improvement.OriginalSource !=
				GetSourceSegment(segmentId))
			{
				_messageBoxService.ShowWarningMessage(
					string.Format(PluginResources.SourceModifiedTextAndAdvice, PluginResources.SDLMTCloudName),
					PluginResources.SourceModified);

				return;
			}

			var rating = GetRatingObject(segmentId);
			if (rating != null)
			{
				UpdateImprovement(improvement, rating);
			}

			if (string.IsNullOrWhiteSpace(improvement.Improvement) && rating == null) return;

			await _translationService.SendFeedback(segmentId, rating, improvement.OriginalTarget, improvement.Improvement);
		}

		private dynamic GetRatingObject(SegmentId? segmentId)
		{
			dynamic rating = new ExpandoObject();

			var isFeedbackForPreviousSegment = AutoSendFeedback && segmentId != null && segmentId != ActiveSegmentId;

			var score = isFeedbackForPreviousSegment ? PreviousRating.Score : _rating;
			if (score > 0) rating.Score = score;

			var comments = isFeedbackForPreviousSegment ? PreviousRating.Comments : GetCommentsAndFeedbackFromUi();
			if (comments.Count > 0) rating.Comments = comments;

			PreviousRating = null;

			if (!((ExpandoObject)rating).Any()) rating = null;
			return rating;
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

		private string GetSourceSegment(SegmentId? segmentId)
		{
			var currentSegmentId = segmentId ?? ActiveSegmentId;
			return _editorController.ActiveDocument.SegmentPairs.FirstOrDefault(sp => sp.Properties.Id == currentSegmentId)?.Source?
				.ToString();
		}

		private void UpdateImprovement(ImprovedTarget improvement, dynamic rating)
		{
			if (!(rating is IDictionary<string, object> ratingAsDictionary)) return;

			if (ratingAsDictionary.ContainsKey("Score"))
				improvement.Score = rating.Score;
			if (ratingAsDictionary.ContainsKey("Comments"))
				improvement.Comments = rating.Comments;
		}

		private void ResetFeedback()
		{
			FeedbackOptions.ForEach(fb => fb.IsChecked = false);
			FeedbackMessage = string.Empty;
			Rating = 0;
		}

		private void ClearFeedbackBox(object obj)
		{
			FeedbackMessage = string.Empty;
		}

		private void SetShortcutService()
		{
			if (_shortcutService == null) return;
			_shortcutService.StudioShortcutChanged -= _shortcutService_ShortcutChanged;
			_shortcutService.StudioShortcutChanged += _shortcutService_ShortcutChanged;
		}

		private void _shortcutService_ShortcutChanged()
		{
			UpdateActionTooltips();
		}

		private void Initialize()
		{
			SetShortcutService();

			_segmentSupervisor.StartSupervising();
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			_actions = _actionProvider.GetActions();
			var feedbackOptions = _actions.Where(action => IsFeedbackOption(action.GetType().Name));

			FeedbackOptions = new List<FeedbackOption>();
			foreach (var feedbackOption in feedbackOptions)
			{
				FeedbackOptions.Add(new FeedbackOption
				{
					OptionName = feedbackOption.Text,
					StudioActionId = feedbackOption.Id
				});
			}
		}

		private SegmentId? ActiveSegmentId => _editorController.ActiveDocument.ActiveSegmentPair?.Properties.Id;

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (_editorController.ActiveDocument == null) return;
			_editorController.ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			_editorController.ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			if (!IsSendFeedbackEnabled) return;
			if (AutoSendFeedback)
			{
				BackupFeedback();
			}

			SetActiveSegmentFeedbackInfo();
		}

		private void BackupFeedback()
		{
			PreviousRating = new Rating
			{
				Score = Rating,
				Comments = GetCommentsAndFeedbackFromUi()
			};
		}

		private Rating PreviousRating { get; set; }

		private void SetActiveSegmentFeedbackInfo()
		{
			var improvement = GetImprovement();
			var comments = improvement?.Comments?.Select(c => (string)c.Clone()).ToList();

			FeedbackOptions.ForEach(fb => fb.IsChecked = false);
			FeedbackMessage = string.Empty;
			if (comments != null)
			{
				FeedbackMessage = comments.FirstOrDefault(c => FeedbackOptions.All(fo => fo.OptionName != c));
				comments.Remove(FeedbackMessage);

				foreach (var feedbackOption in comments.SelectMany(c => FeedbackOptions.Where(fo => fo.OptionName == c)))
				{
					feedbackOption.IsChecked = true;
				}
			}

			Rating = improvement?.Score ?? 0;
		}

		private ImprovedTarget GetImprovement(SegmentId? segmentId = null)
		{
			var currentSegment = segmentId ?? ActiveSegmentId;
			ImprovedTarget improvement = null;
			if (currentSegment != null && _segmentSupervisor.ActiveDocumentImprovements.ContainsKey(currentSegment.Value))
			{
				improvement = _segmentSupervisor.ActiveDocumentImprovements[currentSegment.Value];
			}
			return improvement;
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
			if (_shortcutService != null)
			{
				_shortcutService.StudioShortcutChanged -= _shortcutService_ShortcutChanged;
			}
		}

		public void SetTranslationService(ITranslationService translationService)
		{
			_translationService = translationService;
		}
	}
}
