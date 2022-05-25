using System.Collections.Generic;
using Moq;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
	public class NormalizeAlphanumerics
    {
        private readonly Mock<IDocumentProperties> _documentProperties;

        public NormalizeAlphanumerics()
        {
            _documentProperties = new Mock<IDocumentProperties>();
        }

        [Theory]
        [InlineData("The bus number is 30F. ", "The bus number is 30E.")]
        public void AlphanumericReportModifiedVerification_WithErrors(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.ReportModifiedAlphanumerics).Returns(true);
            numberVerifierSettings.Setup(d => d.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Alphanumerics which are items in a list
        /// source and target normal -
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("some text -AB23", "another text -AC23")]
        public void AlphanumericsInList(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage[0].ErrorMessage);
        }

        ///// <summary>
        ///// Alphanumerics which are items in a list
        ///// source normal - , target special −
        ///// see http://www.fileformat.info/info/unicode/char/2212/browsertest.htm
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="target"></param>
        [Theory]
        [InlineData("some text -AB23", "another text −AC23")]
        public void AlphanumericsInListDifferentLineReportsModified(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage[0].ErrorMessage);
        }

        [Theory]
        [InlineData("some word -AB23", "another text -AB23")]
        public void AlphanumericsInListSpecialSpace(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.CustomsSeparatorsAlphanumerics).Returns(true);
            numberVerifierSettings.Setup(d => d.AlphanumericsCustomSeparator).Returns("-");

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.True(errorMessage.Count == 0);
        }

        [Theory]
        [InlineData("The apartment number is 125H. ", "The apartment number is 125H.")]
        public void AlphanumericVerification_NoErrors(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.True(errorMessage.Count == 0);
        }

        [Theory]
        [InlineData("The apartment is 23F. ", "The apartment is 23E.")]
        public void AlphanumericVerification_WithErrors(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Check for alphanumerics with same text
        /// No error message.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("some text NO8-T1", "another text NO8-T1")]
        public void CheckAlphaNumerics_With_NoErrorMessage(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.CustomsSeparatorsAlphanumerics).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Check for alphanumerics with different target text
        /// Error message: Alphanumeric name modified.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("some text NO8-T1", "another text NO8-T2")]
        public void CheckAlphaNumerics_With_WrongNumber(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.CustomsSeparatorsAlphanumerics).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage[0]?.ErrorMessage);
        }

        /// <summary>
        /// This are not alphanumerics, number verifier should check only the
        /// numbers in this case
        /// No error message
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("mAB23", "mAB23")]
        public void CheckIfIsAnAlphanumeric(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessages = ReportModifiedAlphanumerics(source, target, numberVerifierMain);

            Assert.True(errorMessages.Count == 0);
        }

        /// <summary>
        /// Error number modified
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("mAB23", "mCD24")]
        public void ModifiedNumberError(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessages = ReportModifiedAlphanumerics(source, target, numberVerifierMain);

            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessages[0].ErrorMessage);
        }

        [Theory]
        [InlineData("The bus number is 30F. ", "The bus number is 30E.")]
        public void NoAlphanumericReportModifiedVerification_NoErrors(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.ReportModifiedAlphanumerics).Returns(false);
            numberVerifierSettings.Setup(d => d.CustomsSeparatorsAlphanumerics).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.True(errorMessage.Count == 0);
        }

        public List<ErrorReporting> ReportModifiedAlphanumerics(string source, string target, NumberVerifierMain numberVerifierMain)
        {
            //run initialize method in order to set chosen separators
            numberVerifierMain.Initialize(_documentProperties.Object);

            return numberVerifierMain.CheckSourceAndTarget(source, target);
        }

        [Theory]
        [InlineData("some text -AB23", "another text −AB23")]
        public void SameAlphanumericsInListDifferentMinusNoError(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.True(errorMessage.Count == 0);
        }

        [Theory]
        [InlineData("This is an alphanumeric CDE45", "Wrong alphanumeric CE45")]
        public void WrongAlphanumeric(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage[0].ErrorMessage);
        }

        [Theory]
        [InlineData("12AC and CDE45", "12AB Wrong alphanumeric CE45")]
        public void WrongAlphanumerics(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessages = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            foreach (var errorMessage in errorMessages)
            {
                Assert.Equal(PluginResources.Error_AlphanumericsModified, errorMessage.ErrorMessage);
            }
        }
    }
}