using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PretranslateProjectsCreatedFromTemplateSample.Helpers;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace PretranslateProjectsCreatedFromTemplateSample
{
	public static class PretranslateProjectsCreatedFromTemplateSample
	{
		private static FileBasedProject CreateProjectFromTemplate(string templatePath, string pathToFiles, string newProjectPath)
		{
			var template = new ProjectTemplateReference(templatePath);
			var newProjectDirectory = DirectoryHelper.GetDestinationDirectory(newProjectPath);

			var projectInfo = new ProjectInfo
			{
				Name = newProjectDirectory.Name,
				LocalProjectFolder = newProjectDirectory.FullName
			};

			var newProject = new FileBasedProject(projectInfo, template);
			newProject.AddFolderWithFiles(pathToFiles, false);

			_ = newProject.RunAutomaticTasks(
				newProject.GetSourceLanguageFiles().GetIds(),
				new[]
				{
					AutomaticTaskTemplateIds.Scan,
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages
				});
			newProject.Save();

			return newProject;
		}

		/// <param name="args">
		/// <list>
		/// <item>args[0] - path to the template</item>
		/// <item>args[1] - path to the translatable files</item>
		/// <item>args[2] - path to where the project will be created</item>
		/// </list>
		/// </param>
		private static void Main(string[] args)
		{
			if (args.Length == 0) return;

			var firstArgExtension = Path.GetExtension(args[0]);
			if (firstArgExtension != ".sdltpl") return;
			if (args.Length < 3) return;

			Console.Clear();
			var newProjects = new List<FileBasedProject>();

			var timer = new Stopwatch();
			timer.Start();
			for (var i = 0; i < 10; i++)
			{
				var newProject = CreateProjectFromTemplate(args[0], args[1], args[2]);
				newProjects.Add(newProject);

				if (i == 0) Console.Clear();

				Console.WriteLine($"Project {i + 1} created succesfully");
			}
			timer.Stop();

			Console.WriteLine($"Project creation duration: {timer.Elapsed}");

			timer.Restart();

			var tasks = new List<AutomaticTask>();
			for (var i = 0; i < newProjects.Count; i++)
			{
				var task = TranslationHelper.RunPretranslation(newProjects[i].FilePath);

				//use this if you want to test with TPs
				//var task = Translator.TranslateProject(newProjects[i].FilePath, null, new Credentials
				//{
				//	Uri = new Uri("providerUri:///"),
				//	ApiKey = "providerKey"
				//});
				tasks.Add(task);

				var message = task.Messages.Any() ? task.Messages[0].Message : "Completed succesfully";
				Console.WriteLine($"Project{i + 1}: {message}");
			}
			timer.Stop();

			if (tasks.Any(t => t.Messages.Any()))
			{

			}

			Console.WriteLine(tasks.Any(t => t.Messages.Any()) ? "Some task(s) failed" : "Everything ok");
			Console.WriteLine($"Pretranslation duration: {timer.Elapsed}");
		}
	}
}