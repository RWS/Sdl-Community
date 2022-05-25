using Moq;
using Sdl.Community.NumberVerifier.Tests.SourceSettings;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class CheckFullWidthJapaneseNumbers
    {
        private readonly Mock<IDocumentProperties> _documentProperties;

        public CheckFullWidthJapaneseNumbers()
        {
            _documentProperties = new Mock<IDocumentProperties>();
        }

        /// <summary>
        /// Validate the full-width Japanese numbers: ２０１６ -> 2016
        /// </summary>
        [Theory]
        [InlineData("２０１６", "2016")]
        public void ValidateFullWidthJapaneseNumbers(string source, string target)
        {
            //target settings
            var numberVerifierSettings = SourceSettingsAndPreventLocalization.SpaceCommaPeriod();

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }
    }
}