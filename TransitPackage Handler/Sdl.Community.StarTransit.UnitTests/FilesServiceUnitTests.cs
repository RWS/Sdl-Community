using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
		private readonly IFileService _fileService;

		public FilesServiceUnitTests()
		{
			_fileService = new FileService();
			_testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
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

		[Theory]
		[InlineData("MachineTranslatedSegments.DEU")]
		public void IsTransitFile_TransitFile_ReturnsTrue(string fileName)
		{
			var isTransitFile = _fileService.IsTransitFile(Path.Combine(_testingFilesPath, fileName));
			Assert.True(isTransitFile);
		}

		[Fact]
		public void IsTransitFile_DeclarationAndRootElementOnOneLine_ReturnsTrue()
		{
			var transitFile = Path.GetTempFileName();
			try
			{
				File.WriteAllText(transitFile, "<?xml version=\"1.0\" encoding=\"UTF-16\"?><Transit version=\"4.0\">\r\n<Header>\r\n", Encoding.Unicode);

				Assert.True(_fileService.IsTransitFile(transitFile));
			}
			finally
			{
				File.Delete(transitFile);
			}
		}

		[Theory]
		[InlineData("projects.xml")]
		[InlineData("missing file")]
		public void IsTransitFile_OtherFile_ReturnsFalse(string fileName)
		{
			var isTransitFile = _fileService.IsTransitFile(Path.Combine(_testingFilesPath, fileName));
			Assert.False(isTransitFile);
		}

		[Fact]
		public void GetStudioTargetLanguages_ReturnsTheTargetOfEachLanguagePair()
		{
			var languagePairs = new List<LanguagePair>
			{
				new LanguagePair { SourceLanguage = new CultureInfo("de-DE"), TargetLanguage = new CultureInfo("fr-FR") },
				new LanguagePair { SourceLanguage = new CultureInfo("de-DE"), TargetLanguage = new CultureInfo("en-GB") }
			};

			var targetLanguages = _fileService.GetStudioTargetLanguages(languagePairs);

			Assert.Equal(new[] { "fr-FR", "en-GB" }, targetLanguages.Select(language => language.CultureInfo.Name));
		}
	}
}
