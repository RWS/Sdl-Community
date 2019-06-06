using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace DSIViewerPoC
{

	[RibbonGroup("DsiViewer", Name = "", ContextByType = typeof(EditorController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class DisRibbon : AbstractRibbonGroup
	{
	}

	[Action("DsiViewer",
		Name = "DsiViewer",
		Icon = "",
		Description = "")]
	[ActionLayout(typeof(DisRibbon), 20, DisplayType.Large)]
	public class DsiViewerAppAbstractAction : AbstractAction
	{
		protected override void Execute()
		{
			var editorController = GetEditorController();

			editorController.ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			var doc = sender as Document;
			var segment = doc?.ActiveSegmentPair;
			var contexts = segment?.GetParagraphUnitProperties().Contexts;
			if (contexts?.Contexts?.Count > 0)
			{
				foreach (var context in contexts.Contexts)
				{
					var displayName = context.DisplayName;
					var code = context.DisplayCode;
					var description = context.Description;
				}
			}
		}

		private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}
	}
}
