using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace IATETerminologyProvider
{
	[RibbonGroup("IATETermDefinition", Name = "IATE Term Definition")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class IATETerminologyProviderAction: AbstractRibbonGroup
	{
		[Action("IATETerminologyProvider", Name = "Go to IATE term definition", Icon = "Iate_logo")]
		[ActionLayout(typeof(IATETerminologyProviderAction), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
		public class IATETerminologyProviderTermDefinitionAction : AbstractAction
		{
			public EditorController GetEditorController()
			{
				return SdlTradosStudio.Application.GetController<EditorController>();
			}

			protected override void Execute()
			{
				var editorController = GetEditorController();
				var activeDocument = editorController != null ? editorController.ActiveDocument : null;
				var currentSelection = activeDocument != null
					? activeDocument.Selection != null
					? activeDocument.Selection.Current.ToString().TrimEnd()
					: string.Empty
					: string.Empty;

				if (!string.IsNullOrEmpty(currentSelection))
				{
					System.Diagnostics.Process.Start("https://iate.europa.eu/search/standard");
				}
			}
		}
	}
}