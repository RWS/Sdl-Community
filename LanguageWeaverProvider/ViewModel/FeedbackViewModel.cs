using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
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

		ISegmentPair _activeSegment;
		QualityEstimations _originalQE;
		QualityEstimations _selectedQE;
		string _feedbackMessage;
		string _mtTranslation;
		bool _canSendFeedback;
		int _rating;

		public FeedbackViewModel()
		{
			_editController = SdlTradosStudio.Application.GetController<EditorController>();
			_editController.ActiveDocumentChanged += ActiveDocumentChanged;
			_projectController = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

			InitializeControl();
			InitializeCommands();
		}

		public bool IsCloudServiceSelected => SelectedProvider?.Version == PluginVersion.LanguageWeaverCloud;

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
				CanSendFeedback = value is not null;
			}
		}

		public int Rating
		{
			get => _rating;
			set
			{
				_rating = value;
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
			SetTranslationOptions();
			if (_editController.ActiveDocument is null)
			{
				return;
			}

			_editController.ActiveDocument.ActiveSegmentChanged += ActiveSegmentChanged;
			_editController.ActiveDocument.SegmentsTranslationOriginChanged += ActiveSegmentMetadataUpdated;
			_activeSegment = _editController.ActiveDocument.GetActiveSegmentPair();
			GetSegmentMetadata(_activeSegment);
		}

		private void SetTranslationOptions()
		{
			Providers = new(_projectController.GetTranslationProviderConfiguration()
											  .Entries
											  .Where(entry => entry.MainTranslationProvider.Uri.AbsoluteUri.StartsWith(Constants.TranslationScheme))
											  .Select(entry => JsonConvert.DeserializeObject<TranslationOptions>(entry.MainTranslationProvider.State)));
			SelectedProvider = Providers.First();
		}

		private void ActiveSegmentChanged(object sender, EventArgs e)
		{
			if ((sender as Document).ActiveSegmentPair is not ISegmentPair segmentPair)
			{
				return;
			}

			SetTranslationOptions();
			_activeSegment = segmentPair;
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
			if (segmentProperties is null || !IsLanguageWeaverSource(segmentProperties) || !IsQEEnabled(segmentProperties))
			{
				MTTranslation = null;
				OriginalQE = LanguageWeaverProvider.QualityEstimations.None;
				return;
			}

			MTTranslation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation);
			var qualityEstimation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_QE);
			if (!Enum.TryParse(qualityEstimation, true, out QualityEstimations currentQE))
			{
				return;
			}

			OriginalQE = currentQE;
			SelectedQE = QualityEstimations.FirstOrDefault(qe => qe.Equals(currentQE));
			Rating = SelectedQE switch
			{
				LanguageWeaverProvider.QualityEstimations.Poor => 1,
				LanguageWeaverProvider.QualityEstimations.Good => 5,
				_ => 3
			};
		}

		private async void SendFeedback(object parameter)
		{
			_activeSegment = _editController?.ActiveDocument?.ActiveSegmentPair;
			if (!IsLanguageWeaverSource(_activeSegment?.Properties)
			 || !IsQEEnabled(_activeSegment?.Properties))
			{
				return;
			}

			var providerState = _projectController
				.GetTranslationProviderConfiguration()
				.Entries
				.FirstOrDefault(x => x.MainTranslationProvider.Uri.AbsoluteUri.Equals(Constants.TranslationFullScheme))
				.MainTranslationProvider
				.State;

			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(providerState);
			var mappedPair = translationOptions.PairMappings.FirstOrDefault(x => x.LanguagePair.TargetCultureName.Equals(_editController.ActiveDocument.ActiveFile.Language.CultureInfo.Name));

			var translation = new Translation()
			{
				SourceLanguageId = mappedPair.SourceCode,
				TargetLanguageId = mappedPair.TargetCode,
				Model = _activeSegment.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName),
				SourceText = _activeSegment.Source.ToString(),
				TargetMTText = _activeSegment.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation)
							?? _activeSegment.Target.ToString(),
				QualityEstimationMT = OriginalQE.ToString()
			};

			var improvement = new Improvement()
			{
				Text = _activeSegment.Target.ToString()
			};

			var rating = new Rating()
			{
				Score = _rating.ToString(),
				Comments = new List<string>() { string.IsNullOrEmpty(FeedbackMessage) ? string.Empty : FeedbackMessage }
			};

			var feedbackRequest = new FeedbackRequest()
			{
				Translation = translation,
				Improvement = IsAdapted(_activeSegment.Properties) ? improvement : null,
				Rating = rating,
				QualityEstimation = SelectedQE.ToString()
			};

			CredentialManager.ValidateToken(translationOptions);
			await CloudService.CreateFeedback(translationOptions.AccessToken, feedbackRequest);
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