using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Tests.SourceSettings;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
    /// <summary>
    /// Source separators
    /// </summary>
    public class SourceAndTargetNormalizationPrevent
    {
        
        /// <summary>
        /// Decimal separators : comma, period
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1,55", "1.55")]
        public void DecimalSeparatorsComma(string source, string target)
        {
            //target settings
            var numberVerifierSettings = SourceSettingsAndPreventLocalization.SpaceCommaPeriod();
 
            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

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
