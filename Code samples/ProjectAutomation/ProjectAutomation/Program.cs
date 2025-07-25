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
			var sourceLanguage = "en-US"; // Change this to match the source language
			var targetLanguage = "de-DE"; // Change this to match the target language

			// TODO: Set the memory resource
			var memory = new MemoryResource
			{
				// Example: connect to a FileBased TM 
                // Path = @"C:\...\Projects\Samples\SampleProject\TMs\English-German.sdltm",

                // Example: connect to deepL provider (https://appstore.rws.com/Plugin/24)
                // Uri = new Uri("deepltranslationprovider:///"),
                // UserNameOrClientId = "[your DeepL account e-mail]",
                // Credential = "[your DeepL account API key]"

                // Example: connect to Groupshare resource
                // Uri = new Uri("sdltm.http://URL/?orgPath=/Appstore&tmName=TM_NAME"),
                // UserNameOrClientId = "",
                // UserPasswordOrClientSecret = "",
                // IsWindowsUser = false
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
