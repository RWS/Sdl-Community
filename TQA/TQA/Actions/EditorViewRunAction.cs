using NLog;
using Sdl.Community.TQA.Providers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TQA.Actions
{
	[Action(Constants.TQAReporting_EditorView_Run_Action_Id, Name = "ActionName_RunTQAReport", Icon = "tqa3")]
	[ActionLayout(typeof(TQAReportingRibbonGroupEditorView), 10, DisplayType.Large)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large, Name = "ActionName_RunTQAReporting")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentSpellcheckContextMenuLocation), 10, DisplayType.Large, Name = "ActionName_RunTQAReporting")]
	public class EditorViewRunAction : AbstractViewControllerAction<EditorController>
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
