using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TQA
{
	[RibbonGroup("TQAReporting_RibbonGroup_ProjectsView", Name = "RibbonGroup_Name", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TQAReportingRibbonGroupProjectsView : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TQAReporting_RibbonGroup_FilesView", Name = "RibbonGroup_Name", ContextByType = typeof(FilesController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TQAReportingRibbonGroupFilesView : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TQAReporting_RibbonGroup_EditorView", Name = "RibbonGroup_Name", ContextByType = typeof(EditorController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorReviewRibbonTabLocation))]
	public class TQAReportingRibbonGroupEditorView : AbstractRibbonGroup
	{
	}
}
