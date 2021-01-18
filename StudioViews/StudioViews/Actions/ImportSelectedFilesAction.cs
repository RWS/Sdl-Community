using System;
using System.Linq;
using System.Windows;
using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Community.StudioViews.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.StudioViews.Actions
{
	[Action("StudioViews_ImportToSelectedFilesAction_Action",
		Name = "StudioViews_ImportToSelectedFilesAction_Name",
		Description = "StudioViews_ImportToSelectedFilesAction_Description",
		ContextByType = typeof(FilesController),
		Icon = "StudioViewsImport_Icon"
	)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 0, DisplayType.Default, "", true)]
	public class ImportSelectedFilesAction : AbstractAction
	{
		private StudioViewsFilesSplitView _window;
		private FilesController _filesController;
		private ProjectsController _projectsController;

		public override void Initialize()
		{
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		protected override void Execute()
		{
			var selectedFiles = _filesController.SelectedFiles?.ToList();
			if (selectedFiles?.Count == 0)
			{
				MessageBox.Show("No files selected", "Studio Views", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			var sdlxliffMerger = new SdlxliffMerger();
			var sdlxliffExporter = new SdlxliffExporter();
			var sdlXliffReader = new SdlxliffReader();

			_window = new StudioViewsFilesSplitView();
			var model = new StudioViewsFilesSplitViewModel(_window, selectedFiles, 
				sdlxliffMerger, sdlxliffExporter, sdlXliffReader);

			_window.DataContext = model;
			_window.ShowDialog();
		}
	}
}
