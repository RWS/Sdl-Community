using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.ViewModel
{
	public class FeedbackViewModel : BaseViewModel
	{
		readonly EditorController _editController;
		ITranslationOptions _selectedProvider;
		TranslationErrors _translationErrors;

		ISegmentPair _activeSegment;
		QualityEstimations _originalQE;
		QualityEstimations _selectedQE;
		string _feedbackMessage;
		string _mtTranslation;
		bool _canSendFeedback;
		bool _isQeEnabled;

		public FeedbackViewModel()
		{
			_editController = SdlTradosStudio.Application.GetController<EditorController>();
			_editController.ActiveDocumentChanged += ActiveDocumentChanged;
			SendFeedbackCommand = new RelayCommand(SendFeedback);

			InitializeControl();
		}

		public bool IsCloudServiceSelected => SelectedProvider?.PluginVersion == PluginVersion.LanguageWeaverCloud;

		public ITranslationOptions SelectedProvider
		{
			get => _selectedProvider;
			set
			{
				_selectedProvider = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsCloudServiceSelected));
			}
		}

		public TranslationErrors TranslationErrors
		{
			get => _translationErrors ??= new();
			set
			{
				_translationErrors = value;
				OnPropertyChanged();
			}
		}

		public IEnumerable<QualityEstimations> QualityEstimations => Enum.GetValues(typeof(QualityEstimations)).Cast<QualityEstimations>().Skip(1);

		public QualityEstimations OriginalQE
		{
			get => _originalQE;
			set
			{
				_originalQE = value;
				OnPropertyChanged();
			}
		}

		public QualityEstimations SelectedQE
		{
			get => _selectedQE;
			set
			{
				_selectedQE = value;
				OnPropertyChanged();
			}
		}

		public string FeedbackMessage
		{
			get => _feedbackMessage;
			set
			{
				_feedbackMessage = value;
				OnPropertyChanged();
			}
		}

		public bool CanSendFeedback
		{
			get => _canSendFeedback;
			set
			{
				_canSendFeedback = value;
				OnPropertyChanged();
			}
		}

		public string MTTranslation
		{
			get => _mtTranslation;
			set
			{
				_mtTranslation = value;
				OnPropertyChanged();
			}
		}

		public bool IsQeEnabled
		{
			get => _isQeEnabled;
			set
			{
				_isQeEnabled = value;
				OnPropertyChanged();
			}
		}

		public ICommand SendFeedbackCommand { get; private set; }

		private void InitializeControl()
		{
			if (_editController.ActiveDocument is null)
			{
				return;
			}

			_editController.ActiveDocument.ActiveSegmentChanged += ActiveSegmentChanged;
			_editController.ActiveDocument.SegmentsTranslationOriginChanged += ActiveSegmentMetadataUpdated;
			_editController.ActiveDocument.SegmentsConfirmationLevelChanged += SegmentConfirmationLevelChanged;
			_activeSegment = _editController.ActiveDocument.GetActiveSegmentPair();
			GetSegmentMetadata(_activeSegment);
		}

		private void ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			InitializeControl();
		}

		private void ActiveSegmentChanged(object sender, EventArgs e)
		{
			if ((sender as Document).ActiveSegmentPair is not ISegmentPair segmentPair)
			{
				return;
			}

			_activeSegment = segmentPair;
			TranslationErrors.ResetValues();
			GetSegmentMetadata(_activeSegment);
		}

		private void ActiveSegmentMetadataUpdated(object sender, EventArgs e)
		{
			if (_activeSegment?.Properties is null)
			{
				return;
			}

			var segmentObj = GetPropertyValue(sender, "Segment");
			var segmentPropertiesObj = GetPropertyValue(segmentObj, "Properties");
			var segmentProperties = segmentPropertiesObj as ISegmentPairProperties;
			if (IsCurrentTranslationLanguageWeaverSource(segmentProperties)
			 && segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation) is null
			 && _activeSegment is not null)
			{
				SetSavedSegmentMetadata(segmentProperties);
			}

			GetSegmentMetadata(segmentProperties);
		}

		private async void SegmentConfirmationLevelChanged(object sender, EventArgs e)
		{
			var segmentObj = GetPropertyValue(sender, "Segment");
			var segmentPropertiesObj = GetPropertyValue(segmentObj, "Properties");
			var segmentProperties = segmentPropertiesObj as ISegmentPairProperties;
			if (segmentProperties.ConfirmationLevel != ConfirmationLevel.Translated
			 || !IsLanguageWeaverSource(segmentProperties))
			{
				return;
			}

			var currentSegment = _editController.ActiveDocument.SegmentPairs.FirstOrDefault(x => x.Properties.Id.Id.Equals(segmentProperties.Id.Id));
			if (!bool.TryParse(currentSegment.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Feedback), out var autosendFeedback)
			 || !autosendFeedback)
			{
				return;
			}

			GetSegmentMetadata(currentSegment.Properties);
			if (SelectedProvider?.PluginVersion == PluginVersion.LanguageWeaverCloud)
			{
				await SendCloudFeedback(currentSegment);
			}
			else if (SelectedProvider?.PluginVersion == PluginVersion.LanguageWeaverEdge)
			{
				await SendEdgeFeedback(currentSegment);
			}
		}

		private object GetPropertyValue(object sender, string propertyName)
		{
			if (sender is null)
			{
				return null;
			}

			var propertyObject = sender.GetType().GetProperty(propertyName);
			return propertyObject?.GetValue(sender, null);
		}

		private void GetSegmentMetadata(ISegmentPair segment)
		{
			if (segment is null)
			{
				return;
			}

			var segmentProperties = segment.Properties;
			GetSegmentMetadata(segmentProperties);
		}

		private void GetSegmentMetadata(ISegmentPairProperties segmentProperties)
		{
			SetProviderState(segmentProperties);
			CanSendFeedback = segmentProperties is not null && SelectedProvider is not null && IsLanguageWeaverSource(segmentProperties);

			if (!CanSendFeedback)
			{
				MTTranslation = null;
				OriginalQE = LanguageWeaverProvider.QualityEstimations.None;
				return;
			}

			var qualityEstimation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_QE);

			IsQeEnabled = Enum.TryParse(qualityEstimation, true, out QualityEstimations currentQE);
			OriginalQE = currentQE;
			SelectedQE = QualityEstimations.FirstOrDefault(qe => qe.Equals(currentQE));
			MTTranslation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation);
		}

		private void SetSavedSegmentMetadata(ISegmentPairProperties segmentProperties)
		{
			var currentSegmentId = segmentProperties.Id;
			var documentSegmentPair = _editController.ActiveDocument.SegmentPairs.FirstOrDefault(sp => sp.Properties.Id == currentSegmentId);
			var ratedSegment = ApplicationInitializer.RatedSegments.FirstOrDefault(rs => rs.SegmentId.Equals(currentSegmentId));

			if (documentSegmentPair is null || ratedSegment is null)
			{
				return;
			}

			documentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_QE, ratedSegment.QualityEstimation);
			documentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_LongModelName, ratedSegment.ModelName);
			documentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_ShortModelName, ratedSegment.Model);
			documentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_Translation, ratedSegment.Translation);
			documentSegmentPair.Properties.TranslationOrigin.SetMetaData(Constants.SegmentMetadata_Feedback, ratedSegment.AutosendFeedback.ToString());
			_editController.ActiveDocument.UpdateSegmentPairProperties(documentSegmentPair, documentSegmentPair.Properties);

			ApplicationInitializer.RatedSegments.Remove(ratedSegment);
		}

		private void SetProviderState(ISegmentPairProperties segmentProperties)
		{
			var segmentModel = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
			var pluginVersion = GetPluginVersion(segmentProperties);

			if (segmentModel is null || pluginVersion == PluginVersion.None)
			{
				return;
			}

			var translationOptions = new TranslationOptions() { PluginVersion = pluginVersion };
			CredentialManager.GetCredentials(translationOptions, true);
			SelectedProvider = translationOptions;
		}

		private async void SendFeedback(object parameter)
		{
			_activeSegment = _editController?.ActiveDocument?.ActiveSegmentPair;
			SetProviderState(_activeSegment.Properties);

			if (SelectedProvider.PluginVersion == PluginVersion.LanguageWeaverCloud)
			{
				await SendCloudFeedback(_activeSegment);
			}
			else if (SelectedProvider.PluginVersion == PluginVersion.LanguageWeaverEdge)
			{
				await SendEdgeFeedback(_activeSegment);
			}
		}

		private async Task SendCloudFeedback(ISegmentPair segmentPair)
		{
			if (!IsLanguageWeaverSource(segmentPair?.Properties))
			{
				return;
			}

			var modelName = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
			var sourceCode = modelName.Substring(0, 3);
			var targetCode = modelName.Substring(3, 3);

			var translation = new Translation()
			{
				SourceLanguageId = sourceCode,
				TargetLanguageId = targetCode,
				Model = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName),
				SourceText = segmentPair.Source.ToString(),
				TargetMTText = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation),
				QualityEstimationMT = IsQeEnabled ? OriginalQE.ToString() : null
			};

			var isAdapted = IsAdapted(segmentPair.Properties);
			var improvement = new Improvement(segmentPair.Target.ToString());
			var rating = new Rating(FeedbackMessage, TranslationErrors);
			var feedbackRequest = new FeedbackRequest()
			{
				Translation = translation,
				Improvement = isAdapted ? improvement : null,
				Rating = rating,
				QualityEstimation = IsQeEnabled ? SelectedQE.ToString() : null
			};

			Service.ValidateToken(SelectedProvider);
			await CloudService.CreateFeedback(SelectedProvider.AccessToken, feedbackRequest);
		}

		private async Task SendEdgeFeedback(ISegmentPair segmentPair)
		{
			if (!IsLanguageWeaverSource(segmentPair?.Properties))
			{
				return;
			}

			var feedback = new List<KeyValuePair<string, string>>
			{
				new("sourceText", segmentPair.Source.ToString()),
				new("languagePairId", segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName)),
				new("machineTranslation", segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation))
			};

			if (IsAdapted(segmentPair.Properties))
			{
				feedback.Add(new("suggestedTranslation", segmentPair.Target.ToString()));
			}

			await EdgeService.SendFeedback(SelectedProvider.AccessToken, feedback);
		}

		private bool IsLanguageWeaverSource(ISegmentPairProperties segmentProperties)
		{
			return IsCurrentTranslationLanguageWeaverSource(segmentProperties)
				|| IsAdapted(segmentProperties);
		}

		private bool IsAdapted(ISegmentPairProperties segmentProperties)
		{
			return segmentProperties.TranslationOrigin?.OriginBeforeAdaptation?.OriginSystem?.StartsWith(Constants.PluginShortName) == true;
		}

		private bool IsCurrentTranslationLanguageWeaverSource(ISegmentPairProperties segmentProperties)
		{
			return segmentProperties.TranslationOrigin?.OriginSystem?.StartsWith(Constants.PluginShortName) == true;
		}

		private PluginVersion GetPluginVersion(ISegmentPairProperties segmentProperties)
		{
			var origin = true switch
			{
				var _ when IsCurrentTranslationLanguageWeaverSource(segmentProperties) => segmentProperties.TranslationOrigin.OriginSystem,
				var _ when IsAdapted(segmentProperties) => segmentProperties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem,
				var _ => string.Empty,
			};

			return origin switch
			{
				Constants.PluginNameCloud => PluginVersion.LanguageWeaverCloud,
				Constants.PluginNameEdge => PluginVersion.LanguageWeaverEdge,
				_ => PluginVersion.None,
			};
		}
	}
}