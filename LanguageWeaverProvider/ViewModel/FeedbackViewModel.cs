using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
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

		ISegmentPair _activeSegment;

		QualityEstimations _originalQE;
		QualityEstimations _selectedQE;
		string _feedbackMessage;
		string _mtTranslation;
		int _rating;

		public FeedbackViewModel()
		{
			_editController = SdlTradosStudio.Application.GetController<EditorController>();
			_editController.ActiveDocumentChanged += ActiveDocumentChanged;
			_projectController = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			InitializeControl();
			InitializeCommands();
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

		public string MTTranslation
		{
			get => _mtTranslation;
			set
			{
				_mtTranslation = value;
				OnPropertyChanged();
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
			if (_editController.ActiveDocument is null)
			{
				return;
			}

			_editController.ActiveDocument.ActiveSegmentChanged += ActiveSegmentChanged;
			_editController.ActiveDocument.SegmentsTranslationOriginChanged += ActiveSegmentMetadataUpdated;
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
				return;
			}

			MTTranslation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation);
			var qualityEstimation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_QE);
			if (!Enum.TryParse(qualityEstimation, true, out QualityEstimations currentQE))
			{
				return;
			}

			OriginalQE = currentQE;
			SelectedQE = QualityEstimations.FirstOrDefault(x => x == OriginalQE);
			Rating = SelectedQE switch
			{
				LanguageWeaverProvider.QualityEstimations.Poor => 2,
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

			var nmtModelName = _activeSegment.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(providerState);
			var mappedPair = translationOptions.PairMappings.FirstOrDefault(x => x.SelectedModel.Name.Equals(nmtModelName));

			var translation = new Translation()
			{
				SourceLanguageId = mappedPair.SourceCode,
				TargetLanguageId = mappedPair.TargetCode,
				Model = mappedPair.SelectedModel.Model,
				SourceText = _activeSegment.Source.ToString(),
				TargetMTText = _activeSegment.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation)
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
				Rating = rating
			};

			await CloudService.CreateFeedback(translationOptions.CloudCredentials, feedbackRequest);
		}

		private bool IsLanguageWeaverSource(ISegmentPairProperties segmentProperties)
		{
			if (segmentProperties?.TranslationOrigin is not ITranslationOrigin translationOrigin)
			{
				return false;
			}

			var originSystem = translationOrigin.OriginBeforeAdaptation is null ? translationOrigin.OriginSystem : translationOrigin.OriginBeforeAdaptation.OriginSystem;
			return string.Equals(originSystem, Constants.PluginName, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsAdapted(ISegmentPairProperties segmentProperties)
		{
			var translationOrigin = segmentProperties?.TranslationOrigin;
			var translationOriginBeforeAdaption = translationOrigin?.OriginBeforeAdaptation;
			return translationOrigin is not null
				&& translationOriginBeforeAdaption is not null
				&& string.Equals(translationOriginBeforeAdaption.OriginSystem, Constants.PluginName, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsQEEnabled(ISegmentPairProperties segmentProperties)
		{
			var nmtModel = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
			return nmtModel is not null && nmtModel.ToLower().Contains("qe");
		}
	}
}