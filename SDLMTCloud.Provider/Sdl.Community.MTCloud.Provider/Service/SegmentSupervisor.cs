using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Core.Globalization;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SegmentSupervisor : ISegmentSupervisor
	{
		private readonly EditorController _editorController;
		private Guid _docId;
		private ITranslationService _translationService;

		public SegmentSupervisor(EditorController editorController)
		{
			_editorController = editorController;
		}

		public event ConfirmationLevelChangedEventHandler SegmentConfirmed;

		private Document ActiveDocument => _editorController?.ActiveDocument;

		public Dictionary<SegmentId, TargetSegmentData> ActiveDocumentData
		{
			get
			{
				if (Data.ContainsKey(_docId)) return Data[_docId];
				SetIdAndActiveFile();

				return Data[_docId];
			}
		}

		public Dictionary<Guid, Dictionary<SegmentId, TargetSegmentData>> Data { get; set; } = new Dictionary<Guid, Dictionary<SegmentId, TargetSegmentData>>();

		public void AddImprovement(SegmentId segmentId, string improvement)
		{
			if (!ActiveDocumentData.ContainsKey(segmentId)) return;

			var item = ActiveDocumentData[segmentId].Feedback;
			if (item.Improvement != improvement) item.Improvement = improvement;
		}

		public void CreateFeedbackEntry(SegmentId segmentId, string originalTarget, string targetOrigin,
			string source)
		{
			if (targetOrigin != PluginResources.SDLMTCloudName) return;

			ActiveDocumentData.TryGetValue(segmentId, out var targetSegmentData);
			if (targetSegmentData == null)
			{
				ActiveDocumentData[segmentId] = new TargetSegmentData
				{
					Feedback = new ImprovementFeedback(originalTarget, source),
				};
			}
			else
			{
				ActiveDocumentData[segmentId].Feedback = new ImprovementFeedback(originalTarget, source);
			}
		}

		public ImprovementFeedback GetImprovement(SegmentId? segmentId = null)
		{
			var currentSegment = segmentId ?? ActiveDocument.ActiveSegmentPair?.Properties.Id;
			ImprovementFeedback improvement = null;

			var segmentHasImprovement = currentSegment != null && ActiveDocumentData.ContainsKey(currentSegment.Value);
			if (segmentHasImprovement)
			{
				improvement = ActiveDocumentData[currentSegment.Value].Feedback;
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

		private static bool IsFromSdlMtCloud(ITranslationOrigin translationOrigin, bool lookInPrevious = false)
		{
			//TODO: extract in helper
			if (lookInPrevious)
			{
				return translationOrigin?.OriginBeforeAdaptation?.OriginSystem == PluginResources.SDLMTCloudName;
			}
			return translationOrigin?.OriginSystem == PluginResources.SDLMTCloudName;
		}

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			var segment = (ISegment)((ISegmentContainerNode)sender).Item;
			if (segment == null) return;

			var segmentId = segment.Properties.Id;
			var translationOrigin = segment.Properties.TranslationOrigin;
			var targetSegmentText = segment.ToString();

			if (IsImprovementToTpTranslation(translationOrigin, segmentId, segment))
			{
				AddImprovement(segmentId, targetSegmentText);
			}

			if (segment.Properties.ConfirmationLevel != ConfirmationLevel.Translated) return;
			SegmentConfirmed?.Invoke(segmentId);
		}

		private void CreateFeedback(List<string> sources, TranslationData targetSegmentData, int i, ISegmentPair currentSegmentPair)
		{
			var currentSegmentId = currentSegmentPair?.Properties.Id;

			if (currentSegmentId != null)
			{
				CreateFeedbackEntry(currentSegmentId.Value, targetSegmentData.TargetSegments[i], PluginResources.SDLMTCloudName,
					sources[i]);
			}
			else
			{
				CreateFeedbackEntry(ActiveDocument.ActiveSegmentPair.Properties.Id, targetSegmentData.TargetSegments[i], PluginResources.SDLMTCloudName,
					ActiveDocument.ActiveSegmentPair.Source.ToString());
			}
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (ActiveDocument == null) return;
			SetIdAndActiveFile();
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
		}

		private bool IsImprovementToTpTranslation(ITranslationOrigin translationOrigin, SegmentId segmentId, ISegment segment)
		{
			return IsFromSdlMtCloud(translationOrigin, true) &&
				   ActiveDocumentData.ContainsKey(segmentId) &&
				   ActiveDocumentData[segmentId].Feedback.OriginalMtCloudTranslation != segment.ToString() &&
				   segment.Properties?.ConfirmationLevel == ConfirmationLevel.Translated;
		}

		private void SetIdAndActiveFile()
		{
			_docId = ActiveDocument.ActiveFile.Id;
			if (!Data.ContainsKey(_docId))
			{
				Data[_docId] = new Dictionary<SegmentId, TargetSegmentData>();
			}
		}

		private void TranslationService_TranslationReceived(TranslationData translationData)
		{
			if (ActiveDocument == null) return;
			for (var i = 0; i < translationData.SourceSegments.Count; i++)
			{
				var currentSegmentPair =
					ActiveDocument.SegmentPairs.FirstOrDefault(
						segPair => segPair.Source.ToString() == translationData.SourceSegments[i]);
				CreateFeedback(translationData.SourceSegments, translationData, i, currentSegmentPair);
			}
		}
	}
}