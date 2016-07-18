using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
    /// <summary>
    /// Target separators
    /// </summary>
    public class SourceAndTargetNormalizationRequire
    {
        #region Check thousands numbers
        /// <summary>
        /// Source sep: decimal - comma , thousands - comma
        /// Target sep: decimal - period, thousands - comma, period, space
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1,554,5 negative number -1,554,5", "1,554.5 negative -1 554.5")]
        public void ThousandsSeparatorsSpaceCommaPeriod(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: decimal - comma , thousands - comma, period, space
        /// Target sep: decimal - comma, thousands - comma, period
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1 554,5 some word 1.234,5 another word -1,222,3", "1.554,5 test 1,234,5 another test word −1.222,3")]
        public void ThousandsSeparatorsCommaPeriod(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceThousandsPeriod).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Check for negative numbers
        /// Error : number modified/ unlocalized
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("-1 554,5", "1.554,5")]
        public void CheckNegativeNumbers(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_NumbersNotIdentical);
        }

        /// <summary>
        /// Target sep -thousands : space, thin space, no-break space -decimal : comma, period
        /// Source sep - thousands: space , - decimal: period, comma
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1 554.5 some word 1 234,5 another word −1 222,3", "1 554,5 test 1 234.5 another test word -1 222,3")]
        public void ThousandsSeparatorsAllTypesOfSpaces(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.AllTypesOfSpacesChecked();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);


            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }
        #endregion

        #region Check decimal numbers

        /// <summary>
        /// Source decimal: comma
        /// Target decimal: period
        /// No error
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1,55", "1.55")]
        public void DecimalSeparatorsComma(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);


        }

        /// <summary>
        /// Source decimal : comma
        /// Target decimal: comma, period
        /// No errors
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1,55 another number 1,34", "1.55 number 1,34")]
        public void DecimalSeparatorsCommaPeriod(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
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
        /// Error: number modified
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1,55", "1,55")]
        public void DecimalSeparatorsCommaInsteadOfPeriodErrorMessage(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.SpaceCommaPeriod();
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

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_NumbersNotIdentical);

        }

        /// <summary>
        /// Error number removed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1 554.5 some word 1,25", "1 554.5")]
        public void CheckForRemovedNumbers(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.AllTypesOfSpacesChecked();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);


            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage,PluginResources.Error_NumbersRemoved);
        }

        /// <summary>
        /// Error number added
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1 554.5", "1 554.5 some word 1.25")]
        public void CheckForAddedNumbers(string source, string target)
        {

            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.AllTypesOfSpacesChecked();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);


            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_NumbersAdded);
        }

        #endregion
        }
}
