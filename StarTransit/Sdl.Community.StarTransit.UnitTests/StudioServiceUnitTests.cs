using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NSubstitute;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Service;
using Sdl.Core.Globalization;
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
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(templatePath,null,null);
			Assert.Null(templateInfo);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsProjectLocation(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.NotNull(templateInfo);
			Assert.NotEmpty(templateInfo.Location);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl", @"C:\Users\aghisa\Desktop\files")]
		public void ReadTemplateData_TransitTemplate_ReturnsCorrectProjectLocation(string templateName,string projectLocation)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Equal(projectLocation,templateInfo.Location);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl", "Automotive")]
		public void ReadTemplateData_TransitTemplate_ReturnsCorrectCustomer(string templateName, string selectedCustomerName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.NotNull(templateInfo.Customer);
			Assert.Equal(selectedCustomerName, templateInfo.Customer.Name);
		}

		[Theory]
		[InlineData("MultilingualNoOptions.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsNullCustomer(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Null(templateInfo.Customer);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsDueDate(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.NotNull(templateInfo.DueDate);
		}
		[Theory]
		[InlineData("MultilingualNoOptions.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsNullDueDate(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Null(templateInfo.Customer);
		}

		[Theory]
		[InlineData("Default.sdltpl")]
		public void ReadTemplateData_TransitTemplate_ReturnsNull(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Null(templateInfo);
		}

		[Theory]
		[InlineData("Test Multilingual Package Trados Plugin.de-en.sdltm", "de-DE", "en-GB,fr-FR")]
		public void IsTmCreatedFromPlugin_ReturnsTrue(string tmName, string sourceLanguageCode,
			string targetLanguageCodes)
		{
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var isTmCreatedFromPlugin = _studioService.IsTmCreatedFromPlugin(tmName,
				new CultureInfo(sourceLanguageCode), targetLanguages.ToArray());

			Assert.True(isTmCreatedFromPlugin.Item1);
			Assert.Equal(new Language("en-GB"),isTmCreatedFromPlugin.Item2);
		}

		[Theory]
		[InlineData("TestTransitTM.sdltm", "de-DE", "en-GB,fr-FR")]
		public void IsTmCreatedFromPlugin_ReturnsFalse(string tmName, string sourceLanguageCode,
			string targetLanguageCodes)
		{
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var isTmCreatedFromPlugin = _studioService.IsTmCreatedFromPlugin(tmName,
				new CultureInfo(sourceLanguageCode), targetLanguages.ToArray());

			Assert.False(isTmCreatedFromPlugin.Item1);
			Assert.Null(isTmCreatedFromPlugin.Item2);
		}

		[Theory]
		[InlineData("sdltm.file:///C:/Users/aghisa/Documents/Studio 2021/Translation Memories/TestTransitTM.sdltm")]
		public void GetTmLanguageFromPath(string tmUri)
		{
			var substitute = Substitute.For<IStudioService>();
			substitute.GetTranslationMemoryLanguage(tmUri).Returns(false);
			Assert.False(substitute.GetTranslationMemoryLanguage(tmUri));
			//var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			// templateInfo = _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate);
			 _studioService.GetTranslationMemoryLanguage(tmUri);
			//Assert.NotNull(templateInfo.DueDate);
		}

		private Language[] GetStudioLanguages(string targetLanguageCodes)
		{
			var targetCodes = targetLanguageCodes.Split(',');
			return  targetCodes.Select(code => new Language(code)).ToArray();
		}

	}
}
