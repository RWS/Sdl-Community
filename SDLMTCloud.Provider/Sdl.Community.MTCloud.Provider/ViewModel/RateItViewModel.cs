using System;
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

			_rating = 0;
		}

		private void SetSegmentSupervisor()
		{
			if (_segmentSupervisor == null) return;

			_segmentSupervisor.ConfirmationLevelChanged -= OnConfirmationLevelChanged;
			_segmentSupervisor.ConfirmationLevelChanged += OnConfirmationLevelChanged;
		}

		private void ResetSegmentSupervisor()
		{
			if (_segmentSupervisor == null) return;
			_segmentSupervisor.ConfirmationLevelChanged -= OnConfirmationLevelChanged;
		}

		private async void OnConfirmationLevelChanged(SegmentId confirmedSegment)
		{
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
					StopSegmentSupervisor();
					ResetFeedbackOptions();
				}
				else
				{
					_segmentSupervisor.StartSupervising();
				}

				_isSendFeedbackEnabled = value;
				OnPropertyChanged(nameof(IsSendFeedbackEnabled));
			}
		}

		private void ResetFeedbackOptions()
		{
			ResetFeedback();
			AutoSendFeedback = false;
		}

		private void StopSegmentSupervisor()
		{
			_segmentSupervisor?.StopSupervising();
			ResetSegmentSupervisor();
		}

		public string Feedback
		{
			get => _feedback ?? string.Empty;
			set
			{
				if (_feedback == value) return;
				_feedback = value;
				OnPropertyChanged(nameof(Feedback));
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
			var segment = segmentId ?? ActiveSegmentId;

			if (!_segmentSupervisor.ActiveDocumentImprovements.ContainsKey(segment))
			{
				_messageBoxService.ShowWarningMessage(
					string.Format(PluginResources.OriginalTranslationMissingMessage, PluginResources.SDLMTCloudName,
						PluginResources.SDLMTCloudName), PluginResources.OriginalTranslationMissingTitle);

				return;
			}

			var improvement = _segmentSupervisor.ActiveDocumentImprovements[segment];

			if (_translationService != null)
			{
				var comments = new List<string>();
				if (!string.IsNullOrWhiteSpace(Feedback))
				{
					comments.Add(Feedback);
				}
				comments.AddRange(FeedbackOptions.Where(fo => fo.IsChecked).Select(fo => fo.OptionName).ToList());

				dynamic rating = new ExpandoObject();
				if (_rating > 0) rating.Score = _rating;
				if (comments.Count > 0) rating.Comments = comments;
				if (!((ExpandoObject)rating).Any()) rating = null;

				if (rating != null)
				{
					improvement.Score = _rating;
					improvement.Comments = comments;
				}

				if (string.IsNullOrWhiteSpace(improvement.Improvement) && rating == null) return;
				await _translationService.SendFeedback(segmentId, rating, improvement.OriginalTarget, improvement.Improvement);
			}
			else
			{
				//TODO: Log that translation service is null
			}

			ResetFeedback();
		}

		private void ResetFeedback()
		{
			FeedbackOptions.ForEach(fb => fb.IsChecked = false);
			Feedback = string.Empty;
			Rating = 0;
		}

		private void ClearFeedbackBox(object obj)
		{
			Feedback = string.Empty;
		}

		private void _translationService_TranslationReceived(FeedbackRequest translationFeedback)
		{

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

		private SegmentId ActiveSegmentId => _editorController.ActiveDocument.ActiveSegmentPair.Properties.Id;

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			_editorController.ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			_editorController.ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			ResetFeedback();
			ImprovedTarget improvement = null;

			if (_segmentSupervisor.ActiveDocumentImprovements.ContainsKey(ActiveSegmentId))
			{
				improvement = _segmentSupervisor.ActiveDocumentImprovements[ActiveSegmentId];
			}

			if (improvement == null) return;

			var comments = improvement.Comments?.Select(c => (string)c.Clone()).ToList();

			if (comments != null)
			{
				Feedback = comments.FirstOrDefault(c => FeedbackOptions.All(fo => fo.OptionName != c));
				comments.Remove(Feedback);

				foreach (var feedbackOption in comments.SelectMany(c => FeedbackOptions.Where(fo => fo.OptionName == c)))
				{
					feedbackOption.IsChecked = true;
				}
			}

			Rating = improvement.Score;
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
	}
}
