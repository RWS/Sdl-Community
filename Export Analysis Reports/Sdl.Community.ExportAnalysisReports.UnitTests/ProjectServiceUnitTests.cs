using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.Community.ExportAnalysisReports.Service;
using Xunit;

namespace Sdl.Community.ExportAnalysisReports.UnitTests
{
	public class ProjectServiceUnitTests
    {
		private ExportAnalysisReportsConfiguration _exportReportsConfiguration;
		private readonly ProjectService _projectService;
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestFiles");

		public ProjectServiceUnitTests()
		{
			_projectService = Substitute.For<ProjectService>();
			_exportReportsConfiguration = new ExportAnalysisReportsConfiguration();
		}

		[Fact]
		private void AddFiles_IsSuccessfullyAdded_UnitTest()
		{
			// Arrange
			var projectPath = Path.Combine(_testingFilesPath, @"Project 2\Project2.sdlproj");
			var studioProjectsPath = new List<string>();
			studioProjectsPath.Add(projectPath);
			
			// Act
			var files = _projectService.AddFilePaths(studioProjectsPath);

			// Assert
			Assert.NotEmpty(files);
		}

		[Fact]
		private void AddFiles_IsNotSuccessfullyAdded_UnitTest()
		{
			// Arrange
			var studioProjectsPath = new List<string>();

			// Act
			var files = _projectService.AddFilePaths(studioProjectsPath);
			
			// Assert
			Assert.Empty(files);
		}

		[Fact]
		private void RemoveSingleFileLanguage_IsSuccessfullyRemoved_UnitTest()
		{
			// Arrange
			var languagesToRemove = _exportReportsConfiguration.AddMultipleLanguages();
			var projectLanguages = _exportReportsConfiguration.AddLanguageDetails("English (United Kingdom)");

			// Act
			_projectService.RemoveSingleFileProjectLanguages(languagesToRemove, projectLanguages);

			// Assert
			Assert.True(languagesToRemove.Count.Equals(1));
		}

		[Fact]
		private void RemoveSingleFileLanguage_IsNotSuccessfullyRemoved_UnitTest()
		{
			// Arrange
			var languagesToRemove = _exportReportsConfiguration.AddProjectLanguage();
			var projectLanguages = _exportReportsConfiguration.AddLanguageDetails("English (United Kingdom)");

			// Act
			_projectService.RemoveSingleFileProjectLanguages(languagesToRemove, projectLanguages);

			// Assert
			Assert.NotEmpty(languagesToRemove);
		}

		[Fact]
		private void SetProjectLanguages_IsSuccessfullySet_UnitTest()
		{
			// Arrange
			var projectDetails = new List<ProjectDetails>();
			projectDetails.Add(_exportReportsConfiguration.GetProjectDetails());
			
			// Act
			_projectService.SetProjectLanguages(projectDetails, true, "English (United Kingdom)");
			var isLanguageChecked = projectDetails[0]?.ProjectLanguages["English (United Kingdom)"];

			// Assert
			Assert.True(isLanguageChecked);
		}

		[Fact]
		private void SetProjectLanguages_IsNotSuccessfullySet_UnitTest()
		{
			// Arrange
			var projectDetails = new List<ProjectDetails>();
			projectDetails.Add(_exportReportsConfiguration.GetProjectDetails());

			// Act
			_projectService.SetProjectLanguages(projectDetails, false, "English (United Kingdom)");
			var isLanguageChecked = projectDetails[0]?.ProjectLanguages["English (United Kingdom)"];

			// Assert
			Assert.False(isLanguageChecked);
		}
	}
}