using System;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Events;
using Sdl.Core.Globalization;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public partial class SegmentSupervisor : ISegmentSupervisor
	{
		private const string OriginSystemName = "SDL Machine Translation Cloud Provider";
		private readonly EditorController _editorController;

		public SegmentSupervisor(EditorController editorController)
		{
			_editorController = editorController;
		}

		public void StartSupervising()
		{
			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
		}

		public void StopSupervising()
		{
			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
			if (ActiveDocument == null) return;
			ActiveDocument.ActiveSegmentChanged += ActiveDocumentOnActiveSegmentChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			ActiveDocument.SegmentsTranslationOriginChanged += ActiveDocument_SegmentsTranslationOriginChanged;
		}

		public event ConfirmationLevelChangedEventHandler ConfirmationLevelChanged;

		private Document ActiveDocument => _editorController.ActiveDocument;
		public Dictionary<SegmentId, ImprovedTarget> Improvements { get; set; } = new Dictionary<SegmentId, ImprovedTarget>();

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			var segment = (ISegment)((ISegmentContainerNode)sender).Item;
			if (segment == null) return;

			var segmentId = segment.Properties.Id;
			var translationOrigin = segment.Properties.TranslationOrigin;

			if (IsImprovementToTpTranslation(translationOrigin, segmentId, segment))
			{
				EditImprovements(segmentId, segment.ToString(), null, translationOrigin);
			}
			else
			{
				EditImprovements(segmentId, null, segment.ToString(), translationOrigin);
			}

			if (segment.Properties.ConfirmationLevel == ConfirmationLevel.Translated)
			{
				ConfirmationLevelChanged?.Invoke(segmentId);
			}
		}

		private void ActiveDocument_SegmentsTranslationOriginChanged(object sender, EventArgs e)
		{
			var segment = (ISegment)((ISegmentContainerNode)sender).Item;
			if (segment == null) return;

			var segmentId = segment.Properties.Id;
			EditImprovements(segmentId, null, null, segment.Properties?.TranslationOrigin);
		}

		private void ActiveDocumentOnActiveSegmentChanged(object sender, EventArgs e)
		{
			var activeSegmentPair = _editorController.ActiveDocument.ActiveSegmentPair;
			if (activeSegmentPair == null) return;
			EditImprovements(activeSegmentPair.Properties.Id, null, activeSegmentPair.Target.ToString(), activeSegmentPair.Properties.TranslationOrigin);
		}

		private void EditImprovements(SegmentId segmentId, string improvement, string originalTarget, ITranslationOrigin translationOrigin)
		{
			if (Improvements.ContainsKey(segmentId))
			{
				var item = Improvements[segmentId];
				if (improvement != null && item.Improvement != improvement) item.Improvement = improvement;
				if (translationOrigin != null && !item.OriginalTargetOrigin.Equals(translationOrigin)) item.OriginalTargetOrigin = translationOrigin;
			}
			else
			{
				if (string.IsNullOrWhiteSpace(originalTarget)) return;
				Improvements[segmentId] = new ImprovedTarget(originalTarget)
				{
					Improvement = improvement,
					OriginalTargetOrigin = translationOrigin
				};
			}
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			//TODO: do it like it's done in RateItViewModel to avoid memory leaks
			if (ActiveDocument == null) return;
			ActiveDocument.ActiveSegmentChanged += ActiveDocumentOnActiveSegmentChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			ActiveDocument.SegmentsTranslationOriginChanged += ActiveDocument_SegmentsTranslationOriginChanged;
		}

		private bool IsImprovementToTpTranslation(ITranslationOrigin translationOrigin, SegmentId segmentId, ISegment segment)
		{
			return translationOrigin?.OriginBeforeAdaptation?.OriginSystem == OriginSystemName &&
				   Improvements[segmentId].OriginalTarget != segment.ToString() &&
				   translationOrigin.OriginType == "interactive" &&
				   segment.Properties?.ConfirmationLevel == ConfirmationLevel.Translated;
		}
	}
}