//using System.Linq;
//using Sdl.Desktop.IntegrationApi;
//using Sdl.Desktop.IntegrationApi.Extensions;
//using Sdl.ProjectAutomation.Core;
//using Sdl.TranslationStudioAutomation.IntegrationApi;

//namespace CaptureQARuleState.Actions;

//[Action(nameof(CreateReportAction), Icon = "CreateReport", Name = "Create Verification Report",
//    Description = "", ContextByType = typeof(ProjectsController))]
//[ActionLayout(typeof(RibbonGroup), 10, DisplayType.Large)]
//public class CreateReportAction : AbstractAction
//{
//    protected override void Execute()
//    {
//        var controller = SdlTradosStudio.Application.GetController<ProjectsController>();
//        var selectedProject = controller.SelectedProjects.Count() == 1
//            ? controller.SelectedProjects.First()
//            : controller.CurrentProject;


//        var sourceFiles = selectedProject.GetTargetLanguageFiles();
//        var sourceFilesIds = sourceFiles.GetIds();

//        var result = selectedProject.RunAutomaticTask(sourceFilesIds, AutomaticTaskTemplateIds.VerifyFiles);
//        var reportId = result.Reports.First().Id;

//        selectedProject.SaveTaskReportAs(reportId, @"C:\Things\Jira tasks\Tests\Report.html", ReportFormat.Html);
//    }
//}