using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class FilesServiceUnitTests
	{
		private readonly string _testingFilesPath;
		private readonly string _tmFilePath;
		private readonly string _transitFilePath;
		private readonly IFileService _fileService;

		public FilesServiceUnitTests()
		{
			_fileService = new FileService();
			_testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
			_tmFilePath = Path.Combine(_testingFilesPath, "_AEXTR_1.DEU");
			_transitFilePath = Path.Combine(_testingFilesPath, "FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN.DEU");
		}

		[Theory]
		[InlineData("randomEx")]
		[InlineData(null)]
		[InlineData("")]
		public void MapFileLanguage_ReturnsEmpty(string fileExtension)
		{
			var languageCode = _fileService.MapFileLanguage(fileExtension);
			Assert.Empty(languageCode);
		}

		[Theory]
		[InlineData("DEU","de-DE")]
		[InlineData("AZC", "az-Cyrl-AZ")]
		public void MapFileLanguage_ReturnsCorrectCode(string fileExtension,string languageCode)
		{
			var transitLanguageCode = _fileService.MapFileLanguage(fileExtension);
			Assert.Equal(languageCode,transitLanguageCode);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void MapTransitLanguage_ReturnsEmpty(string fileExtension)
		{
			var transitFileExtension = _fileService.MapStarTransitLanguage(fileExtension);
			Assert.Empty(transitFileExtension);
		}

		[Theory]
		[InlineData("DEU")]
		public void MapTransitLanguage_NotFound_ReturnsSameExtension(string fileExtension)
		{
			var transitFileExtension = _fileService.MapStarTransitLanguage(fileExtension);
			Assert.Equal(fileExtension,transitFileExtension);
		}

		[Theory]
		[InlineData("NGA", "EDO,EFI,NGA")]
		[InlineData("ITA", "ITS,ITA")]
		[InlineData("FRI", "FRV")]
		public void MapTransitLanguage_ReturnsCorrectCode(string fileExtension, string languageCode)
		{
			var transitLanguageCode = _fileService.MapStarTransitLanguage(fileExtension);
			Assert.Equal(languageCode, transitLanguageCode);
		}

		[Fact]
		public void IsTransitFile_RetunsTrue()
		{
			var isTransitFile = _fileService.IsTransitFile(_transitFilePath);
			Assert.True(isTransitFile);
		}

		[Fact]
		public void IsTransitFile_ReturnsTrue()
		{
			var isTransitFile = _fileService.IsTransitFile(_tmFilePath);
			Assert.True(isTransitFile);
		}

		[Fact]
		public void IsTransitFile_ReturnsFalse()
		{
			var isTransitFile = _fileService.IsTransitFile("random path");
			Assert.False(isTransitFile);
		}

		[Fact]
		public void GetStudioTargetLanguages_ReturnsEmpty()
		{
			var empty = _fileService.GetStudioTargetLanguages(null);
			Assert.Empty(empty);
		}

		[Fact]
		public void GetStudioTargetLanguages_ReturnsList()
		{
			var languagePairs = new List<LanguagePair>
			{
				new LanguagePair
				{
					SourceLanguage = new CultureInfo("en-en"), TargetLanguage = new CultureInfo("fr-fr")
				},new LanguagePair
				{
					SourceLanguage = new CultureInfo("en-en"), TargetLanguage = new CultureInfo("de-de")
				}
			};
			var targetLanguages = _fileService.GetStudioTargetLanguages(languagePairs);
			Assert.NotEmpty(targetLanguages);
		}
	}
}
