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
		[InlineData("TestTransitTM.sdltm", "de-DE", "en-GB,fr-FR")]
		public void TmSupportsAnyLanguageDirection_ReturnsFalse(string tmName, string packageSourceLanguageCode,
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

		private Language[] GetStudioLanguages(string targetLanguageCodes)
		{
			var targetCodes = targetLanguageCodes.Split(',');
			return  targetCodes.Select(code => new Language(code)).ToArray();
		}

	}
}
