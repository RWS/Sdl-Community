using Moq;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class NormalizeNumbersAllowLocalization
	{
		[Theory]
		[InlineData("1,55", " ", ",")]
		public void NormalizeDecimalNumbersComma(string text, string thousandSep, string decimalSep)
		{
			// add to settings allow localization and thousands separators
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
			numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);


			var normalizedNumber = numberVerifierMain.NormalizeNumber(new SeparatorModel
			{
				MatchValue = text,
				ThousandSeparators = thousandSep,
				DecimalSeparators = decimalSep,
				NoSeparator = false,
				CustomSeparators = string.Empty
			});

			Assert.Equal("1d55", normalizedNumber);
		}
	}
}