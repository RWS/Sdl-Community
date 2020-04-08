using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NSubstitute;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class ProjectServiceUnitTests
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly string _tempTestFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"StarTransitTest\StarTransitProject");

		private PackageModel _packageModel;
		private ProjectService _projectService;
		private StarTransitConfiguration _starTransitConfiguration;
		private IFileTypeManager _fileTypeManager;
		public ProjectServiceUnitTests()
		{
			_fileTypeManager = Substitute.For<IFileTypeManager>();

			_projectService = Substitute.For<ProjectService>(_fileTypeManager);
			var packageService = Substitute.For<PackageService>();
			_starTransitConfiguration = new StarTransitConfiguration(packageService);
		}

		[Fact]
		public void CreateProject_IsSuccessfullyCreated_Test()
		{
			_packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			if(_packageModel !=null)
			{
				_packageModel.ProjectTemplate = _starTransitConfiguration.SetTemplateInfo(_testingFilesPath, "Default.sdltpl");
			}

			var projectInfo = SetProjectInfo();

			var studioProj = Substitute.For<IProject>();
			studioProj.GetProjectInfo().Returns(projectInfo);
			_projectService.CreateNewProject(Arg.Any<ProjectInfo>(), Arg.Any<ProjectTemplateReference>()).Returns(studioProj);
			
			var result = _projectService.CreateProject(_packageModel);
			Assert.Equal("Project was successfully created!", result?.Message);
		}

		private ProjectInfo SetProjectInfo()
		{
			var projectInfo = new ProjectInfo
			{
				Name = _packageModel.Name,
				LocalProjectFolder = Path.Combine(_tempTestFolder, _packageModel.Name),
				SourceLanguage = new Language(_packageModel.LanguagePairs[0].SourceLanguage),
				TargetLanguages = GetTargetLanguages(_packageModel.LanguagePairs),
				DueDate = _packageModel.DueDate
			};
			return projectInfo;
		}

		private Language[] GetTargetLanguages(List<LanguagePair> languagePairs)
		{
			var targetCultureInfoList = new List<CultureInfo>();
			foreach (var pair in languagePairs)
			{
				var targetLanguage = pair.TargetLanguage;
				targetCultureInfoList.Add(targetLanguage);
			}
			var targetLanguageList = new List<Language>();
			foreach (var target in targetCultureInfoList)
			{
				var language = new Language(target);
				targetLanguageList.Add(language);
			}
			var targetArray = targetLanguageList.ToArray();
			return targetArray;
		}
	}
}