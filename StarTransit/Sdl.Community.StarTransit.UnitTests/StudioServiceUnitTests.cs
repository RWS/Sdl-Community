using System;
using System.IO;
using NSubstitute;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Service;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class StudioServiceUnitTests
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly IStudioService _studioService;
		public StudioServiceUnitTests()
		{
			var projectsController = Substitute.For<ProjectsController>();
			_studioService = new StudioService(projectsController);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("randomPath")]
		public void ReadTemplateData_EmptyPath_ReturnsNull(string templatePath)
		{
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(templatePath);
			Assert.Null(templateInfo);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsProjectLocation(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);

			Assert.NotNull(templateInfo);
			Assert.NotEmpty(templateInfo.Location);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl", @"C:\Users\aghisa\Desktop\files")]
		public void ReadTemplateData_TransitTemplate_ReturnsCorrectProjectLocation(string templateName,string projectLocation)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);

			Assert.Equal(projectLocation,templateInfo.Location);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl", "Automotive")]
		public void ReadTemplateData_TransitTemplate_ReturnsCorrectCustomer(string templateName, string selectedCustomerName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);

			Assert.NotNull(templateInfo.Customer);
			Assert.Equal(selectedCustomerName, templateInfo.Customer.Name);
		}

		[Theory]
		[InlineData("MultilingualNoOptions.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsNullCustomer(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);

			Assert.Null(templateInfo.Customer);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsDueDate(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);

			Assert.NotNull(templateInfo.DueDate);
		}
		[Theory]
		[InlineData("MultilingualNoOptions.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsNullDueDate(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);

			Assert.Null(templateInfo.Customer);
		}

		[Theory]
		[InlineData("Default.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsNull(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);

			Assert.Null(templateInfo);
		}

	}
}
