using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
	[Action("CommunityADFComment", Name = "CADF - Add comment to all filtered segments", Icon = "AddComment")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	public class AddCommentAction: AbstractAction
	{
		protected override void Execute()
		{
			var addCommentsWindow = new AddCommentWindow();
			if (addCommentsWindow.ShowDialog() == DialogResult.OK)
			{
				var severityLevel = addCommentsWindow.SeverityLevel;
				var comment = addCommentsWindow.Comment;
				var editorController = SdlTradosStudio.Application.GetController<EditorController>();
				var activeDocument = editorController?.ActiveDocument;
				if (activeDocument != null)
				{
					var filteredSegments = activeDocument.FilteredSegmentPairs;
					foreach (var filteredSegment in filteredSegments)
					{
						activeDocument.AddCommentOnSegment(filteredSegment, comment, severityLevel);
					}
				}
			}
		}
	}
}
