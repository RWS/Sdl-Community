using System;
using System.IO;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace PretranslateProjectsCreatedFromTemplateSample
{
	public static class Program
	{
		private const string TemplatePath      = @"";
		private const string SourceFilesFolder = @"";
		private const string OutputFolder      = @"";

		private static void Main()
		{
			var template = new ProjectTemplateReference(TemplatePath);

			var projectInfo = new ProjectInfo
			{
				Name = new DirectoryInfo(SourceFilesFolder).Name + "_" + Guid.NewGuid(),
				LocalProjectFolder = Path.Combine(OutputFolder, new DirectoryInfo(SourceFilesFolder).Name)
			};

			var project = new FileBasedProject(projectInfo, template);
			project.AddFolderWithFiles(SourceFilesFolder, recursive: false);
			project.RunAutomaticTasks(
				project.GetSourceLanguageFiles().GetIds(),
                [
                    AutomaticTaskTemplateIds.Scan,
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages
                ]);

			project.Save();
		}
	}
}