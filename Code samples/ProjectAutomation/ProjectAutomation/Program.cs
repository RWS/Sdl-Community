using System;
using System.IO;
using ProjectAutomation.Services;

namespace ProjectAutomation
{
	class Program
	{
		static void Main(string[] args)
		{
			// TODO: Set location where to find the source files
			var sourceFilesDirectory = @"c:\temp\source files folder";

			// TODO: Set location for the project output folder... projects will be created here in sub-folders
			var projectsDirectory = @"c:\temp\projects";
			if (!Directory.Exists(projectsDirectory))
			{
				Directory.CreateDirectory(projectsDirectory);
			}

			// TODO: Set the source and target language combination
			var sourceLanguage = "en-US";
			var targetLanguage = "it-IT";

			// TODO: Set the memory resource
			var memory = new MemoryResource
			{
				Path = @"c:\temp\mytm.sdltm",
				//Uri = new Uri("sdlmtcloud:///"),
				//UserNameOrClientId = "TODO: UserName Or ClientID",
				//UserPasswordOrClientSecret = "TODO: UserPassword Or ClientSecret"
			};

			CreateProject(sourceFilesDirectory, projectsDirectory, sourceLanguage, targetLanguage, memory);

			Console.WriteLine("Done!");
			Console.ReadLine();
		}



		private static void CreateProject(string sourceFilesDirectory, string projectsDirectory,
			string sourceLanguage, string targetLanguage, MemoryResource memory)
		{
			var projectDirectory = GetOutputProjectDirectory(projectsDirectory);

			using (var projectCreator = new ProjectCreator())
			{
				projectCreator.Create(sourceFilesDirectory, projectDirectory, sourceLanguage, targetLanguage, memory);
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
