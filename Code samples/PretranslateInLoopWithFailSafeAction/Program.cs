using System;
using PretranslateInLoopWithFailSafeAction.Helpers;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace PretranslateInLoopWithFailSafeAction
{
	internal class Program
	{
		private static AutomaticTask Pretranslate(FileBasedProject project)
		{
			return project.RunAutomaticTask(
				project.GetTargetLanguageFiles().GetIds(), AutomaticTaskTemplateIds.PreTranslateFiles);
		}

		private static void Main(string[] args)
		{
			if (!int.TryParse(args[args.Length - 1], out var cyclesTotal)) return;

			var projectPath = args[0];
			var destinationPath = args[1];

			Run(projectPath, destinationPath, cyclesTotal);
		}

		private static void Run(string projectPath, string destinationPath, int cyclesTotal, int cycleNo = 0)
		{
			destinationPath = DirectoryHelper.EnsurePathExists(destinationPath);
			DirectoryHelper.DirectoryCopy(projectPath, destinationPath);

			var newProjectPath = DirectoryHelper.GetSdlproj(destinationPath);
			var project = new FileBasedProject(newProjectPath);
			for (; cycleNo < cyclesTotal; cycleNo++)
			{
				var task = Pretranslate(project);
				RunFailSafeAction(projectPath, ref project, ref task);
				project.Save();

				Console.WriteLine($"Pretranslation {cycleNo + 1}: {task.Status}");
			}
		}

		private static void RunFailSafeAction(string originalProjectPath, ref FileBasedProject project, ref AutomaticTask task)
		{
			if (task.Status == TaskStatus.Failed)
			{
				Console.WriteLine("Pretranslation failed. Retrying...");

				var filePath = project.FilePath;
				project.Delete();

				DirectoryHelper.DirectoryCopy(originalProjectPath, filePath);

				project = new FileBasedProject(filePath);
				task = Pretranslate(project);
			}
		}
	}
}