using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class NormalizeNumbersAllowLocalization
    {
        private readonly Mock<IDocumentProperties> _documentProperties;

        public NormalizeNumbersAllowLocalization()
        {
            _documentProperties = new Mock<IDocumentProperties>();
        }

        [Theory]
        [InlineData("1,55", "1.55")]
        public void NormalizeDecimalNumbersComma(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetNoSeparator).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetDecimalPeriod).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }
    }
}