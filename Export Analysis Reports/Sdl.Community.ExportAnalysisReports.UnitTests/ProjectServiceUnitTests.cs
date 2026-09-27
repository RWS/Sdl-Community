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

		[Theory]
		[InlineData(@"Project 2\Project2.sdlproj")]
		private void AddFiles_IsSuccessfullyAdded_UnitTest(string filePath)
		{
			// Arrange
			var projectPath = Path.Combine(_testingFilesPath, filePath);
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

		[Theory]
		[InlineData("English (United Kingdom)")]
		private void RemoveSingleFileLanguage_IsSuccessfullyRemoved_UnitTest(string languageName)
		{
			// Arrange
			var languagesToRemove = _exportReportsConfiguration.GetMultipleLanguages();
			var projectLanguages = _exportReportsConfiguration.AddLanguageDetails(languageName);

			// Act
			_projectService.RemoveSingleFileProjectLanguages(languagesToRemove, projectLanguages);

			// Assert
			Assert.True(languagesToRemove.Count.Equals(1));
		}

		[Theory]
		[InlineData("English (United Kingdom)")]
		private void RemoveSingleFileLanguage_IsNotSuccessfullyRemoved_UnitTest(string languageName)
		{
			// Arrange
			var languagesToRemove = _exportReportsConfiguration.GetProjectLanguage();
			var projectLanguages = _exportReportsConfiguration.AddLanguageDetails(languageName);

			// Act
			_projectService.RemoveSingleFileProjectLanguages(languagesToRemove, projectLanguages);

			// Assert
			Assert.NotEmpty(languagesToRemove);
		}

		[Theory]
		[InlineData(false, "Project 2", true, "English (United Kingdom)")]
		private void SetProjectLanguages_IsSuccessfullySet_UnitTest(bool isSingleFileProject, string projectName, bool isChecked, string languageName)
		{
			// Arrange
			var projectDetails = new List<ProjectDetails>();
			projectDetails.Add(_exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName));

			// Act
			_projectService.SetProjectLanguages(projectDetails, isChecked, languageName);
			var isLanguageChecked = projectDetails[0]?.ProjectLanguages[languageName];

			// Assert
			Assert.True(isLanguageChecked);
		}

		[Theory]
		[InlineData(true, "Project 2", false, "English (United Kingdom)")]
		private void SetProjectLanguages_IsNotSuccessfullySet_UnitTest(bool isSingleFileProject, string projectName, bool isChecked, string languageName)
		{
			// Arrange
			var projectDetails = new List<ProjectDetails>();
			projectDetails.Add(_exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName));

			// Act
			_projectService.SetProjectLanguages(projectDetails, isChecked, languageName);
			var isLanguageChecked = projectDetails[0]?.ProjectLanguages[languageName];

			// Assert
			Assert.False(isLanguageChecked);
		}

		[Theory]
		[InlineData(true, "Project 2")]
		private void RemoveAllSingleProjects_AreSuccessfullyRemoved_UnitTest(bool isSingleFileProject, string projectName)
		{
			// Arrange
			var projectDetails = new List<ProjectDetails>();
			projectDetails.Add(_exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName));

			// Act
			_projectService.RemoveAllSingleProjects(projectDetails);

			// Assert
			Assert.True(projectDetails.Count == 0);
		}

		[Theory]
		[InlineData(false, "Project 2")]
		private void RemoveAllSingleProjects_AreNotSuccessfullyRemoved_UnitTest(bool isSingleFileProject, string projectName)
		{
			// Arrange
			var projectDetails = new List<ProjectDetails>();
			projectDetails.Add(_exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName));

			// Act
			_projectService.RemoveAllSingleProjects(projectDetails);

			// Assert
			Assert.False(projectDetails.Count == 0);
		}

		[Fact]
		private void SetAllProjectDetails_AreSuccessfullySet_UnitTest()
		{
			// Arrange
			var projectsDetails = new List<ProjectDetails>();
			projectsDetails.Add(_exportReportsConfiguration.GetProjectDetails(false, "Project 2"));
			var projectDetails = _exportReportsConfiguration.GetProjectDetails(false, "Project 3");

			// Act
			_projectService.SetAllProjectDetails(projectsDetails, projectDetails);

			// Assert
			Assert.True(projectsDetails.Count > 1);
		}

		[Theory]
		[InlineData(false, "Project 2")]
		private void SetAllProjectDetails_AreNotSuccessfullySet_UnitTest(bool isSingleFileProject, string projectName)
		{
			// Arrange
			var projectsDetails = new List<ProjectDetails>();
			projectsDetails.Add(_exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName));
			var projectDetails = _exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName);

			// Act
			_projectService.SetAllProjectDetails(projectsDetails, projectDetails);

			// Assert
			Assert.False(projectsDetails.Count > 1);
		}
	}
}