using System;
using System.IO;
using NSubstitute;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class ProjectServiceUnitTests
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly string _tempTestFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"StarTransitTest\StarTransitProject");

		private PackageModel _packageModel;
		private readonly ProjectService _projectService;
		private readonly StarTransitConfiguration _starTransitConfiguration;

		public ProjectServiceUnitTests()
		{
			var fileTypeManager = Substitute.For<IFileTypeManager>();
			_projectService = Substitute.For<ProjectService>(fileTypeManager, null);
			var packageService = Substitute.For<PackageService>();
			_starTransitConfiguration = new StarTransitConfiguration(packageService);
		}

		[Fact]
		public void CreateProject_IsSuccessfullyCreated_Test()
		{
			// Arrange
			ConfigureProject(true, "Project was successfully created!");

			// Act
			var result = _projectService?.CreateProject(_packageModel);

			// Assert: the method returns what we expected
			Assert.Equal("Project was successfully created!", result?.Message);
		}

		[Fact]
		public void CreateProject_IsNotSuccessfullyCreated_Test()
		{
			// Arrange
			ConfigureProject(true, "Project was not created correctly because no target files were found in the package!");

			// Act
			var result = _projectService?.CreateProject(_packageModel);

			// Assert: the method returns what we expected
			Assert.NotEqual("Project was successfully created!", result?.Message);
		}

		private void ConfigureProject(bool isCreated, string message)
		{
			_packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			if (_packageModel != null)
			{
				_packageModel.ProjectTemplate = _starTransitConfiguration.SetTemplateInfo(_testingFilesPath, "Default.sdltpl");
			}

			var projectInfo = _starTransitConfiguration?.SetProjectInfo(_packageModel, _tempTestFolder);
			var messageModel = _starTransitConfiguration?.SetMessageModel(isCreated, message);
			
			// substituting the implementation so we can test the CreateProject action
			var studioProj = Substitute.For<IProject>();
			studioProj?.GetProjectInfo().Returns(projectInfo);
			_projectService?.CreateNewProject(Arg.Any<ProjectInfo>(), Arg.Any<ProjectTemplateReference>()).Returns(studioProj);
			_projectService?.UpdateProjectSettings(Arg.Any<IProject>()).Returns(messageModel);
		}
	}
}