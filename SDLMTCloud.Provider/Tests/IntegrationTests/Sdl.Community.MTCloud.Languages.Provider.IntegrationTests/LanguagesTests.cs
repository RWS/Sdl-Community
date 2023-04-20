using System;
using System.IO;
using LanguageMappingProvider;
using Xunit;

namespace Sdl.Community.MTCloud.Languages.Provider.IntegrationTests
{
	public class LanguagesTests
	{
		[Fact]
		public void ReadLanguages_NotEmpty_ReturnTrue()
		{
			//// Arrange
			var languages = new LanguageProvider();
			var filePath = Path.GetTempFileName() + ".xlsx";

			//// Act		
			var results = languages.GetMappedLanguages(filePath);
			var count = results.Count;

			//// Assert									
			Assert.True(count > 0, "Expected: 1 or more languages; found: 0");

			// cleanup
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		[Fact]
		public void SaveLanguages_WithUpdatedValue_ReturnTrue()
		{
			//// Arrange			
			var languages = new LanguageProvider();
			var filePath = Path.GetTempFileName() + ".xlsx";
			var expectedValue = "testValue101";

			//// Act						
			var result1 = languages.GetMappedLanguages(filePath);

			// update the first cell of the first row
			result1[0].Name = expectedValue;

			// save the updated changes to the first cell of the first row
			languages.SaveMappedLanguages(result1, filePath);

			// read the file with the updated changes
			var result2 = languages.GetMappedLanguages(filePath);

			//// Assert
			var testValue = result2[0].Name;
			Assert.True(string.Compare(testValue, expectedValue, StringComparison.InvariantCultureIgnoreCase) == 0,
				"Expected: " + expectedValue + "; found: " + testValue);

			// cleanup
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
	}
}
