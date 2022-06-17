using System;
using System.Collections.Generic;
using NLog;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Trados.Transcreate.Common;

namespace Trados.Transcreate.Actions
{
	[Action(Id = "TranscreateManager_SwitchWithRecommendedTranslation_Action",
		Name = "TranscreateManager_SwitchWithRecommendedTranslation_Name",
		Description = "TranscreateManager_SwitchWithRecommendedTranslation_Description",
		Icon = "Check", ContextByType = typeof(EditorController))]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 0, DisplayType.Default, "", true)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentSpellcheckContextMenuLocation), 0, DisplayType.Default, "", true)]
	public class SwitchWithRecommendedTranslationAction : AbstractAction
	{
		private EditorController _editorController;
		private IStudioDocument _studioDocument;
		private bool _isTranscreateDocument;

		protected override void Execute()
		{
			if (!_isTranscreateDocument)
			{
				return;
			}

			var selectedSegmentPair = _studioDocument?.GetActiveSegmentPair();
			var selectedContextType = GetContextType(selectedSegmentPair);
			if (selectedContextType == null)
			{
				return;
			}

			var slectedSegmentPairs = GetSegmentPairs(selectedContextType);
			var selectedIndex = GetSelectedIndex(slectedSegmentPairs, selectedSegmentPair);
			if (selectedIndex <= -1)
			{
				return;
			}

			var recommendedSegmentPairs = GetSegmentPairs("Recommended");
			var recommendedSegmentPair = recommendedSegmentPairs[selectedIndex];

			//switch
			var recommendedTargetClone = recommendedSegmentPair.Target.Clone() as ISegment;
			var recommendedPropertiesClone = recommendedSegmentPair.Properties.Clone() as ISegmentPairProperties;

			SwitchTranslation(recommendedSegmentPair, selectedSegmentPair?.Target, selectedSegmentPair?.Properties);
			SwitchTranslation(selectedSegmentPair, recommendedTargetClone, recommendedPropertiesClone);
		}

		public override void Initialize()
		{
			if (_editorController != null)
			{
				_editorController.ActiveDocumentChanged -= EditorControllerOnActiveDocumentChanged;
				_editorController.Opened -= EditorControllerOnOpened;
			}

			_editorController = GetEditorController();

			if (_editorController != null)
			{
				_editorController.ActiveDocumentChanged += EditorControllerOnActiveDocumentChanged;
				_editorController.Opened += EditorControllerOnOpened;
			}

			DocumentChanged(_editorController?.ActiveDocument);
		}

		private void SwitchTranslation(ISegmentPair segmentPair, ISegment updatedSegment, ISegmentPairProperties updatedProperties)
		{
			segmentPair.Target.Clear();
			foreach (var item in updatedSegment)
			{
				segmentPair.Target.Add(item.Clone() as IAbstractMarkupData);
			}

			_studioDocument.UpdateSegmentPair(segmentPair);


			segmentPair.Properties.ConfirmationLevel = updatedProperties.ConfirmationLevel;
			segmentPair.Properties.TranslationOrigin = updatedProperties.TranslationOrigin?.Clone() as ITranslationOrigin;
			segmentPair.Properties.IsLocked = updatedProperties.IsLocked;
			_studioDocument.UpdateSegmentPairProperties(segmentPair, segmentPair.Properties);
		}

		private int GetSelectedIndex(IReadOnlyList<ISegmentPair> segmentPairs, ISegmentPair selectedSegmentPair)
		{
			for (var index = 0; index < segmentPairs.Count; index++)
			{
				var segmentPair = segmentPairs[index];
				if (segmentPair.Properties.Id == selectedSegmentPair.Properties.Id)
				{
					return index;
				}
			}

			return -1;
		}

		private void EditorControllerOnOpened(object sender, DocumentEventArgs e)
		{
			DocumentChanged(e.Document);
		}

		private void EditorControllerOnActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			DocumentChanged(e.Document);
		}

		private void DocumentChanged(IStudioDocument document)
		{
			if (_studioDocument != null)
			{
				_studioDocument.ActiveSegmentChanged -= StudioDocumentOnActiveSegmentChanged;
			}

			_studioDocument = document;
			_isTranscreateDocument = IsTranscreateDocument(document);

			if (_isTranscreateDocument)
			{
				_studioDocument.ActiveSegmentChanged += StudioDocumentOnActiveSegmentChanged;
			}

			SetEnabled(_studioDocument?.GetActiveSegmentPair());
		}

		private bool IsTranscreateDocument(IStudioDocument document)
		{
			var projectInfo = document?.Project?.GetProjectInfo();
			if (projectInfo != null)
			{
				return projectInfo.ProjectOrigin == Constants.ProjectOrigin_TranscreateProject ||
					   projectInfo.ProjectOrigin == Constants.ProjectOrigin_BackTranslationProject;
			}

			return false;
		}

		private void StudioDocumentOnActiveSegmentChanged(object sender, EventArgs e)
		{
			SetEnabled(_studioDocument?.GetActiveSegmentPair());
		}

		private void SetEnabled(ISegmentPair segment)
		{
			var contextType = GetContextType(segment);
			Enabled = contextType?.StartsWith("Alternative ") ?? false;
		}

		private List<ISegmentPair> GetSegmentPairs(string contextType)
		{
			var segmentPairs = new List<ISegmentPair>();
			foreach (var segmentPair in _studioDocument.SegmentPairs)
			{
				var segmentPairContextType = GetContextType(segmentPair);
				if (segmentPairContextType == contextType)
				{
					segmentPairs.Add(segmentPair);
				}
				else if (segmentPairs.Count > 0)
				{
					// used to optimize exiting the loop sooner
					break;
				}
			}

			return segmentPairs;
		}

		private string GetContextType(ISegmentPair segmentPair)
		{
			var paragraphUnitProperties = segmentPair?.GetParagraphUnitProperties();
			var contextType = GetContextType(paragraphUnitProperties);
			return contextType;
		}

		private string GetContextType(IParagraphUnitProperties paragraphUnitProperties)
		{
			if (paragraphUnitProperties?.Contexts?.Contexts != null)
			{
				foreach (var context in paragraphUnitProperties.Contexts.Contexts)
				{
					if (context.ContextType == "Recommended" || context.ContextType.StartsWith("Alternative "))
					{
						return context.ContextType;
					}
				}
			}

			return null;
		}

		private static EditorController GetEditorController()
		{
			try
			{
				var editorController = SdlTradosStudio.Application?.GetController<EditorController>();
				return editorController;
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
			}

			return null;
		}
	}
}
