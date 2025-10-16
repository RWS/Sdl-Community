using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System.Windows;

namespace CustomViewExample.Ribbon.Actions
{

    [RibbonGroup("CustomViewExample_MyEditorActionsGroup", "My Editor Group")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorReviewRibbonTabLocation))]
    public class MyEditorActionsGroup : AbstractRibbonGroup
    {
    }

    [Action(Id = "CustomViewExample_MyAction1_Id", Name = "Action 1", Description = "Action 1 Description",
		Icon = "wordLight_yellow", ContextByType = typeof(EditorController))]
	[ActionLayout(typeof(MyEditorActionsGroup), 3, DisplayType.Large, "Action 1", true)]
	internal class MyAction1 : AbstractAction
	{
		protected override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			if (editorController.ActiveDocument == null)
			{
				return;
			}
			var activeFile = editorController.ActiveDocument.ActiveFile;

			// return false and nothing happens (with any file with any segment id)
			var result = editorController.ActiveDocument.SetActiveSegmentPair(activeFile, "1", true);


			MessageBox.Show("Selected Segment 1 (Action 1):" + result);
		}
	}

    [Action(Id = "CustomViewExample_MyAction2_Id", Name = "Action 2", Description = "Action 2 Description",
        ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(MyEditorActionsGroup), 0, DisplayType.Normal, "Action 2", true)]
    internal class MyAction2 : AbstractAction
    {
        protected override void Execute()
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            if (editorController.ActiveDocument == null)
            {
                return;
            }
            var activeFile = editorController.ActiveDocument.ActiveFile;

            // return false and nothing happens (with any file with any segment id)
            var result = editorController.ActiveDocument.SetActiveSegmentPair(activeFile, "2", true);


            MessageBox.Show("Selected Segment 2 (Action 2):" + result);
        }
    }

    [Action(Id = "CustomViewExample_MyAction3_Id", Name = "Action 3", Description = "Action 3 Description",
        ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(MyEditorActionsGroup), 0, DisplayType.Normal, "Action 3", true)]
    internal class MyAction3 : AbstractAction
    {
        protected override void Execute()
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            if (editorController.ActiveDocument == null)
            {
                return;
            }
            var activeFile = editorController.ActiveDocument.ActiveFile;

            // return false and nothing happens (with any file with any segment id)
            var result = editorController.ActiveDocument.SetActiveSegmentPair(activeFile, "3", true);


            MessageBox.Show("Selected Segment 3 (Action 3):" + result);
        }
    }
}
