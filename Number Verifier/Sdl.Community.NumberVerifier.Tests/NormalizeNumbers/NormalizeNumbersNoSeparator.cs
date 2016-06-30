using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
    public class NormalizeNumbersNoSeparator
    {
        
        [Theory]
        [InlineData("1,55", " ", ",", false)]
        public void CheckIfNoSeparatorMethodIsCalled(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);


            var normalizedNumber = numberVerifierMain.NormalizedNumber(text, thousandSep, decimalSep, noSeparator);

            var methodsMock = new Mock<INumberVerifierMethods>(MockBehavior.Strict);
            methodsMock.Verify(s => s.NormalizeNumberNoSeparator(thousandSep, decimalSep, normalizedNumber), Times.Never);

        }

        [Theory]
        [InlineData("1,55", " ", ",", true)]
        public string NormalizeNoSeparatorNumers(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);


            var normalizedNumber = numberVerifierMain.NormalizedNumber(text, thousandSep, decimalSep, noSeparator);

            return normalizedNumber;

        }

        [Theory]
        [InlineData("1,55", " ", ",", true)]
        public void NotNormalizeDecimalNumbers(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var normalizedNumber = NormalizeNoSeparatorNumers(text, thousandSep, decimalSep, noSeparator);

            Assert.Equal(normalizedNumber, "1d55");

        }

        [Theory]
        [InlineData("1,234.56", ",", ".", true)]
        public void NormalizeThousandsNumberNoSeparatorSelected(string text, string thousandSep, string decimalSep,
            bool noSeparator)
        {
            var normalizedNumber = NormalizeNoSeparatorNumers(text, thousandSep, decimalSep, noSeparator);

            Assert.Equal(normalizedNumber,"1t234d56");
        }

    }
}
