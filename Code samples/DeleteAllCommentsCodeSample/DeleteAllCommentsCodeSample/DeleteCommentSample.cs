using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace DeleteAllCommentsCodeSample
{
	[Action("DeleteCommentsAction", Name = "DeleteComments", Icon = "")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	class DeleteCommentSample:AbstractAction
	{
		protected override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocument = editorController?.ActiveDocument;
			if (activeDocument != null)
			{
				var segmentPairs = activeDocument.SegmentPairs;
				foreach (var segmentPair in segmentPairs)
				{
					activeDocument.DeleteAllCommentsOnSegment(segmentPair);
				}
			}
		}
	}
}
