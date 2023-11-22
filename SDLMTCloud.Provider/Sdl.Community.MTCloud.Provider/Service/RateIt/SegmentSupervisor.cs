using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Extensions;
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
		

		public SegmentSupervisor(EditorController editorController)
		{
			_editorController = editorController;
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

		public void UpdateImprovement(SegmentId segmentId, string improvement)
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
			var targetSegment = ActiveDocument.ActiveSegmentPair.Target;
			var currentSegmentId = segmentId ?? targetSegment?.Properties.Id;
			ImprovementFeedback improvement = null;

			if (currentSegmentId is null) return null;

			var segmentHasImprovement = ActiveDocumentData.ContainsKey(currentSegmentId.Value);
			if (segmentHasImprovement)
			{
				improvement = ActiveDocumentData[currentSegmentId.Value];

				//segmentId == null means that the user is forcing feedback sending which means that the improvement may not be up to date
				if (segmentId == null && IsImprovementToTpTranslation(targetSegment.Properties.TranslationOrigin, currentSegmentId.Value, targetSegment))
				{
					UpdateImprovement(currentSegmentId.Value, targetSegment.ToString());
				}
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

			if (_translationService != null)
			{
				_translationService.TranslationReceived -= TranslationService_TranslationReceived;
			}

			if (translationService != null)
			{
				_translationService = translationService;
				_translationService.TranslationReceived += TranslationService_TranslationReceived;
			}

			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
		}

		private static bool WasPreviousOriginMTCloud(ITranslationOrigin translationOrigin)
		{
			return translationOrigin?.OriginBeforeAdaptation?.OriginSystem?.IsLanguageWeaverOrigin() ?? false;
		}

		private static bool IsOriginMTCloud(ITranslationOrigin translationOrigin)
		{
			return translationOrigin?.OriginSystem?.IsLanguageWeaverOrigin() ?? false;
		}

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			if ((sender as ISegmentContainerNode).Item is not ISegment targetSegment)
			{
				return;
			}

			var segmentId = targetSegment.Properties.Id;
			var translationOrigin = targetSegment.Properties.TranslationOrigin;
			if (IsImprovementToTpTranslation(translationOrigin, segmentId, targetSegment))
			{
				UpdateImprovement(segmentId, targetSegment.ToString());
			}

			if ((IsOriginMTCloud(translationOrigin) || WasPreviousOriginMTCloud(translationOrigin))
			 && targetSegment.Properties.ConfirmationLevel == ConfirmationLevel.Translated)
			{
				ShouldSendFeedback?.Invoke(segmentId);
			}
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
			return (WasPreviousOriginMTCloud(translationOrigin) || IsOriginMTCloud(translationOrigin))
				&& ActiveDocumentData.ContainsKey(segmentId)
				&& ActiveDocumentData[segmentId].OriginalMtCloudTranslation != segment.ToString()
				&& segment.Properties?.ConfirmationLevel == ConfirmationLevel.Translated;
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