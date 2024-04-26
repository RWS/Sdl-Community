using System.Linq;
using Sdl.Community.StudioViews.Providers;
using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Community.StudioViews.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Versioning;

namespace Sdl.Community.StudioViews
{
	[ViewPart(
		Id = "StudioViewsEditorController",
		Name = "StudioViewsEditorController_Name",
		Description = "StudioViewsEditorController_Description",
		Icon = "StudioViewsMain_Icon"
	)]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Left)]
	public class StudioViewsEditorController : AbstractViewPartController
	{
		private StudioViewsEditorView _control;
		private StudioVersionService _studioVersionService;

		protected override void Initialize()
		{
			_studioVersionService = new StudioVersionService();

			var commonService = new ProjectFileService();
			var projectHelper = new ProjectService(ApplicationInstance.ProjectsController, _studioVersionService);
			var analysisBands = projectHelper.GetAnalysisBands(ApplicationInstance.ProjectsController.CurrentProject ?? ApplicationInstance.ProjectsController.SelectedProjects.FirstOrDefault());
			var filterItemService = new FilterItemService(analysisBands);
			var sdlxliffMerger = new SdlxliffMerger();
			var segmentBuilder = new SegmentBuilder();
			var segmentVisitor = new SegmentVisitor();
			var paragraphUnitProvider = new ParagraphUnitProvider(segmentVisitor, filterItemService, segmentBuilder);
			var sdlxliffExporter = new SdlxliffExporter(segmentBuilder);
			var sdlXliffReader = new SdlxliffReader();
			var displayFilter = new DisplayFilter();
			var wordCountProvider = new WordCountProvider();

			var model = new StudioViewsEditorViewModel(ApplicationInstance.EditorController, filterItemService,
				commonService, sdlxliffMerger, sdlxliffExporter, sdlXliffReader, paragraphUnitProvider, displayFilter, wordCountProvider);

			_control = new StudioViewsEditorView { DataContext = model };
		}

		protected override IUIControl GetContentControl()
		{
			return _control;
		}
	}
}
