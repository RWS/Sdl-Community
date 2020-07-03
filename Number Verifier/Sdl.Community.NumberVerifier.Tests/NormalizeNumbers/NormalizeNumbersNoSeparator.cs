using Moq;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class NormalizeNumbersNoSeparator
    {
	    [Theory]
        [InlineData("1,55", " ", ",", true)]
        public string NormalizeNoSeparatorNumbers(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
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
				NoSeparator = noSeparator,
				CustomSeparators = string.Empty
			});

            return normalizedNumber;
        }

        [Theory]
        [InlineData("1.55", "1,55 ")]
        public void NotNormalizeDecimalNumbers(string source, string target)
        {
			//target settings
			var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
			numberVerifierSettings.Setup(t => t.TargetNoSeparator).Returns(true);
			numberVerifierSettings.Setup(t => t.TargetDecimalComma).Returns(true);

			// source settings
			numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
			numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

        [Theory]
        [InlineData("1000", "1.000")]
        public void NormalizeThousandsNumberNoSeparatorSelected(string source, string target)
        {
			//target settings
			var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
			numberVerifierSettings.Setup(t => t.TargetThousandsPeriod).Returns(true);

			// source settings
			numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}
	}
}