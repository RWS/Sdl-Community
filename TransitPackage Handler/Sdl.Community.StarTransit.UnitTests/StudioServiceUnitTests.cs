using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Models;
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
		public async Task ReadTemplateData_EmptyPath_ReturnsNull(string templatePath)
		{
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(templatePath,null,null);
			Assert.Null(templateInfo);
		}

		[Theory]
		[InlineData("TransitMultilingualTemplate.sdltpl")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsProjectLocation(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.NotNull(templateInfo);
			Assert.NotEmpty(templateInfo.Location);
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
		[InlineData("TransitMultilingualTemplate.sdltpl")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsDueDate(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.NotNull(templateInfo.DueDate);
		}
		[Theory]
		[InlineData("MultilingualNoOptions.sdltpl")]
		public async Task ReadTemplateData_TransitTemplate_ReturnsNullDueDate(string templateName)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, null, null);

			Assert.Null(templateInfo.Customer);
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
		[InlineData("TransitMultilingualTemplate.sdltpl", "de-DE", "en-GB,fr-FR")]
		public async Task ReadTemplateData_TransitTemplate_GetCorrectTmOption(string templateName, string sourceLanguageCode,
			string targetLanguageCodes)
		{
			var languagePair = new LanguagePair
			{
				SourceLanguage = new CultureInfo("de-DE"),
				TargetLanguage = new CultureInfo("en-GB"),
				CreateNewTm = true,
				TemplatePenalty = 5
			};
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate, new CultureInfo(sourceLanguageCode), targetLanguages);

			Assert.Single(templateInfo.LanguagePairs);
			Assert.Equal(languagePair.SourceLanguage,templateInfo.LanguagePairs[0].SourceLanguage);
			Assert.Equal(languagePair.TargetLanguage, templateInfo.LanguagePairs[0].TargetLanguage);
			Assert.True(templateInfo.LanguagePairs[0].CreateNewTm);
			Assert.Equal(5, templateInfo.LanguagePairs[0].TemplatePenalty);
		}

		[Theory]
		[InlineData("TransitMultilingualPenaltiesOptions.sdltpl", "de-DE", "en-GB,fr-FR,it-IT", 2)]
		public async Task ReadTemplateData_MultilingualTransitTemplate_GetCorrectTmOptionsNumber(string templateName,
			string sourceLanguageCode,
			string targetLanguageCodes, int tmsOptions)
		{
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate,
				new CultureInfo(sourceLanguageCode), targetLanguages);

			Assert.NotNull(templateInfo.LanguagePairs);
			Assert.Equal(tmsOptions, templateInfo.LanguagePairs.Count);
		}

		[Theory]
		[InlineData("TransitMultilingualPenaltiesOptions.sdltpl", "de-DE", "en-GB,fr-FR,it-IT")]
		public async Task ReadTemplateData_MultilingualTransitTemplate_GetCorrectLanguagePairOptions_DeFr(string templateName,
			string sourceLanguageCode,
			string targetLanguageCodes)
		{
			var deFrLanguagePair = new LanguagePair
			{
				SourceLanguage = new CultureInfo("de-DE"),
				TargetLanguage = new CultureInfo("fr-FR"),
				CreateNewTm = true,
				TemplatePenalty = 10
			};
			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var templateInfo = await  _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate,
				new CultureInfo(sourceLanguageCode), targetLanguages);

			var fRCorrespondingLpOption = GetCorresponndingLanguagePair(templateInfo.LanguagePairs, deFrLanguagePair);
			Assert.Equal(deFrLanguagePair.SourceLanguage, fRCorrespondingLpOption.SourceLanguage);
			Assert.Equal(deFrLanguagePair.TargetLanguage, fRCorrespondingLpOption.TargetLanguage);
			Assert.Equal(deFrLanguagePair.CreateNewTm,fRCorrespondingLpOption.CreateNewTm);
			Assert.Equal(deFrLanguagePair.TemplatePenalty, fRCorrespondingLpOption.TemplatePenalty);
		}

		[Theory]
		[InlineData("TransitMultilingualPenaltiesOptions.sdltpl", "de-DE", "en-GB,fr-FR,it-IT")]
		public async Task ReadTemplateData_MultilingualTransitTemplate_GetCorrectLanguagePairOptions_DeIt(string templateName,
			string sourceLanguageCode,
			string targetLanguageCodes)
		{
			var deItLanguagePair = new LanguagePair
			{
				SourceLanguage = new CultureInfo("de-DE"),
				TargetLanguage = new CultureInfo("it-IT"),
				ChoseExistingTm = true,
				TemplatePenalty = 0,
				TmName = "Transit De-It.sdltm",
				TmPath = @"C:/Users/aghisa/Documents/Studio 2021/Translation Memories/Transit De-It.sdltm"
			};

			var multilingualTemplate = Path.Combine(_testingFilesPath, templateName);
			var targetLanguages = GetStudioLanguages(targetLanguageCodes);

			var templateInfo = await _studioService.GetModelBasedOnStudioTemplate(multilingualTemplate,
				new CultureInfo(sourceLanguageCode), targetLanguages);

			var correspondingLpOption = GetCorresponndingLanguagePair(templateInfo.LanguagePairs, deItLanguagePair);
			Assert.Equal(deItLanguagePair.SourceLanguage, correspondingLpOption.SourceLanguage);
			Assert.Equal(deItLanguagePair.TargetLanguage, correspondingLpOption.TargetLanguage);
			Assert.Equal(deItLanguagePair.CreateNewTm, correspondingLpOption.CreateNewTm);
			Assert.Equal(deItLanguagePair.TemplatePenalty, correspondingLpOption.TemplatePenalty);
		}

		private LanguagePair GetCorresponndingLanguagePair(List<LanguagePair> templateInfoLanguagePairs, LanguagePair languagePair)
		{
			return templateInfoLanguagePairs.FirstOrDefault(l =>
				l.SourceLanguage.Equals(languagePair.SourceLanguage) &&
				l.TargetLanguage.Equals(languagePair.TargetLanguage));
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

		[Theory]
		[InlineData("projects.xml")]
		public async Task ReadCustomers_ReturnsNotNull(string projectsFileName)
		{
			var projectsXmlFilePath = Path.Combine(_testingFilesPath, projectsFileName);
			var customers = await _studioService.GetCustomers(projectsXmlFilePath);
			Assert.NotNull(customers);
		}

		[Theory]
		[InlineData("projects.xml", "Automotive")]
		public async Task ReadCustomers_ContainsCustomer(string projectsFileName, string customerName)
		{
			var projectsXmlFilePath = Path.Combine(_testingFilesPath, projectsFileName);
			var customers = await _studioService.GetCustomers(projectsXmlFilePath);
			Assert.Contains(customers,
				customer => customer.Name != null && customer.Name.Contains(customerName));
		}

		[Fact]
		public async Task ReadCustomers_EmptyPath_ReturnsNotNull()
		{
			var customers = await _studioService.GetCustomers(null);
			Assert.NotNull(customers);
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

	}
}
