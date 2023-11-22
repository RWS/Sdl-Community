using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PretranslateProjectsCreatedFromTemplateSample.Helpers;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace PretranslateProjectsCreatedFromTemplateSample
{
	public static class PretranslateProjectsCreatedFromTemplateSample
	{
		private static void Main(string[] args)
		{
			if (args.Length < 4) return;

			if (!TryGetTemplate(args[0], out var template)) return;
			if (!int.TryParse(args[args.Length - 1], out var cyclesTotal)) return;

			Run(args, template, cyclesTotal);
		}

		private static void Run(string[] args, ProjectTemplateReference template, int cyclesTotal)
		{
			var pretranslator = new Pretranslator();
			var projects = new List<FileBasedProject>();
			for (var i = 1; i < args.Length - 2; i++)
			{
				projects.Add(pretranslator.CreateProjectFromTemplate(template, args[i], $"{args[args.Length - 2]}"));
				Console.WriteLine($@"Project with files from ""{args[i]}"" created succesfully");
			}

			for (var cycleNo = 0; cycleNo < cyclesTotal; cycleNo++)
			{
				foreach (var project in projects)
				{
					using (var pretranslator2 = new Pretranslator())
					{
						var task = pretranslator2.Pretranslate(project);
						project.Save();

						var message = "";
						if (task.Status == TaskStatus.Failed)
							message = string.Join(Environment.NewLine, task.Messages.Select(m => m.Message));
						Console.WriteLine($"Pretranslation {cycleNo + 1}: {task.Status} {message}");
					}
				}
			}
		}

		private static bool TryGetTemplate(string templatePath, out ProjectTemplateReference template)
		{
			template = null;

			if (File.Exists(templatePath) && Path.GetExtension(templatePath) == ".sdltpl")
			{
				template = new ProjectTemplateReference(templatePath);
				return true;
			}

			return false;
		}
	}
}