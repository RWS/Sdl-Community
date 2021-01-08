using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using StudioViews.Services;
using StudioViews.View;
using StudioViews.ViewModel;

namespace StudioViews
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
		private StudioViewsEditorView _control = new StudioViewsEditorView();
		private EditorController _editorController;
		private ProjectsController _projectsController;
		
		protected override void Initialize()
		{
			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			var fileInfoService = new FileInfoService();
			var filterItemHelper = new FilterItemHelper();
			var projectHelper = new ProjectHelper(_projectsController);
			
			var model = new StudioViewsEditorViewModel(_editorController, fileInfoService, filterItemHelper, projectHelper);
			_control = new StudioViewsEditorView {DataContext = model};
		}

		protected override IUIControl GetContentControl()
		{
			return _control;
		}
	}
}
