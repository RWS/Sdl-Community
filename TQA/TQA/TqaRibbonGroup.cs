using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TQA
{
	[RibbonGroup("TqaRibbonGroup", Name = "TQA Reporting", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TqaRibbonGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TqaRibbonGroupReview", Name = "TQA Reporting", ContextByType = typeof(FilesController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TqaRibbonGroupFiles : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TqaRibbonGroupReview", Name = "TQA Reporting", ContextByType = typeof(EditorController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorReviewRibbonTabLocation))]
	public class TqaRibbonGroupReview : AbstractRibbonGroup
	{
	}
}
