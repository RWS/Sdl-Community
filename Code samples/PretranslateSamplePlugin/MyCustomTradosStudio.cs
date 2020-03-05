using System;
using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace PretranslateSamplePlugin
{
	[Action("PretranslateSamplePlugin")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			var projectInfo = new ProjectInfo
			{
				Name = "",
				SourceLanguage = new Language(""),
				TargetLanguages = new[] { new Language("") },
				LocalProjectFolder = @"",
			};

			var fileBasedProject = new FileBasedProject(projectInfo);
			AddServerTm(fileBasedProject, "", "", "", false, "",
				"");

			var projectFiles = fileBasedProject.AddFiles(new[] { @"" });

			fileBasedProject.RunAutomaticTasks(projectFiles.GetIds(), new[]
			{
				AutomaticTaskTemplateIds.Scan,
				AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
				AutomaticTaskTemplateIds.CopyToTargetLanguages,
				AutomaticTaskTemplateIds.PreTranslateFiles,
			});

			fileBasedProject.Save();
		}

		private void AddServerTm(FileBasedProject project, string serverAddress, string organizationPath, string tmName, bool useWindowsSecurity, string username, string password)
		{
			var tmAddress = new Uri($"sdltm.{serverAddress}{organizationPath}/{tmName}");
			var config = project.GetTranslationProviderConfiguration();

			var tm = new TranslationProviderCascadeEntry
			(
				new TranslationProviderReference(tmAddress),
				true,
				true,
				false
			);
			config.Entries.Add(tm);
			project.UpdateTranslationProviderConfiguration(config);

			project.Credentials.AddCredential(new Uri(serverAddress),
				$"user={username};password={password};type={(useWindowsSecurity ? "WindowsUser" : "CustomUser")}");
			project.UpdateTranslationProviderConfiguration(config);
		}
	}
}
