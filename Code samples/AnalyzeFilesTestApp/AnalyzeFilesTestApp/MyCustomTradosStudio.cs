using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace AnalyzeFilesTestApp
{
	[Action("AnalyzeFilesTestApp")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			var projectInfo = new ProjectInfo
			{
				Name = "",
				SourceLanguage = new Language("en-US"),
				TargetLanguages = new[] { new Language("de-DE") },
				LocalProjectFolder = "",
			};

			var fileBasedProject = new FileBasedProject(projectInfo);

			var settings = fileBasedProject.GetSettings();
			var analysisSettings = settings.GetSettingsGroup<AnalysisTaskSettings>();

			analysisSettings.ReportCrossFileRepetitions.Value = false;
			fileBasedProject.UpdateSettings(settings);

			var projectFiles = fileBasedProject.AddFiles(new[] { "" });

			fileBasedProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
			fileBasedProject.RunDefaultTaskSequence(projectFiles.GetIds());

			fileBasedProject.Save();
		}
	}
}