using System;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SegmentSupervisor : ISegmentSupervisor
	{
		private const string OriginSystemName = "SDL Machine Translation Cloud Provider";
		private readonly EditorController _editorController;

		public SegmentSupervisor(EditorController editorController)
		{
			_editorController = editorController;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
		}

		public event EventHandler ImprovementAvailable;

		private Document ActiveDocument => _editorController.ActiveDocument;
		public ISegmentPair ActiveSegmentPair => _editorController.ActiveDocument.ActiveSegmentPair;
		public string Improvement { get; private set; }
		public string OriginalTargetText { get; private set; }
		public ITranslationOrigin OriginalTargetTextOrigin { get; set; }

		public Dictionary<string, ImprovedTarget> Improvements { get; set; } = new Dictionary<string, ImprovedTarget>();

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			var currentText = ActiveSegmentPair.Target.ToString();

			var isImprovementToTpTranslation = OriginalTargetTextOrigin?.OriginBeforeAdaptation?.OriginSystem == OriginSystemName &&
			                                   OriginalTargetText != currentText &&
			                                   ActiveSegmentPair.Properties.TranslationOrigin.OriginType == "interactive" &&
			                                   ActiveSegmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Translated;
			if (isImprovementToTpTranslation)
			{
				Improvement = currentText;
				Improvements[ActiveSegmentPair.Source.ToString()] = new ImprovedTarget
				{
					OriginalTarget = OriginalTargetText,
					Improvement = Improvement
				};
			}

			if (!string.IsNullOrWhiteSpace(Improvement))
			{
				ImprovementAvailable?.Invoke(this, EventArgs.Empty);
			}
			else
			{
				OriginalTargetText = currentText;
				Improvement = null;
			}
		}

		private void ActiveDocument_SegmentsTranslationOriginChanged(object sender, EventArgs e)
		{
			OriginalTargetTextOrigin = ActiveSegmentPair.Properties.TranslationOrigin;
		}

		private void ActiveDocumentOnActiveSegmentChanged(object sender, EventArgs e)
		{
			OriginalTargetTextOrigin = ActiveSegmentPair.Properties.TranslationOrigin;
			OriginalTargetText = ActiveSegmentPair.Target.ToString();
			Improvement = string.Empty;
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			//TODO: do it like it's done in RateItViewModel to avoid memory leaks
			if (ActiveDocument == null) return;
			ActiveDocument.ActiveSegmentChanged += ActiveDocumentOnActiveSegmentChanged;
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			ActiveDocument.SegmentsTranslationOriginChanged += ActiveDocument_SegmentsTranslationOriginChanged;
		}
	}
}