using System;
using System.IO;
using PretranslateProjectsCreatedFromTemplateSample.Helpers;
using Sdl.ProjectAutomation.Core;

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
			for (var cycleNo = 0; cycleNo < cyclesTotal; cycleNo++)
			{
				for (var i = 1; i < args.Length - 2; i++)
				{
					using (var pretranslator = new Pretranslator())
					{
						_ = pretranslator.CreateProjectFromTemplate(template, args[i], $"{args[args.Length - 2]}");
						Console.WriteLine($@"Cycle {cycleNo + 1} - Project with files from ""{args[i]}"" created succesfully");
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