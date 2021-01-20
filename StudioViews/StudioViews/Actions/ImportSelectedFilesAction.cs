using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Community.StudioViews.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using MessageBox = System.Windows.MessageBox;

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
		private StudioViewsFilesImportView _window;
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
			if (selectedFiles == null || selectedFiles.Count == 0)
			{
				MessageBox.Show("No files selected", "Studio Views", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var projectHelper = new ProjectService(_projectsController);
			var analysisBands = projectHelper.GetAnalysisBands(_projectsController.CurrentProject ?? _projectsController.SelectedProjects.FirstOrDefault());
			var filterItemHelper = new FilterItemService();
			var commonService = new ProjectFileService();
			var sdlxliffImporter = new SdlxliffImporter(commonService, filterItemHelper, analysisBands);
			var sdlXliffReader = new SdlxliffReader();

			_window = new StudioViewsFilesImportView();
			var model = new StudioViewsFilesImportViewModel(_window, selectedFiles, commonService, filterItemHelper, sdlxliffImporter, sdlXliffReader);

			_window.DataContext = model;
			_window.ShowDialog();

			if (model.DialogResult != DialogResult.OK)
			{
				return;
			}
			
			_window.Dispatcher.Invoke(
				delegate
				{
					var messageInfo = new MessageInfo
					{
						Title = "Task Result",
						Message = model.Message,
						LogFilePath = model.LogFilePath,
						Folder = model.ExportPath,
						ShowImage = true,
						ImageUrl = model.Success
							? "/Sdl.Community.StudioViews;component/Resources/information.png"
							: "/Sdl.Community.StudioViews;component/Resources/warning.png"
					};

					var messageView = new MessageBoxView();
					var messageViewModel = new MessageBoxViewModel(messageView, messageInfo);
					messageView.DataContext = messageViewModel;

					messageView.ShowDialog();
				});
		}
	}
}
