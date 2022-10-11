using System;
using System.IO;
using ProjectAutomation.Services;

namespace ProjectAutomation
{
	class Program
	{
		private static string _sourceFilesDirectory;
		private static string _projectsDirectory;
		private static string _sourceLanguage;
		private static string _targetLanguage;
		private static string _tmPath;

	
		static void Main(string[] args)
		{
			// TODO: Set location where to find the source files
			_sourceFilesDirectory = Path.Combine(@"C:\Code\Automation\AutomationTests", "SourceFiles", "en");

			// TODO: Set location for the project output folder... projects will be created here in sub-folders
			_projectsDirectory = Path.Combine(@"C:\Code\Automation\AutomationTests", "Projects");
			if (!Directory.Exists(_projectsDirectory))
			{
				Directory.CreateDirectory(_projectsDirectory);
			}

			// TODO: Set full path to the file based tm
			_tmPath = @"C:\Users\phartnett\Documents\Studio 2022\Translation Memories\it-IT.sdltm";

			// TODO: Set the source and target language combination
			_sourceLanguage = "en-US";
			_targetLanguage = "it-IT";
		
			
			CreateProject();

			Console.ReadLine();
		}



		private static void CreateProject()
		{
			var projectDirectory = GetOutputProjectDirectory(_projectsDirectory);

			using (var projectCreator = new ProjectCreator())
			{
				projectCreator.Create(_sourceFilesDirectory, projectDirectory,_sourceLanguage, _targetLanguage, _tmPath);
			}
		}

		private static string GetOutputProjectDirectory(string baseFolder)
		{
			var dateTimeString = GetDateTimeToString(DateTime.Now);
			var projectDirectory = Path.Combine(baseFolder, dateTimeString);

			if (!Directory.Exists(projectDirectory))
			{
				Directory.CreateDirectory(projectDirectory);
			}

			return projectDirectory;
		}

		private static string GetDateTimeToString(DateTime dateTime)
		{
			var value = dateTime.Year +
						dateTime.Month.ToString().PadLeft(2, '0') +
						dateTime.Day.ToString().PadLeft(2, '0') +
						"-" +
						dateTime.Hour.ToString().PadLeft(2, '0') +
						dateTime.Minute.ToString().PadLeft(2, '0') +
						dateTime.Second.ToString().PadLeft(2, '0');
			return value;
		}
	}
}
