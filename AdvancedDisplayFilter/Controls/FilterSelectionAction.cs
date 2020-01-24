using System;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.AdvancedDisplayFilter.Models;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
	[Action("CommunityADFFilter", Name = "CADF - Filter", Icon = "")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	public class FilterSelectionAction: AbstractAction
	{
		protected override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocument = editorController?.ActiveDocument;
			if (activeDocument != null)
			{
				var selection = activeDocument.Selection;
				var sourceSelection = activeDocument.Selection?.Source?.ToString().TrimStart().TrimEnd();
				var targetSelection = activeDocument.Selection?.Target?.ToString().TrimStart().TrimEnd();
				var contentFilterSettings = new ContentFilterSettings();

				if (!string.IsNullOrEmpty(sourceSelection))
				{
					contentFilterSettings.SelectedText = sourceSelection;
					contentFilterSettings.SearchInSource = true;
					var filterContent = new ContentDisplayFilter(contentFilterSettings);
					activeDocument.ApplyFilterOnSegments(filterContent);
				}
				if (!string.IsNullOrEmpty(targetSelection))
				{
					contentFilterSettings.SearchInSource = false;
					contentFilterSettings.SelectedText = targetSelection;
					var filterContent = new ContentDisplayFilter(contentFilterSettings);
					activeDocument.ApplyFilterOnSegments(filterContent);
				}
			}
		}
	}
}
