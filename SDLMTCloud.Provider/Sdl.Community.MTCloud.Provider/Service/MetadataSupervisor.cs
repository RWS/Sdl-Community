using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = System.Windows.Application;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class MetadataSupervisor : IMetadataSupervisor
	{
		private ITranslationService _translationService;
		private readonly ISegmentMetadataCreator _segmentMetadataCreator;
		private Guid _docId;
		private readonly EditorController _editorController;
		private bool _isFirstTime = true;
		private Window _batchProcessingWindow;

		public MetadataSupervisor(ISegmentMetadataCreator segmentMetadataCreator, EditorController editorController)
		{
			_segmentMetadataCreator = segmentMetadataCreator;
			_editorController = editorController;
		}

		public void StartSupervising(ITranslationService translationService)
		{
			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;

			if (ActiveDocument != null)
			{
				ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
				ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			}

			_translationService = translationService;
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= TranslationService_TranslationReceived;
				_translationService.TranslationReceived += TranslationService_TranslationReceived;
			}

			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (ActiveDocument == null) return;
			SetIdAndActiveFile();
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
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
				foreach (var sourceSegment in translationData.SourceSegments)
				{
					var currentSegmentPair =
						ActiveDocument.SegmentPairs.FirstOrDefault(segPair => segPair.Source.ToString() == sourceSegment);

					AddTargetSegmentMetaData(translationData.TranslationOriginInformation, currentSegmentPair);
				}
			}
		}

		private void AttachToClosedEvent()
		{
			_batchProcessingWindow.Closed -= Window_Closed;
			_batchProcessingWindow.Closed += Window_Closed;
			_isFirstTime = false;
		}

		private void SetBatchProcessingWindow()
		{
			if (_batchProcessingWindow is not null) return;
			_batchProcessingWindow = Application.Current.Dispatcher.Invoke(MtCloudApplicationInitializer.GetCurrentWindow);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			_isFirstTime = true;
			_batchProcessingWindow = null;
			Application.Current.Dispatcher.Invoke(MtCloudApplicationInitializer.CloseOpenedDocuments);
			_segmentMetadataCreator.AddToSegmentContextData();
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

		private void AddToSegmentContextData()
		{
			var currentSegmentPair = ActiveDocument.ActiveSegmentPair;
			var translationOrigin = currentSegmentPair.Properties.TranslationOrigin;

			if (translationOrigin is null)
				return;

			if (ActiveDocumentData.TryGetValue(ActiveDocument.ActiveSegmentPair.Properties.Id, out var targetData))
			{
				_segmentMetadataCreator.AddToCurrentSegmentContextData(ActiveDocument, targetData.TranslationOriginInformation);

			}
		}

		private void AddTargetSegmentMetaData(TranslationOriginInformation translationOriginInformation, ISegmentPair currentSegmentPair)
		{
			currentSegmentPair ??= ActiveDocument.ActiveSegmentPair;
			var segmentId = currentSegmentPair.Properties.Id;

			ActiveDocumentData.TryGetValue(segmentId, out var targetSegmentData);
			if (targetSegmentData == null)
			{
				ActiveDocumentData[segmentId] = new TargetSegmentData
				{
					TranslationOriginInformation = translationOriginInformation
				};
			}
			else
			{
				ActiveDocumentData[segmentId].TranslationOriginInformation = targetSegmentData.TranslationOriginInformation;
			}
		}

		public Dictionary<SegmentId, TargetSegmentData> ActiveDocumentData
		{
			get
			{
				if (Data.ContainsKey(_docId)) return Data[_docId];
				SetIdAndActiveFile();

				return Data[_docId];
			}
		}

		private Document ActiveDocument => _editorController?.ActiveDocument;

		private void SetIdAndActiveFile()
		{
			_docId = ActiveDocument.ActiveFile.Id;
			if (!Data.ContainsKey(_docId))
			{
				Data[_docId] = new Dictionary<SegmentId, TargetSegmentData>();
			}
		}

		public Dictionary<Guid, Dictionary<SegmentId, TargetSegmentData>> Data { get; set; } = new();
	}
}
