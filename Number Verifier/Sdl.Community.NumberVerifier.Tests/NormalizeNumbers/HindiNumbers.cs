using System.Linq;
using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class HindiNumbers
    {
        private readonly Mock<IDocumentProperties> _documentProperties;

        public HindiNumbers()
        {
            _documentProperties = new Mock<IDocumentProperties>();
        }

        /// <summary>
        /// Validate wrong numbers from Arabic to Hindi
        /// </summary>
        [Theory]
        [InlineData("12.34,89", "١٢٣.٤,٨٩", "Arabic (Arabic)")]
        public void CheckFromArabicToHindiInValid(string source, string target, string sourceLanguage)
        {
            // settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
            numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var results = numberVerifierMain.GetTargetFromHindiNumbers(source, target, sourceLanguage);
            if (results.Any())
            {
                Assert.NotEqual(results[0].SourceText, results[0].TargetText);
            }
        }

        /// <summary>
        /// Validate correct numbers from Arabic to Hindi
        /// </summary>
        [Theory]
        [InlineData("1.234,89", "١.٢٣٤,٨٩", "Arabic (Arabic)")]
        public void CheckFromArabicToHindiValid(string source, string target, string sourceLanguage)
        {
            // settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
            numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var results = numberVerifierMain.GetTargetFromHindiNumbers(source, target, sourceLanguage);

            Assert.All(results, result => Assert.Equal(result.SourceText, result.TargetArabicText));
        }

        /// <summary>
        /// Validate wrong numbers from Hindi to Arabic
        /// </summary>
        [Theory]
        [InlineData("١٢٣.٤,٨٩", "12.34,89", "Hindi (India)")]
        public void CheckFromHindiToArabicInvalid(string source, string target, string sourceLanguage)
        {
            // settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
            numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var results = numberVerifierMain.GetTargetFromHindiNumbers(source, target, sourceLanguage);
            if (results.Any())
            {
                Assert.NotEqual(results[0].SourceText, results[0].TargetText);
            }
        }

        /// <summary>
        /// Validate correct numbers from Hindi to Arabic
        /// </summary>
        [Theory]
        [InlineData("١.٢٣٤,٨٩", "1.234,89", "Hindi (India)")]
        public void CheckFromHindiToArabicValid(string source, string target, string sourceLanguage)
        {
            // settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
            numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            var results = numberVerifierMain.GetTargetFromHindiNumbers(source, target, sourceLanguage);

            Assert.All(results, result => Assert.Equal(result.SourceArabicText, result.TargetArabicText));
        }
    }
}