using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
    public  class MultipleErrors
    {

        [Theory]
        [InlineData("1,554,5 alphanumeric AB23", " wrong separator 1 554.5 wrong alphanumeric BC12")]
        public void ThousandsNumbersAndAlphanumerics(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);
           

            Assert.True(errorMessage.Count==3);
        }
    }
}
