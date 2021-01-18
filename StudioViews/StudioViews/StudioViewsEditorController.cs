using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Community.StudioViews.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioViews
{
	[ViewPart(
		Id = "StudioViewsEditorController",
		Name = "StudioViewsEditorController_Name",
		Description = "StudioViewsEditorController_Description",
		Icon = "StudioViewsMain_Icon"
	)]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Left)]
	public class StudioViewsEditorController: AbstractViewPartController
	{
		private StudioViewsEditorView _control;
		private EditorController _editorController;
		private ProjectsController _projectsController;
		
		protected override void Initialize()
		{
			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			var fileInfoService = new FileInfoService();
			var commonService = new CommonService(fileInfoService);
			var filterItemHelper = new FilterItemHelper();
			var projectHelper = new ProjectHelper(_projectsController);
			var sdlxliffMerger = new SdlxliffMerger();
			var sdlxliffExporter = new SdlxliffExporter();
			var sdlXliffReader = new SdlxliffReader();
			
			var model = new StudioViewsEditorViewModel(_editorController, filterItemHelper, projectHelper,
				commonService, sdlxliffMerger, sdlxliffExporter, sdlXliffReader);
			
			_control = new StudioViewsEditorView {DataContext = model};
		}

		protected override IUIControl GetContentControl()
		{
			return _control;
		}
	}
}
