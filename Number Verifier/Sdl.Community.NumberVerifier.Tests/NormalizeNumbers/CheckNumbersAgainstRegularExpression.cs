//using System.Collections.Generic;
//using Moq;
//using Sdl.Community.NumberVerifier.Parsers.Number;
//using Sdl.Community.NumberVerifier.Tests.Utilities;
//using Sdl.FileTypeSupport.Framework.BilingualApi;
//using Xunit;

//namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
//{
//	public class CheckNumbersAgainstRegularExpression
//	{
//		private readonly Mock<IDocumentProperties> _documentProperties;
//		private readonly NumberNormalizer _numberNormalizer;

//		public CheckNumbersAgainstRegularExpression()
//		{
//			_documentProperties = new Mock<IDocumentProperties>();
//			_numberNormalizer = new NumberNormalizer();
//		}

//		/// <summary>
//		/// In case of -(space)number the dash should be ignored because is not a negative number
//		/// is a item in a list
//		/// </summary>
//		[Theory]
//		[InlineData("- 34", ".,", ",")]
//		public void SkipTheDash(string text, string thousandSep, string decimalSep)
//		{
//			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
//			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

//			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
//			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

//			//run initialize method in order to set chosen separators
//			numberVerifierMain.Initialize(_documentProperties.Object);

//			_numberNormalizer.GetNormalizedNumbers(text, null, numberVerifierSettings.Object, out var normalizedNumbers, out _);

//			Assert.Equal("34", normalizedNumbers.InitialPartsList[0]);
//		}

//		[Theory]
//		[InlineData("−74,5", ".,", ",")]
//		public void NormalizeNegativeNumbers(string text, string thousandSep, string decimalSep)
//		{
//			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
//			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
//			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
//			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

//			//run initialize method in order to set chosen separators
//			numberVerifierMain.Initialize(_documentProperties.Object);

//			_numberNormalizer.GetNormalizedNumbers(text, null, numberVerifierSettings.Object, out var normalizedNumbers, out _);

//			Assert.Equal(new List<string> { "−74,5" }, normalizedNumbers.InitialPartsList);
//			Assert.Equal(new List<string> { "s74d5" }, normalizedNumbers.NormalizedPartsList);
//		}

//		[Theory]
//		[InlineData("This ab46 is not an alphanumeric, the plugin will recognize only the number", ".,", ",")]
//		public void FindNumbersWithinTheWords(string text, string thousandSep, string decimalSep)
//		{
//			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
//			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
//			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

//			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

//			//run initialize method in order to set chosen separators
//			numberVerifierMain.Initialize(_documentProperties.Object);

//			_numberNormalizer.GetNormalizedNumbers(text, null, numberVerifierSettings.Object, out var normalizedNumbers, out _);

//			Assert.Equal("46", normalizedNumbers.InitialPartsList[0]);
//		}

//		[Theory]
//		[InlineData("Drill holes al least every 600 milimeters", "Perces de trous au moins tous les 600 centimeters", ".,", ",")]
//		public void CheckNumbersAreValid(string sourceText, string targetText, string thousandSep, string decimalSep)
//		{
//			// Arrange
//			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
//			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(false);
//			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

//			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

//			// run initialize method in order to set chosen separators
//			numberVerifierMain.Initialize(_documentProperties.Object);

//			// Act
//			_numberNormalizer.GetNormalizedNumbers(sourceText, targetText, numberVerifierSettings.Object, out var sourceResult, out var targetResult);

//			// Assert
//			Assert.Equal(sourceResult.InitialPartsList[0], targetResult.InitialPartsList[0]);
//		}

//		[Theory]
//		[InlineData("Drill holes al least every 600 milimeters", "Perces de trous au moins tous les 60 centimeters", ".,", ",")]
//		public void CheckNumbersAreNotValid(string sourceText, string targetText, string thousandSep, string decimalSep)
//		{
//			// Arrange
//			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
//			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(false);
//			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

//			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

//			//run initialize method in order to set chosen separators
//			numberVerifierMain.Initialize(_documentProperties.Object);

//			// Act
//			_numberNormalizer.GetNormalizedNumbers(sourceText, targetText, numberVerifierSettings.Object, out var sourceResult, out var targetResult);

//			// Assert
//			Assert.NotEqual(sourceResult.InitialPartsList, targetResult.InitialPartsList);
//		}
//	}
//}