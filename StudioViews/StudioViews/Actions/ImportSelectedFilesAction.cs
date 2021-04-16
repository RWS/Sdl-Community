using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Providers;
using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Community.StudioViews.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Versioning;
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
		private StudioVersionService _studioVersionService;

		public override void Initialize()
		{
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_studioVersionService = new StudioVersionService();
		}

		protected override void Execute()
		{
			var selectedFiles = _filesController.SelectedFiles.Where(projectFile => projectFile.Role == FileRole.Translatable).ToList();
			if (selectedFiles.Count == 0)
			{
				MessageBox.Show(PluginResources.Message_No_files_selected, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var missingFiles = selectedFiles.Any(file => file.LocalFileState == LocalFileState.Missing);
			if (missingFiles)
			{
				MessageBox.Show(PluginResources.Message_Missing_Project_Files_Download_From_Server, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var projectHelper = new ProjectService(_projectsController, _studioVersionService);
			var analysisBands = projectHelper.GetAnalysisBands(_projectsController.CurrentProject ?? _projectsController.SelectedProjects.FirstOrDefault());
			var filterItemHelper = new FilterItemService();
			var commonService = new ProjectFileService();
			var segmentVisitor = new SegmentVisitor();
			var paragraphUnitProvider = new ParagraphUnitProvider(segmentVisitor);
			var sdlxliffImporter = new SdlxliffImporter(commonService, filterItemHelper, analysisBands, paragraphUnitProvider);
			var sdlXliffReader = new SdlxliffReader();

			_window = new StudioViewsFilesImportView();
			var model = new StudioViewsFilesImportViewModel(_window, selectedFiles, commonService, filterItemHelper, sdlxliffImporter, sdlXliffReader);

			_window.DataContext = model;
			_window.ShowDialog();

			if (model.DialogResult != DialogResult.OK)
			{
				return;
			}

			OpenMessageWindow(model);
		}

		private static void OpenMessageWindow(StudioViewsFilesImportViewModel model)
		{
			var messageInfo = new MessageInfo
			{
				Title = PluginResources.Message_Title_Task_Result,
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
		}
	}
}
