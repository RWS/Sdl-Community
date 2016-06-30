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
    public class NormalizeNumbersAllowLocalization
    {
        [Theory]
        [InlineData("1,55", " ", ",")]
        public void NormalizeDecimalNumbersComma(string text, string thousandSep, string decimalSep)
        {
            // add to settings allow localization and thousnds separators
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            //numberVerifierMain.AddCustomSeparators(thousandSep);
            //numberVerifierMain.AddCustomSeparators(decimalSep);


            var normalizedNumber = numberVerifierMain.NormalizedNumber(text, thousandSep, decimalSep, false);

            Assert.Equal(normalizedNumber, "1d55");
        }




    }
}
