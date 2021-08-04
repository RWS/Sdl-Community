using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace PreTranslateDeepLSample
{
	[Action("PreTranslateSample")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio: AbstractAction
	{
		protected override void Execute()
		{
			var selectedProject= SdlTradosStudio.Application?.GetController<ProjectsController>()?.SelectedProjects?.FirstOrDefault();
			if (selectedProject != null)
			{
				var settings = selectedProject.GetSettings();
				var preTranslateSettings = settings.GetSettingsGroup<TranslateTaskSettings>();
				preTranslateSettings.NoTranslationMemoryMatchFoundAction.Value = NoTranslationMemoryMatchFoundAction.ApplyAutomatedTranslation;
				preTranslateSettings.MinimumMatchScore.Value = 75;
				selectedProject.UpdateSettings(settings);
				selectedProject.Save();
				var targetFiles = selectedProject.GetTargetLanguageFiles();

				selectedProject.RunAutomaticTasks(targetFiles.GetIds(), new[]
				{
					AutomaticTaskTemplateIds.PreTranslateFiles,
				});
				selectedProject.Save();
			}
		}
	}
}
