using NLog;
using Sdl.Community.TQA.Providers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TQA.Actions
{
	[Action(Constants.TQAReporting_FilesView_Run_Action_Id, Name = "ActionName_RunTQAReport", Icon = "tqa3")]
	[ActionLayout(typeof(TQAReportingRibbonGroupFilesView), 10, DisplayType.Large)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 10, DisplayType.Large, Name = "ActionName_RunTQAReporting")]
	public class FilesViewRunAction : AbstractViewControllerAction<FilesController>
	{
		protected override void Execute()
		{
			var logger = LogManager.GetCurrentClassLogger();
			var reportProvider = new ReportProvider(logger);
			var categoriesProvider = new CategoriesProvider();
			var qualitiesProvider = new QualitiesProvider();

			var mw = new MainWindow(Controller, reportProvider, categoriesProvider, qualitiesProvider, logger);
			mw.Show(ApplicationInstance.GetActiveForm());
		}
	}
}
