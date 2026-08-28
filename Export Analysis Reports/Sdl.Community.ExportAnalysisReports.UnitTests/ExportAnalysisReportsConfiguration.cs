using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.UnitTests
{
	public class ExportAnalysisReportsConfiguration
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestFiles");

		public string GetTempExportFolder()
		{
			var tempExportFolder = $@"C:\Users\{Environment.UserName}\ExportAnalysisReports\ExportProjectFolder";

			if (Directory.Exists(tempExportFolder))
			{
				Directory.Delete(tempExportFolder, true);
			}
			Directory.CreateDirectory(tempExportFolder);
			return tempExportFolder;
		}

		public OptionalInformation GetOptionalInformation()
		{
			return new OptionalInformation
			{
				IncludeAdaptiveBaseline = false,
				IncludeAdaptiveLearnings = false,
				IncludeContextMatch = false,
				IncludeCrossRep = false,
				IncludeInternalFuzzies = false,
				IncludeLocked = false,
				IncludePerfectMatch = false
			};
		}

		public ProjectDetails GetProjectDetails(bool isSingleFileProject, string projectName)
		{
			return new ProjectDetails
			{
				Status = "In Progress",
				ProjectName = projectName,
				ProjectPath = Path.Combine(_testingFilesPath, $"{ projectName }.sdlproj"),
				ReportsFolderPath = Path.Combine(_testingFilesPath, $@"{projectName}\Reports"),
				ReportPath = null,
				IsSingleFileProject = isSingleFileProject,
				ProjectLanguages = GetProjectLanguage(),
				LanguageAnalysisReportPaths = AddLanguageAnalysisReportPaths(),
				ShouldBeExported = true
			};
		}

		public BindingList<LanguageDetails> AddLanguageDetails(string languageName)
		{
			return new BindingList<LanguageDetails> { new LanguageDetails { IsChecked = true, LanguageName = languageName } };
		}

		public Dictionary<string, bool> GetMultipleLanguages()
		{
			var projectLanguages = new Dictionary<string, bool>();
			projectLanguages.Add("English (United Kingdom)", true);
			projectLanguages.Add("Spanish (Spain International)", true);

			return projectLanguages;
		}

		public Dictionary<string, bool> GetProjectLanguage()
		{
			var projectLanguages = new Dictionary<string, bool>();
			projectLanguages.Add("English (United Kingdom)", true);
			return projectLanguages;
		}

		private Dictionary<string, string> AddLanguageAnalysisReportPaths()
		{
			var analyseReportPath = Path.Combine(_testingFilesPath, @"Project 2\Reports\Analyze Files en-US_en-GB.xml");
			var projectLanguages = new Dictionary<string, string>();
			projectLanguages.Add("English (United Kingdom)", analyseReportPath);
			return projectLanguages;
		}
	}
}
