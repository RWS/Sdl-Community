using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.Core.Globalization;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service.RateIt
{
	public class SegmentSupervisor : ISegmentSupervisor
	{
		private readonly EditorController _editorController;
		private ITranslationService _translationService;
		private static List<string> _providerNames;
		

		public SegmentSupervisor(EditorController editorController)
		{
			_editorController = editorController;
			_providerNames = new List<string> { PluginResources.SDLMTCloud_Provider_Name, PluginResources.SDLMTCloud_Provider_OldName };
		}

		public event ShouldSendFeedbackEventHandler ShouldSendFeedback;

		private IStudioDocument ActiveDocument => _editorController?.ActiveDocument;

		private ConcurrentDictionary<SegmentId, ImprovementFeedback> ActiveDocumentData
		{
			get
			{
				if (ActiveDocument?.ActiveFile?.Id == null) return null;

				var activeFileId = ActiveDocument.ActiveFile.Id;
				if (!Data.ContainsKey(activeFileId))
				{
					Data[activeFileId] = new ConcurrentDictionary<SegmentId, ImprovementFeedback>();
				}

				return Data[activeFileId];
			}
		}

		private Dictionary<Guid, ConcurrentDictionary<SegmentId, ImprovementFeedback>> Data { get; set; } = new();

		public void AddImprovement(SegmentId segmentId, string improvement)
		{
			if (!ActiveDocumentData.ContainsKey(segmentId)) return;

			var item = ActiveDocumentData[segmentId];
			if (item.Improvement != improvement) item.Improvement = improvement;
		}

		public void CreateFeedbackEntry(SegmentId segmentId, string originalTarget, string source)
		{
			if (ActiveDocumentData is null) return;
			ActiveDocumentData[segmentId] = new ImprovementFeedback(originalTarget, source);
		}

		public ImprovementFeedback GetImprovement(SegmentId? segmentId = null)
		{
			var currentSegment = segmentId ?? ActiveDocument.ActiveSegmentPair?.Properties.Id;
			ImprovementFeedback improvement = null;

			var segmentHasImprovement = currentSegment != null && ActiveDocumentData.ContainsKey(currentSegment.Value);
			if (segmentHasImprovement)
			{
				improvement = ActiveDocumentData[currentSegment.Value];
			}
			return improvement;
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

		private static bool WasPreviousOriginMTCloud(ITranslationOrigin translationOrigin)
		{
			return _providerNames.Contains(translationOrigin?.OriginBeforeAdaptation?.OriginSystem);
		}

		private static bool IsOriginMTCloud(ITranslationOrigin translationOrigin)
		{
			return _providerNames.Contains(translationOrigin?.OriginSystem);
		}

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			var targetSegment = (ISegment)((ISegmentContainerNode)sender).Item;
			if (targetSegment == null) return;

			var segmentId = targetSegment.Properties.Id;
			var translationOrigin = targetSegment.Properties.TranslationOrigin;

			if (IsImprovementToTpTranslation(translationOrigin, segmentId, targetSegment))
			{
				AddImprovement(segmentId, targetSegment.ToString());
			}

			if (!IsOriginMTCloud(translationOrigin) && !WasPreviousOriginMTCloud(translationOrigin) ||
			    targetSegment.Properties.ConfirmationLevel != ConfirmationLevel.Translated) return;

			ShouldSendFeedback?.Invoke(segmentId);
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (ActiveDocument == null) return;
			ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
		}

		private bool IsImprovementToTpTranslation(ITranslationOrigin translationOrigin, SegmentId segmentId, ISegment segment)
		{
			if (ActiveDocumentData is null) return false;

			return WasPreviousOriginMTCloud(translationOrigin) &&
				   ActiveDocumentData.ContainsKey(segmentId) &&
				   ActiveDocumentData[segmentId].OriginalMtCloudTranslation != segment.ToString() &&
				   segment.Properties?.ConfirmationLevel == ConfirmationLevel.Translated;
		}

		private void TranslationService_TranslationReceived(TranslationData translationData)
		{
			if (ActiveDocument == null) return;
			foreach (var currentSegmentPairId in translationData.Segments.Keys)
			{
				CreateFeedbackEntry(currentSegmentPairId, translationData.TargetSegments[currentSegmentPairId],
					translationData.Segments[currentSegmentPairId]);
			}
		}
	}
}