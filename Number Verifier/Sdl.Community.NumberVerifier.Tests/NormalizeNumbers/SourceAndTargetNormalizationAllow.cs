using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
    /// <summary>
    /// Target separators + Source separators
    /// </summary>
    public class SourceAndTargetNormalizationAllow
    {

        #region Check thousands numbers and negative numbers

        /// <summary>
        /// Thousands sep: comma, period
        /// Decimal sep: period
        /// Error: no error
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1.554,5 negative number -1.554,5", "1,554,5 negative -1.554,5")]
        public void ThousandsSeparatorsCommaPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Thousands sep: comma, period, space
        /// Decimal: comma
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1 554,5 some word 1.234,5 another word -1 222,3", "1.554,5 test 1,234,5 another test word −1.222,3")]
        public void ThousandsSeparatorsSpaceCommaPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        [Theory]
        [InlineData("-1 554,5","1 554,5")]
        public void CheckNegativeNumbers(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_NumbersNotIdentical);
        }
        /// <summary>
        /// Thousands sep: space, thin space, no-break space
        /// Decimal sep: comma, period
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1 554.5 some word 1 234,5 another word −1 222,3", "1 554,5 test 1 234.5 another test word -1 222,3")]
        public void ThousandsSeparatorsAllTypesOfSpaces(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.AllTypesOfSpacesChecked();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Thousands sep: no separator, comma, period
        /// Decimal sep: comma, period
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1554.5 some word 1.234,5 another word 1222,3", "1,554,5 test 1234.5 another test word 1222,3")]
        public void ThousandsSeparatorsNoSeparatorCommaPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.TargetNoSeparator).Returns(true);
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetThousandsPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        #endregion

        #region Check only decimal numbers

        /// <summary>
        /// Check decimal numbers, both numbers should have comma as separator
        /// Error message: Number modified
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1,55", "1.55")]
        public void DecimalSeparatorsComma(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_NumbersNotIdentical);
        }


        /// <summary>
        /// Check decimal numbers, numbers can have period or comma as separator
        /// No error message
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("2,55", "2.55")]
        public void DecimalSeparatorsCommaAndPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        #endregion

        #region Check error messages

        /// <summary>
        /// Check decimal numbers and removed numbers error.
        /// Error: Number removed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("2,55 1,25", "2.55")]
        public void CheckForRemovedNumbers(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_NumbersRemoved);
        }

        /// <summary>
        /// Check decimal numbers and added numbers error.
        /// Error: Number added.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("2,55", "2.55 1,25")]
        public void CheckForAddedNumbers(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);
            Assert.Equal(errorMessage[0].ErrorMessage,PluginResources.Error_NumbersAdded);
        }

        #endregion
    }
}
