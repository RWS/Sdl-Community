using Sdl.Community.AdvancedDisplayFilter.Controls;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdvancedDisplayFilter.Actions
{
	[Action("CommunityADFSourceFilter", Name = "CADF - Source Filter", Icon = "filter_source")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	public class FilterSourceAction:AbstractAction
	{
		private readonly SegmentTextVisitor _segmentTextVisitor = new SegmentTextVisitor();

		protected override void Execute()
		{
			var activeDocument = SdlTradosStudio.Application.GetController<EditorController>()?.ActiveDocument;
			var displayFilterControl = CommunityApplicationInitializer.DisplayFilterControl;
			if (activeDocument != null && displayFilterControl != null)
			{
				var activeSegment = activeDocument.ActiveSegmentPair;
				var sourceText = _segmentTextVisitor.GetText(activeSegment?.Source);
				displayFilterControl.textBox_source.Text = sourceText;
				displayFilterControl.ApplyFilter(false);
			}
		}
	}
}
