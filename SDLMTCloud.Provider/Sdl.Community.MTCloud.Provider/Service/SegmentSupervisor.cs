using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly EditorController _editorController;
		private Guid _docId;

		public SegmentSupervisor(EditorController editorController)
		{
			_editorController = editorController;
		}

		public event ConfirmationLevelChangedEventHandler SegmentConfirmed;

		private IStudioDocument ActiveDocument => _editorController.ActiveDocument;

		public Dictionary<SegmentId, ImprovedTarget> ActiveDocumentImprovements
		{
			get
			{
				if (Improvements.ContainsKey(_docId)) return Improvements[_docId];
				SetIdAndActiveFile();

				return Improvements[_docId];
			}
		}

		public Dictionary<Guid, Dictionary<SegmentId, ImprovedTarget>> Improvements { get; set; } = new Dictionary<Guid, Dictionary<SegmentId, ImprovedTarget>>();

		public void StartSupervising()
		{
			StopSupervising();

			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			if (ActiveDocument == null) return;
			ActiveDocument.ActiveSegmentChanged += ActiveDocumentOnActiveSegmentChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
		}

		public void StopSupervising()
		{
			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;

			if (ActiveDocument == null) return;
			ActiveDocument.ActiveSegmentChanged -= ActiveDocumentOnActiveSegmentChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
		}

		private string GetSourceSegment(ISegment segment)
		{
			return
				_editorController.ActiveDocument.SegmentPairs.FirstOrDefault(sp => sp.Properties.Id == segment.Properties.Id)?.Source
					.ToString();
		}

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			var segment = (ISegment)((ISegmentContainerNode)sender).Item;
			if (segment == null) return;

			var segmentId = segment.Properties.Id;
			var translationOrigin = segment.Properties.TranslationOrigin;
			var sourceSegment = GetSourceSegment(segment);

			if (IsImprovementToTpTranslation(translationOrigin, segmentId, segment))
			{
				EditImprovements(segmentId, segment.ToString(), null, translationOrigin, sourceSegment);
			}
			else
			{
				EditImprovements(segmentId, null, segment.ToString(), translationOrigin, sourceSegment);
			}

			if (segment.Properties.ConfirmationLevel != ConfirmationLevel.Translated) return;
			SegmentConfirmed?.Invoke(segmentId);
		}

		private void ActiveDocumentOnActiveSegmentChanged(object sender, EventArgs e)
		{
			var activeSegmentPair = _editorController.ActiveDocument.ActiveSegmentPair;
			if (activeSegmentPair == null) return;
			EditImprovements(activeSegmentPair.Properties.Id, null, activeSegmentPair.Target.ToString(), activeSegmentPair.Properties.TranslationOrigin, activeSegmentPair.Source.ToString());
		}

		private void EditImprovements(SegmentId segmentId, string improvement, string originalTarget, ITranslationOrigin translationOrigin, string originalSource)
		{
			if (improvement != null && ActiveDocumentImprovements.ContainsKey(segmentId))
			{
				var item = ActiveDocumentImprovements[segmentId];
				if (item.Improvement != improvement) item.Improvement = improvement;
			}
			else
			{
				if (translationOrigin.OriginSystem != PluginResources.SDLMTCloudName ||
				    ActiveDocumentImprovements.ContainsKey(segmentId) &&
				    originalSource == ActiveDocumentImprovements[segmentId].OriginalSource) return;

				ActiveDocumentImprovements[segmentId] = new ImprovedTarget(originalTarget, originalSource);
			}
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (ActiveDocument == null) return;

			SetIdAndActiveFile();

			ActiveDocument.ActiveSegmentChanged += ActiveDocumentOnActiveSegmentChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
		}

		private bool IsImprovementToTpTranslation(ITranslationOrigin translationOrigin, SegmentId segmentId, ISegment segment)
		{
			return translationOrigin?.OriginBeforeAdaptation?.OriginSystem == PluginResources.SDLMTCloudName &&
				   ActiveDocumentImprovements.ContainsKey(segmentId) &&
				   ActiveDocumentImprovements[segmentId].OriginalTarget != segment.ToString() &&
				   translationOrigin?.OriginType == "interactive" &&
				   segment.Properties?.ConfirmationLevel == ConfirmationLevel.Translated;
		}

		private void SetIdAndActiveFile()
		{
			_docId = ActiveDocument.ActiveFile.Id;
			if (!Improvements.ContainsKey(_docId))
			{
				Improvements[_docId] = new Dictionary<SegmentId, ImprovedTarget>();
			}
		}
	}
}