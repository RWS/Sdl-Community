using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = System.Windows.Application;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class MetadataSupervisor : IMetadataSupervisor
	{
		private readonly EditorController _editorController;
		private readonly ISegmentMetadataCreator _segmentMetadataCreator;
		private Window _batchProcessingWindow;
		private Guid _docId;
		private bool _isFirstTime = true;
		private ITranslationService _translationService;

		public MetadataSupervisor(ISegmentMetadataCreator segmentMetadataCreator, EditorController editorController)
		{
			_segmentMetadataCreator = segmentMetadataCreator;
			_editorController = editorController;

			MtCloudApplicationInitializer
				.Subscribe<RefreshQeStatus>(OnQeStatus);
		}

		private IStudioDocument ActiveDocument => _editorController?.ActiveDocument;

		public ConcurrentDictionary<SegmentId, TranslationOriginDatum> ActiveDocumentData
		{
			get
			{
				if (Data.ContainsKey(_docId)) return Data[_docId];
				SetIdAndActiveFile();

				return Data[_docId];
			}
		}

		public Dictionary<Guid, ConcurrentDictionary<SegmentId, TranslationOriginDatum>> Data { get; set; } = new();

		public void CloseOpenedDocuments()
		{
			var activeDocs = _editorController.GetDocuments().ToList();

			foreach (var activeDoc in activeDocs)
			{
				_editorController.Close(activeDoc);
			}
		}

		public void OnQeStatus(RefreshQeStatus refreshQeStatus)
		{
			var storedQe = GetCurrentSegmentStoredQe();
			SetCurrentSegmentEstimation(storedQe);
		}

		public void StartSupervising(ITranslationService translationService)
		{
			if (ActiveDocument != null)
			{
				ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
				ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
				ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
				ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
			}

			_translationService = translationService;
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= TranslationService_TranslationReceived;
				_translationService.TranslationReceived += TranslationService_TranslationReceived;
			}

			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
		}

		private static bool IsFromSdlMtCloud(ITranslationOrigin translationOrigin, bool lookInPrevious = false)
		{
			//TODO: extract in helper
			if (lookInPrevious)
			{
				return translationOrigin?.OriginBeforeAdaptation?.OriginSystem == PluginResources.SDLMTCloudName;
			}
			return translationOrigin?.OriginSystem == PluginResources.SDLMTCloudName;
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			var storedQe = GetCurrentSegmentStoredQe();
			SetCurrentSegmentEstimation(storedQe);
		}

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			var segment = (ISegment)((ISegmentContainerNode)sender).Item;
			if (segment == null) return;

			var translationOrigin = segment.Properties.TranslationOrigin;
			if (IsFromSdlMtCloud(translationOrigin))
			{
				AddToSegmentContextData();
			}
			else if (string.IsNullOrWhiteSpace(segment.ToString()))
			{
				SetCurrentSegmentEstimation(null);
			}
		}

		private void AddTargetSegmentMetaData(TranslationOriginDatum translationOriginDatum, ISegmentPair currentSegmentPair)
		{
			currentSegmentPair ??= ActiveDocument.ActiveSegmentPair;
			var segmentId = currentSegmentPair.Properties.Id;

			if (!ActiveDocumentData.TryGetValue(segmentId, out var targetData) || string.IsNullOrWhiteSpace(targetData?.QualityEstimation))
			{
				ActiveDocumentData[segmentId] = translationOriginDatum;
			}
		}

		private void AddToSegmentContextData()
		{
			var currentSegmentPair = ActiveDocument.ActiveSegmentPair;
			var translationOrigin = currentSegmentPair.Properties.TranslationOrigin;

			if (translationOrigin is null)
				return;

			var currentSegmentId = currentSegmentPair.Properties.Id;
			if (ActiveDocumentData.TryGetValue(currentSegmentId, out var targetData))
			{
				_segmentMetadataCreator.AddToCurrentSegmentContextData(ActiveDocument, targetData);
			}
			SetCurrentSegmentEstimation(targetData?.QualityEstimation);
		}

		private void AttachToClosedEvent()
		{
			_batchProcessingWindow.Closed -= Window_Closed;
			_batchProcessingWindow.Closed += Window_Closed;
			_isFirstTime = false;
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (ActiveDocument?.ActiveFile == null) return;
			SetIdAndActiveFile();
			ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;

			var activeSegmentPair = ActiveDocument.ActiveSegmentPair;
			if (activeSegmentPair is null) return;

			var currentSegmentId = activeSegmentPair.Properties.Id;
			ActiveDocumentData.TryGetValue(currentSegmentId, out var targetSegmentData);
			SetCurrentSegmentEstimation(targetSegmentData?.QualityEstimation);
		}

		private string GetCurrentSegmentStoredQe()
		{
			return ActiveDocument?.ActiveSegmentPair?.Properties.TranslationOrigin.GetMetaData("quality_estimation");
		}

		private void SetBatchProcessingWindow()
		{
			if (_batchProcessingWindow is not null) return;
			_batchProcessingWindow = Application.Current.Dispatcher.Invoke(MtCloudApplicationInitializer.GetCurrentWindow);
		}

		private void SetCurrentSegmentEstimation(string qualityEstimation)
		{
			var isActiveSegmentTranslated = !string.IsNullOrWhiteSpace(ActiveDocument?.ActiveSegmentPair?.Target.ToString());
			var estimation = isActiveSegmentTranslated ? qualityEstimation : null;
			MtCloudApplicationInitializer.PublishEvent(new ActiveSegmentQeChanged { Estimation = estimation });
		}

		private void SetIdAndActiveFile()
		{
			_docId = ActiveDocument.ActiveFile.Id;
			if (!Data.ContainsKey(_docId))
			{
				Data[_docId] = new ConcurrentDictionary<SegmentId, TranslationOriginDatum>();
			}
		}

		private void TranslationService_TranslationReceived(TranslationData translationData)
		{
			SetBatchProcessingWindow();
			if (_batchProcessingWindow is not null)
			{
				if (_isFirstTime)
				{
					AttachToClosedEvent();
				}

				_segmentMetadataCreator.AddTargetSegmentMetaData(translationData);
			}
			else if (ActiveDocument is not null)
			{
				foreach (var sourceSegment in translationData.Segments)
				{
					var currentSegmentPair =
						ActiveDocument.SegmentPairs.FirstOrDefault(
							segPair => segPair.Source.ToString() == sourceSegment.Value || segPair.Properties.Id == sourceSegment.Key);

					var translationOriginData = translationData.TranslationOriginData;
					AddTargetSegmentMetaData(
						new TranslationOriginDatum
						{
							Model = translationOriginData.Model,
							QualityEstimation = translationOriginData.QualityEstimations[currentSegmentPair.Properties.Id]
						}, currentSegmentPair);
				}
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			_isFirstTime = true;
			_batchProcessingWindow = null;
			Application.Current.Dispatcher.Invoke(CloseOpenedDocuments);
			_segmentMetadataCreator.AddToSegmentContextData();
		}
	}
}