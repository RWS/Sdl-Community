using Sdl.Community.AdvancedDisplayFilter.Controls;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdvancedDisplayFilter.Actions
{
	[Action("CommunityADFTargetFilter", Name = "CADF - Target Filter", Icon = "")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	public class FilterTargetAction:AbstractAction
	{
		private readonly SegmentTextVisitor _segmentTextVisitor = new SegmentTextVisitor();

		protected override void Execute()
		{
			var activeDocument = SdlTradosStudio.Application.GetController<EditorController>()?.ActiveDocument;
			var displayFilterControl = CommunityApplicationInitializer.DisplayFilterControl;
			if (activeDocument != null && displayFilterControl != null)
			{
				var activeSegment = activeDocument.ActiveSegmentPair;
				var targetText = _segmentTextVisitor.GetText(activeSegment?.Target);
				displayFilterControl.target_textbox.Text = targetText;
				displayFilterControl.ApplyFilter(false);
			}
		}
	}
}
