using System.Collections.Generic;
using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class CheckNumbersAgainstRegularExpression
	{
		/// <summary>
        /// In case of -(space)number the dash should be ignored because is not a negative number  
        /// is a item in a list
        /// </summary>
        /// <param name="text"></param>
        /// <param name="thousandSep"></param>
        /// <param name="decimalSep"></param>
        [Theory]
        [InlineData("- 34", ".,", ",")]
        public void SkipTheDash(string text, string thousandSep, string decimalSep)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);
            var normalizeNumber = NumberVerifierSetup.GetNormalizedNumber(text, thousandSeparators, decimalSeparators, false, false);

            numberVerifierMain.NormalizeDecimalsNumbers(normalizeNumber);
            numberVerifierMain.NormalizeThousandsNumbers(normalizeNumber);

            Assert.Equal("34", normalizeNumber.InitialNumberList[0]);
            Assert.Equal("34", normalizeNumber.NormalizedNumberList[0]);
            
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
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);
			
            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);

            var normalizeNumber = NumberVerifierSetup.GetNormalizedNumber(text, thousandSeparators, decimalSeparators, false, false);
            numberVerifierMain.NormalizeDecimalsNumbers(normalizeNumber);
            numberVerifierMain.NormalizeThousandsNumbers(normalizeNumber);

            Assert.Equal("−74,5", $"{normalizeNumber.InitialNumberList[0]}{normalizeNumber.InitialNumberList[1]}");
            Assert.Equal("m74m5", $"{normalizeNumber.NormalizedNumberList[0]}{normalizeNumber.NormalizedNumberList[1]}");
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
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);
           
            var normalizeNumber = NumberVerifierSetup.GetNormalizedNumber(text, thousandSeparators, decimalSeparators, false, false);
            numberVerifierMain.NormalizeDecimalsNumbers(normalizeNumber);
            numberVerifierMain.NormalizeThousandsNumbers(normalizeNumber);

            Assert.Equal("46", normalizeNumber.InitialNumberList[0]);
            Assert.Equal("46", normalizeNumber.NormalizedNumberList[0]);
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
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep, true);
			var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep, true);

			// Act
			var sourceResult = numberVerifierMain.GetNumbersTuple(sourceText, decimalSeparators, thousandSeparators, false, false);
			var targetResult = numberVerifierMain.GetNumbersTuple(targetText, decimalSeparators, thousandSeparators, false, false);

			// Assert
			Assert.Equal(sourceResult.Item1[0], targetResult.Item1[0]);
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
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep, true);
			var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep, true);

			// Act
			var sourceResult = numberVerifierMain.GetNumbersTuple(sourceText, decimalSeparators, thousandSeparators, false, false);
			var targetResult = numberVerifierMain.GetNumbersTuple(targetText, decimalSeparators, thousandSeparators, false, false);

			// Assert
			Assert.NotEqual(sourceResult.Item1, targetResult.Item1);
		}
	}
}
