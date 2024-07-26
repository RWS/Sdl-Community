using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Providers;
using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Community.StudioViews.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Versioning;

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

		public void Execute(List<SystemFileInfo> importFiles, Language langauge)
		{
			_filesController.Activate();

			Run(importFiles, langauge);
		}

		protected override void Execute()
		{
			Run(null, _filesController.CurrentSelectedLanguage);
		}

		private void Run(IReadOnlyCollection<SystemFileInfo> importFiles, Language language)
		{
			var projectHelper = new ProjectService(_projectsController, _studioVersionService);
			var analysisBands = projectHelper.GetAnalysisBands(_projectsController.CurrentProject ?? _projectsController.SelectedProjects.FirstOrDefault());
			var filterItemService = new FilterItemService(analysisBands);
			var commonService = new ProjectFileService();
			var segmentVisitor = new SegmentVisitor();
			var segmentBuilder = new SegmentBuilder();
			var paragraphUnitProvider = new ParagraphUnitProvider(segmentVisitor, filterItemService, segmentBuilder);
			var sdlxliffImporter = new SdlxliffImporter(commonService, filterItemService, paragraphUnitProvider, segmentBuilder);
			var sdlXliffReader = new SdlxliffReader();

			_window = new StudioViewsFilesImportView();
			var model = new StudioViewsFilesImportViewModel(_window, _filesController, language, commonService, filterItemService, sdlxliffImporter, sdlXliffReader);

			_window.DataContext = model;
			if (importFiles != null)
			{
				model.AddFiles(importFiles);
			}

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
