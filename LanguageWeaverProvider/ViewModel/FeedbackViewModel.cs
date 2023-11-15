using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using Newtonsoft.Json;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.ViewModel
{
	public class FeedbackViewModel : BaseViewModel
	{
		readonly EditorController _editController;
		readonly FileBasedProject _projectController;

		ObservableCollection<ITranslationOptions> _providers;
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
			_projectController = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

			InitializeControl();
			InitializeCommands();
		}

		public bool IsCloudServiceSelected => SelectedProvider?.PluginVersion == PluginVersion.LanguageWeaverCloud;

		public bool MultipleProvidersActive => Providers?.Count > 1;

		public ObservableCollection<ITranslationOptions> Providers
		{
			get => _providers;
			set
			{
				_providers = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(MultipleProvidersActive));
			}
		}

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

		private void InitializeCommands()
		{
			SendFeedbackCommand = new RelayCommand(SendFeedback);
		}

		private void ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			InitializeControl();
		}

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

		private void ActiveSegmentChanged(object sender, EventArgs e)
		{
			if ((sender as Document).ActiveSegmentPair is not ISegmentPair segmentPair)
			{
				return;
			}

			_activeSegment = segmentPair;
			GetSegmentMetadata(_activeSegment);
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

		private void ActiveSegmentMetadataUpdated(object sender, EventArgs e)
		{
			if (_activeSegment?.Properties is null)
			{
				return;
			}

			var segmentObj = GetPropertyValue(sender, "Segment");
			var segmentPropertiesObj = GetPropertyValue(segmentObj, "Properties");
			var segmentProperties = segmentPropertiesObj as ISegmentPairProperties;
			GetSegmentMetadata(segmentProperties);
		}

		private object GetPropertyValue(object sender, string propertyName)
		{
			if (sender == null || string.IsNullOrEmpty(propertyName))
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
			if (segmentProperties is null || !IsLanguageWeaverSource(segmentProperties) || SelectedProvider is null)
			{
				MTTranslation = null;
				OriginalQE = LanguageWeaverProvider.QualityEstimations.None;
				CanSendFeedback = false;
				return;
			}

			CanSendFeedback = true;
			MTTranslation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation);
			var qualityEstimation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_QE);

			IsQeEnabled = Enum.TryParse(qualityEstimation, true, out QualityEstimations currentQE);
			OriginalQE = currentQE;
			SelectedQE = QualityEstimations.FirstOrDefault(qe => qe.Equals(currentQE));
		}

		private void SetProviderState(ISegmentPairProperties segmentProperties)
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			var segmentModel = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
			var origin = segmentProperties.TranslationOrigin.OriginBeforeAdaptation is null
				       ? segmentProperties.TranslationOrigin.OriginSystem
				       : segmentProperties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem;

			if (segmentModel is null || origin is null)
			{
				return;
			}


			Providers = new(_projectController.GetTranslationProviderConfiguration()
								  .Entries
								  .Where(entry => entry.MainTranslationProvider.Uri.AbsoluteUri.StartsWith(Constants.BaseTranslationScheme))
								  .Select(entry => JsonConvert.DeserializeObject<TranslationOptions>(entry.MainTranslationProvider.State)));
			var versionToSearch = origin.StartsWith(Constants.PluginNameCloud)
								? PluginVersion.LanguageWeaverCloud
								: origin.StartsWith(Constants.PluginNameEdge)
								? PluginVersion.LanguageWeaverEdge
								: PluginVersion.None;

			var matchingProviders = Providers.Where(provider => provider.PluginVersion == versionToSearch);
			SelectedProvider = matchingProviders.FirstOrDefault(provider => provider.PairMappings.Any(pairMapping => pairMapping.SelectedModel.Name.Equals(segmentModel)));
		}

		private async void SendFeedback(object parameter)
		{
			if (parameter is not string)
			{
				return;
			}

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

			var mappedPair = SelectedProvider.PairMappings.FirstOrDefault(x => x.LanguagePair.TargetCultureName.Equals(_editController.ActiveDocument.ActiveFile.Language.CultureInfo.Name));

			var translation = new Translation()
			{
				SourceLanguageId = mappedPair.SourceCode,
				TargetLanguageId = mappedPair.TargetCode,
				Model = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName),
				SourceText = segmentPair.Source.ToString(),
				TargetMTText = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation)
							?? segmentPair.Target.ToString(),
				QualityEstimationMT = IsQeEnabled ? OriginalQE.ToString() : null
			};

			var improvement = new Improvement()
			{
				Text = segmentPair.Target.ToString()
			};

			var rating = new Rating()
			{
				Comments = new List<string>() { string.IsNullOrEmpty(FeedbackMessage) ? string.Empty : FeedbackMessage }
			};

			rating.Comments.AddRange(TranslationErrors.GetProblemsReported());

			var feedbackRequest = new FeedbackRequest()
			{
				Translation = translation,
				Improvement = IsAdapted(segmentPair.Properties) ? improvement : null,
				Rating = rating,
				QualityEstimation = IsQeEnabled ? SelectedQE.ToString() : null
			};

			CredentialManager.ValidateToken(SelectedProvider);
			await CloudService.CreateFeedback(SelectedProvider.AccessToken, feedbackRequest);
		}

		private async Task SendEdgeFeedback(ISegmentPair segmentPair)
		{
			if (!IsLanguageWeaverSource(segmentPair?.Properties))
			{
				return;
			}

			var mappedPair = SelectedProvider.PairMappings.FirstOrDefault(x => x.LanguagePair.TargetCultureName.Equals(_editController.ActiveDocument.ActiveFile.Language.CultureInfo.Name));
			var feedbackDictionary = new Dictionary<string, string>
			{
				["sourceText"] = segmentPair.Source.ToString(),
				["languagePairId"] = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName),
				["machineTranslation"] = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation)
			};

			if (IsAdapted(segmentPair.Properties))
			{
				feedbackDictionary["suggestedTranslation"] = segmentPair.Target.ToString();
			}

			await EdgeService.SendFeedback(SelectedProvider.AccessToken, feedbackDictionary);
		}

		private bool IsLanguageWeaverSource(ISegmentPairProperties segmentProperties)
		{
			if (segmentProperties?.TranslationOrigin is not ITranslationOrigin translationOrigin)
			{
				return false;
			}

			var originSystem = translationOrigin.OriginBeforeAdaptation is null ? translationOrigin.OriginSystem : translationOrigin.OriginBeforeAdaptation.OriginSystem;
			return originSystem is not null
				&& originSystem.ToLower().Contains(Constants.PluginShortName.ToLower());
		}

		private bool IsAdapted(ISegmentPairProperties segmentProperties)
		{
			var translationOrigin = segmentProperties?.TranslationOrigin;
			var translationOriginBeforeAdaption = translationOrigin?.OriginBeforeAdaptation;
			return translationOrigin is not null
				&& translationOriginBeforeAdaption is not null
				&& translationOriginBeforeAdaption.OriginSystem.ToLower().Contains(Constants.PluginShortName.ToLower());
		}

		private bool IsQEEnabled(ISegmentPairProperties segmentProperties)
		{
			var nmtModel = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName);
			return nmtModel is not null
				&& nmtModel.ToLower().Contains("qe");
		}
	}
}