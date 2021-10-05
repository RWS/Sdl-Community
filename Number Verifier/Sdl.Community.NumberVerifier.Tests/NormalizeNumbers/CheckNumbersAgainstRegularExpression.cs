using System.Collections.Generic;
using Moq;
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class CheckNumbersAgainstRegularExpression
	{
		private readonly Mock<IDocumentProperties> _documentProperties;
		private readonly NumberNormalizer _numberNormalizer;

		public CheckNumbersAgainstRegularExpression()
		{
			_documentProperties = new Mock<IDocumentProperties>();
			_numberNormalizer = new NumberNormalizer();
		}

		/// <summary>
		/// In case of -(space)number the dash should be ignored because is not a negative number  
		/// is a item in a list
		/// </summary>
		[Theory]
        [InlineData("- 34", ".,", ",")]
        public void SkipTheDash(string text, string thousandSep, string decimalSep)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);

			var normalizedNumbers = _numberNormalizer.GetNormalizedNumbers(text, numberVerifierSettings.Object, false);

            Assert.Equal("34", normalizedNumbers.InitialPartsList[0]);
        }

        [Theory]
        [InlineData("−74,5", ".,", ",")]
        public void NormalizeNegativeNumbers(string text, string thousandSep, string decimalSep)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);
			
            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);

	        var normalizeNumber = _numberNormalizer.GetNormalizedNumbers(text, numberVerifierSettings.Object, false);

	        Assert.Equal(new List<string> { "−74,5"}, normalizeNumber.InitialPartsList);
	        Assert.Equal(new List<string> { "m74m5" }, normalizeNumber.NormalizedPartsList);
        }

        [Theory]
        [InlineData("This ab46 is not an alphanumeric, the plugin will recognize only the number", ".,", ",")]
        public void FindNumbersWithinTheWords(string text, string thousandSep, string decimalSep)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);

			var normalizeNumber = _numberNormalizer.GetNormalizedNumbers(text, numberVerifierSettings.Object, false);

			Assert.Equal("46", normalizeNumber.InitialPartsList[0]);
        }

		[Theory]
		[InlineData("Drill holes al least every 600 milimeters", "Perces de trous au moins tous les 600 centimeters", ".,", ",")]
		public void CheckNumbersAreValid(string sourceText, string targetText, string thousandSep, string decimalSep)
		{
			// Arrange
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(false);
			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			// run initialize method in order to set chosen separators
			numberVerifierMain.Initialize(_documentProperties.Object);

			var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep, true);
			var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep, true);

			// Act
			var sourceResult = _numberNormalizer.GetNormalizedNumbers(sourceText, numberVerifierSettings.Object, false);
			var targetResult = _numberNormalizer.GetNormalizedNumbers(targetText, numberVerifierSettings.Object, false);

			// Assert
			Assert.Equal(sourceResult.InitialPartsList[0], targetResult.InitialPartsList[0]);
		}


		[Theory]
		[InlineData("Drill holes al least every 600 milimeters", "Perces de trous au moins tous les 60 centimeters", ".,", ",")]
		public void CheckNumbersAreNotValid(string sourceText, string targetText, string thousandSep, string decimalSep)
		{
			// Arrange
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(false);
			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			numberVerifierMain.Initialize(_documentProperties.Object);

			var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep, true);
			var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep, true);

			// Act
			var sourceResult = _numberNormalizer.GetNormalizedNumbers(sourceText, numberVerifierSettings.Object, false);
			var targetResult = _numberNormalizer.GetNormalizedNumbers(targetText, numberVerifierSettings.Object, false);

			// Assert
			Assert.NotEqual(sourceResult.InitialPartsList, targetResult.InitialPartsList);
		}
	}
}