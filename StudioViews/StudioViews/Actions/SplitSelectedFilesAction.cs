using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Services;
using Sdl.Community.StudioViews.View;
using Sdl.Community.StudioViews.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Versioning;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Sdl.Community.StudioViews.Actions
{
    [Action("StudioViews_SplitSelectedFiles_Action",
        Name = "StudioViews_SplitSelectedFiles_Name",
        Description = "StudioViews_SplitSelectedFiles_Description",
        ContextByType = typeof(FilesController),
        Icon = "StudioViewsSplit_Icon"
    )]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 0, DisplayType.Default, "", true)]
    public class SpitSelectedFilesAction : AbstractAction
    {
        private StudioVersionService _studioVersionService;
        private StudioViewsFilesSplitView _window;

        public override void Initialize() => _studioVersionService = new StudioVersionService();

        public void Run()
        {
            var selectedFiles = ApplicationInstance.FilesController.SelectedFiles.Where(projectFile => projectFile.Role == FileRole.Translatable).ToList();
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

            var project = ApplicationInstance.ProjectsController.CurrentProject ?? ApplicationInstance.ProjectsController.SelectedProjects.FirstOrDefault();
            var projectFileService = new ProjectFileService();
            var sdlxliffMerger = new SdlxliffMerger();
            var segmentBuilder = new SegmentBuilder();
            var sdlxliffExporter = new SdlxliffExporter(segmentBuilder);
            var sdlXliffReader = new SdlxliffReader();
            var wordCountProvider = new WordCountProvider();

            var projectHelper = new ProjectService(ApplicationInstance.ProjectsController, _studioVersionService);
            var analysisBands = projectHelper.GetAnalysisBands(ApplicationInstance.ProjectsController.CurrentProject ?? ApplicationInstance.ProjectsController.SelectedProjects.FirstOrDefault());
            var filterItemService = new FilterItemService(analysisBands);

            _window = new StudioViewsFilesSplitView();
            var model = new StudioViewsFilesSplitViewModel(_window, project, selectedFiles, projectFileService, filterItemService,
                sdlxliffMerger, sdlxliffExporter, sdlXliffReader, wordCountProvider);

            _window.DataContext = model;
            _window.ShowDialog();

            if (model.DialogResult != DialogResult.OK)
            {
                return;
            }

            OpenMessageWindow(model);
        }

        protected override void Execute()
        {
            Run();
        }

        private static void OpenMessageWindow(StudioViewsFilesSplitViewModel model)
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