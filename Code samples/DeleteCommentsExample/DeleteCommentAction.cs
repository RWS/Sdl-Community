using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace DeleteCommentsExample
{
	[Action("DeleteCommentExample",
		Name = "Delete Comment Example")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class DeleteCommentAction : AbstractAction
	{
		protected override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocument = editorController.ActiveDocument;

			var selection = activeDocument.Selection.Target;

			var from = selection.From;
			var to = selection.UpTo;
			if (selection.IsReversed)
			{
				to = from;
				from = selection.UpTo;
			}

			if (from is null || to is null)
			{
				MessageBox.Show("Select valid segments", null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var startCoordinates = new TextCoordinates
			{
				CharNumber = from.CursorPosition,
				SegmentId = int.Parse(from.SegmentId)
			};

			var endCoordinates = new TextCoordinates
			{
				CharNumber = to.CursorPosition,
				SegmentId = int.Parse(to.SegmentId)
			};

			var manager = DefaultFileTypeManager.CreateInstance(true);
			var currentFilePath = activeDocument.ActiveFile.LocalFilePath;
			var converter = manager.GetConverterToDefaultBilingual(currentFilePath, currentFilePath, null);

			var commentRemover = new CommentRemover(new SegmentVisitor(), startCoordinates, endCoordinates);
			converter?.AddBilingualProcessor(new BilingualContentHandlerAdapter(commentRemover));

			var activeFile = activeDocument.ActiveFile;
			var mode = activeDocument.Mode;

			editorController.Close(activeDocument);
			converter?.Parse();
			editorController.Open(activeFile, mode);
		}
	}
}