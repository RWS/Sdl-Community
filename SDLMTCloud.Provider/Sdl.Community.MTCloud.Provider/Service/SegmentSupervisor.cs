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

		private IStudioDocument ActiveDocument => _editorController?.ActiveDocument;

		public Dictionary<SegmentId, Feedback> ActiveDocumentImprovements
		{
			get
			{
				if (Improvements.ContainsKey(_docId)) return Improvements[_docId];
				SetIdAndActiveFile();

				return Improvements[_docId];
			}
		}

		public Dictionary<Guid, Dictionary<SegmentId, Feedback>> Improvements { get; set; } = new Dictionary<Guid, Dictionary<SegmentId, Feedback>>();

		public Feedback GetImprovement(SegmentId? segmentId = null)
		{
			var currentSegment = segmentId ?? ActiveDocument.ActiveSegmentPair?.Properties.Id;
			Feedback improvement = null;

			var segmentHasImprovement = currentSegment != null && ActiveDocumentImprovements.ContainsKey(currentSegment.Value);
			if (segmentHasImprovement)
			{
				improvement = ActiveDocumentImprovements[currentSegment.Value];
			}
			return improvement;
		}

		public void StartSupervising(ITranslationService translationService)
		{
			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;

			if (ActiveDocument != null)
			{
				ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
			}

			_translationService = translationService;

			if (_translationService != null)
			{
				_translationService.TranslationReceived -= TranslationService_TranslationReceived;
				_translationService.TranslationReceived += TranslationService_TranslationReceived;
			}

			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			if (ActiveDocument != null)
			{
				ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			}
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

		

		public void AddImprovement(SegmentId segmentId, string improvement)
		{
			if (!ActiveDocumentImprovements.ContainsKey(segmentId)) return;

			var item = ActiveDocumentImprovements[segmentId];
			if (item.Suggestion != improvement) item.Suggestion = improvement;
		}

		public void CreateFeedbackEntry(SegmentId segmentId, string originalTarget, string targetOrigin,
			string source)
		{
			if (targetOrigin != PluginResources.SDLMTCloudName ) return;
			ActiveDocumentImprovements[segmentId] = new Feedback(originalTarget, source);
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (ActiveDocument == null) return;
			SetIdAndActiveFile();
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
		}

		private bool IsImprovementToTpTranslation(ITranslationOrigin translationOrigin, SegmentId segmentId, ISegment segment)
		{
			return translationOrigin?.OriginBeforeAdaptation?.OriginSystem == PluginResources.SDLMTCloudName &&
				   ActiveDocumentImprovements.ContainsKey(segmentId) &&
				   ActiveDocumentImprovements[segmentId].OriginalMtCloudTranslation != segment.ToString() &&
				   segment.Properties?.ConfirmationLevel == ConfirmationLevel.Translated;
		}

		private void SetIdAndActiveFile()
		{
			_docId = ActiveDocument.ActiveFile.Id;
			if (!Improvements.ContainsKey(_docId))
			{
				Improvements[_docId] = new Dictionary<SegmentId, Feedback>();
			}
		}

		private void TranslationService_TranslationReceived(List<string> sources, List<string> targets)
		{
			if (ActiveDocument == null) return;

			for (var i = 0; i < sources.Count; i++)
			{
				var currentSegmentId = ActiveDocument.SegmentPairs.FirstOrDefault(segPair => segPair.Source.ToString() == sources[i])?.Properties.Id;

				if (currentSegmentId != null)
				{
					CreateFeedbackEntry(currentSegmentId.Value, targets[i], PluginResources.SDLMTCloudName,
						sources[i]);
				}
				else
				{
					CreateFeedbackEntry(ActiveDocument.ActiveSegmentPair.Properties.Id, targets[i], PluginResources.SDLMTCloudName,
						ActiveDocument.ActiveSegmentPair.Source.ToString());
				}
			}
		}
	}
}