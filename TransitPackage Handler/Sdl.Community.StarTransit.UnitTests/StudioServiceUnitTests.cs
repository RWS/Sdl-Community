using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class StudioServiceUnitTests
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly IStudioService _studioService;
		public StudioServiceUnitTests()
		{
			_studioService = new StudioService(new ProjectsControllerServiceStub());
		}

		[Fact]
		public void GetProjectTemplates_ReturnsTemplatesOrderedByName()
		{
			var controllerService = new ProjectsControllerServiceStub
			{
				Templates =
				{
					new ProjectTemplateInfo { Name = "Zulu" },
					new ProjectTemplateInfo { Name = "Alpha" },
					new ProjectTemplateInfo { Name = "Mike" }
				}
			};

			var templates = new StudioService(controllerService).GetProjectTemplates();

			Assert.Equal(new[] { "Alpha", "Mike", "Zulu" }, templates.Select(t => t.Name));
		}

		[Fact]
		public void GetProjectTemplates_StudioHasNoProjectsController_ReturnsEmpty()
		{
			// ProjectsControllerService returns null when Studio did not hand out a ProjectsController
			var templates = new StudioService(new ProjectsControllerServiceStub { Templates = null }).GetProjectTemplates();

			Assert.Empty(templates);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("randomPath")]
		public async Task ReadTemplateData_EmptyPath_ReturnsNull(string templatePath)
		{
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(templatePath,null,null);
			Assert.Null(templateInfo);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl", @"C:\Users\aghisa\Desktop\files")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsCorrectProjectLocation(string templateName,string projectLocation)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Equal(projectLocation,templateInfo.Location);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl", "Automotive")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsCorrectCustomer(string templateName, string selectedCustomerName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.NotNull(templateInfo.Customer);
			Assert.Equal(selectedCustomerName, templateInfo.Customer.Name);
		}

		[Theory]
		[InlineData("MultilingualNoOptions.sdltpl")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsNullCustomer(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Null(templateInfo.Customer);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl", "2021-05-15T00:00:00+03:00")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsDueDate(string templateName, string templateDueDate)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Equal(DateTime.Parse(templateDueDate), templateInfo.DueDate);
		}
		[Theory]
		[InlineData("Default.sdltpl")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsNull(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Null(templateInfo);
		}

		[Theory]
		[InlineData("Test Multilingual Package Trados Plugin.de-en.sdltm", "de-DE", "en-GB,fr-FR")]
		public void IsTmCreatedFromPlugin_ReturnsTrue(string tmName, string sourceLanguageCode,
			string targetLanguageCodes)
		{
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var (isCreatedFromPlugin, language) = _studioService.IsTmCreatedFromPlugin(tmName,
				new CultureInfo(sourceLanguageCode), targetLanguages.ToArray());

			Assert.True(isCreatedFromPlugin);
			Assert.Equal(new Language("en-GB"),language);
		}

		[Theory]
		[InlineData("TestTransitTM.sdltm", "de-DE", "en-GB,fr-FR")]
		public void IsTmCreatedFromPlugin_ReturnsFalse(string tmName, string sourceLanguageCode,
			string targetLanguageCodes)
		{
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var (isCreatedFromPlugin, language) = _studioService.IsTmCreatedFromPlugin(tmName,
				new CultureInfo(sourceLanguageCode), targetLanguages.ToArray());

			Assert.False(isCreatedFromPlugin);
			Assert.Null(language);
		}

		[Theory]
		[InlineData("TestTransitTM.sdltm")]
		public void IsTmCreatedFromPlugin_NoLanguages_ReturnsFalse(string tmName)
		{

			var (isCreatedFromPlugin, language) = _studioService.IsTmCreatedFromPlugin(tmName,null, null);

			Assert.False(isCreatedFromPlugin);
			Assert.Null(language);
		}

		[Theory]
		[InlineData("TestTransitTM.sdltm", "en-US", "en-GB,fr-FR")]
		public void TmSupportsAnyLanguageDirection_CorrectSourceButNoMatchingTarget_ReturnsFalse(string tmName, string packageSourceLanguageCode,
			string packageTargetLanguageCodes)
		{
			var uri = new Uri($"{Path.Combine(_testingFilesPath, tmName)}");
			var targetLanguages = GetStudioLanguages(packageTargetLanguageCodes);

			var (isSupported, language) = _studioService.TmSupportsAnyLanguageDirection(uri, new CultureInfo(packageSourceLanguageCode), targetLanguages);
			Assert.False(isSupported);
			Assert.Null(language);
		}

		[Theory]
		[InlineData("TestTransitTM.sdltm", "de-DE", "en-GB,id-ID")]
		public void TmSupportsAnyLanguageDirection_ReturnsNull_IncorrectSourceLanguage_CorrectTarget(string tmName, string packageSourceLanguageCode,
			string packageTargetLanguageCodes)
		{
			var uri = new Uri($"{Path.Combine(_testingFilesPath, tmName)}");
			var targetLanguages = GetStudioLanguages(packageTargetLanguageCodes);

			var (isSupported, language) = _studioService.TmSupportsAnyLanguageDirection(uri, new CultureInfo(packageSourceLanguageCode), targetLanguages);
			Assert.False(isSupported);
			Assert.Null(language);
		}

		[Theory]
		[InlineData("TestTransitTM.sdltm", "en-US", "en-GB,id-ID")]
		public void TmSupportsAnyLanguageDirection_ReturnsTrue_CorrectTargetLanguage(string tmName, string packageSourceLanguageCode,
			string packageTargetLanguageCodes)
		{
			var uri = new Uri($"{Path.Combine(_testingFilesPath, tmName)}");
			var targetLanguage = new Language("id-ID");
			var targetLanguages = GetStudioLanguages(packageTargetLanguageCodes);

			var (isSupported, language) = _studioService.TmSupportsAnyLanguageDirection(uri, new CultureInfo(packageSourceLanguageCode), targetLanguages);
			Assert.True(isSupported);
			Assert.NotNull(language);
			Assert.Equal(targetLanguage, language);
		}

		[Theory]
		[InlineData("projects.xml", "Automotive")]
		public async Task ReadCustomers_ContainsCustomer(string projectsFileName, string customerName)
		{
			var projectsXmlFilePath = Path.Combine(_testingFilesPath, projectsFileName);
			var customers = await _studioService.GetCustomers(projectsXmlFilePath);
			Assert.Contains(customers, customer => customer.Name == customerName);
		}

		[Fact]
		public async Task ReadCustomers_NoPath_ReturnsOnlyTheBlankPlaceholder()
		{
			var customers = await _studioService.GetCustomers(null);

			//the first entry is always a blank customer the UI uses to clear the selection
			var placeholder = Assert.Single(customers);
			Assert.Null(placeholder.Name);
		}

		[Theory]
		[InlineData("TestTransitTM.sdltm")]
		public void TmSupportsAnyLanguageDirection_NoLanguages_ReturnsNull(string tmName)
		{
			var uri = new Uri($"{Path.Combine(_testingFilesPath, tmName)}");

			var (isSupported, language) = _studioService.TmSupportsAnyLanguageDirection(uri, null, null);
			Assert.False(isSupported);
			Assert.Null(language);
		}

		private Language[] GetStudioLanguages(string targetLanguageCodes)
		{
			var targetCodes = targetLanguageCodes.Split(',');
			return  targetCodes.Select(code => new Language(code)).ToArray();
		}

		private class ProjectsControllerServiceStub : IProjectsControllerService
		{
			public List<ProjectTemplateInfo> Templates { get; set; } = new List<ProjectTemplateInfo>();

			public IEnumerable<ProjectTemplateInfo> GetProjectTemplates() => Templates;

			public IEnumerable<FileBasedProject> GetSelectedProjects() => Enumerable.Empty<FileBasedProject>();

			public void OpenProjectInFilesView(IProject studioProject)
			{
			}
		}
	}
}
