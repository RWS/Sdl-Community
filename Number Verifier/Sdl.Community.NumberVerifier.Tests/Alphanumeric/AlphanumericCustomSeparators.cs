using System.Linq;
using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
	public class AlphanumericCustomSeparators
    {
        private readonly Mock<IDocumentProperties> _documentProperties;

        public AlphanumericCustomSeparators()
        {
            _documentProperties = new Mock<IDocumentProperties>();
        }

        [Theory]
        [InlineData("The apartment number is 125H-B10. ", "The apartment number is 125H-B10.", "-")]
        public void AlphanumericVerification_CustomSeparator_NoErrors(string source, string target, string customSeparators)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(c => c.AlphanumericsCustomSeparator).Returns(customSeparators);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        [Theory]
        [InlineData("The apartment number is 125H-B10. ", "The apartment number is 125H-C20.", "-")]
        public void AlphanumericVerification_CustomSeparator_WithErrors(string source, string target, string customSeparators)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(c => c.AlphanumericsCustomSeparator).Returns(customSeparators);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage[0].ErrorMessage);
        }

        [Theory]
        [InlineData("BS2-3", "-")]
        public void FindAlphanumericWithCustomSeparators(string text, string customSeparators)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CustomSeparators();
            numberVerifierSettings.Setup(c => c.CustomsSeparatorsAlphanumerics).Returns(true);
            numberVerifierSettings.Setup(c => c.AlphanumericsCustomSeparator).Returns(customSeparators);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var textAlphanumericsList = numberVerifierMain.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
        }

        [Theory]
        [InlineData("BS44", "BS44", "*")]
        public void FindAlphanumericWithCustomSeparators_NoError(string sourceText, string targetText, string customSeparators)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CustomSeparators();
            numberVerifierSettings.Setup(c => c.CustomsSeparatorsAlphanumerics).Returns(true);
            numberVerifierSettings.Setup(c => c.AlphanumericsCustomSeparator).Returns(customSeparators);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessages = numberVerifierMain.CheckAlphanumerics(sourceText, targetText);

            Assert.True(!errorMessages.Any());
        }

        [Theory]
        [InlineData("12S\\A*-3", "\\*-")]
        public void FindAlphanumericWithMultipleCustomSeparators(string text, string customSeparators)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CustomSeparators();
            numberVerifierSettings.Setup(c => c.CustomsSeparatorsAlphanumerics).Returns(true);
            numberVerifierSettings.Setup(c => c.AlphanumericsCustomSeparator).Returns(customSeparators);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var textAlphanumericsList = numberVerifierMain.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
        }

        [Theory]
        [InlineData("The class where I've studies was AB13 at the second floor.", "Die Klasse, in der ich studiert habe, war AB13 im zweiten Stock.")]
        public void FindAlphanumericWithoutCustomSeparators_NoError(string sourceText, string targetText)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CustomSeparators();
            numberVerifierSettings.Setup(c => c.CustomsSeparatorsAlphanumerics).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators

            var sourceAlphanumerics = numberVerifierMain.GetAlphanumericList(sourceText);
            var targetAlphanumerics = numberVerifierMain.GetAlphanumericList(targetText);

            Assert.Equal(sourceAlphanumerics.Item2[0], targetAlphanumerics.Item2[0]);
        }

        [Theory]
        [InlineData("The bus is T13+", "Die bus ist T13")]
        public void FindAlphanumericWithUnassignedCustomSeparators_NoError(string sourceText, string targetText)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CustomSeparators();
            numberVerifierSettings.Setup(c => c.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators

            var sourceAlphanumerics = numberVerifierMain.GetAlphanumericList(sourceText);
            var targetAlphanumerics = numberVerifierMain.GetAlphanumericList(targetText);

            Assert.Equal(sourceAlphanumerics.Item2[0], targetAlphanumerics.Item2[0]);
        }
    }
}