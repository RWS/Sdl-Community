using Sdl.Community.AdvancedDisplayFilter.Controls;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdvancedDisplayFilter.Actions
{
	[Action("CommunityADFFilter", Name = "CADF - Selection Filter", Icon = "")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	public class FilterSelectionAction: AbstractAction
	{
		protected override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocument = editorController?.ActiveDocument;
			var displayFilterControl = CommunityApplicationInitializer.DisplayFilterControl;

			if (activeDocument!= null && displayFilterControl != null)
			{
				var sourceSelection = activeDocument.Selection?.Source?.ToString().TrimStart().TrimEnd();
				var targetSelection = activeDocument.Selection?.Target?.ToString().TrimStart().TrimEnd();

				if (!string.IsNullOrEmpty(sourceSelection))
				{
					displayFilterControl.textBox_source.Text = sourceSelection;
					displayFilterControl.ApplyFilter(false);
				}
				if (!string.IsNullOrEmpty(targetSelection))
				{
					displayFilterControl.target_textbox.Text = targetSelection;
					displayFilterControl.ApplyFilter(false);
				}
			}
		}
	}
}
