using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
    public class SourceAndTargetNormalization
    {
        [Theory]
        [InlineData("1,55","1.55")]
        public void SourceAndTargetCheck(string source,string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var paragraphUnitMock = new Mock<IParagraphUnit>();
            var paragraphMock = new Mock<IParagraph>();
           
           // paragraphMock.Setup(m=>m.)

           // paragraphUnitMock.Setup(s=>s.Source).Returns(paragraphMock.Object.)
            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);
        }

      
    }
}
