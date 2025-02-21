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
        private readonly Mock<IDocumentProperties> _documentProperties;

        public SourceAndTargetNormalizationAllow()
        {
            _documentProperties = new Mock<IDocumentProperties>();
        }

        [Theory]
        [InlineData("-1 554,5", "1 554,5")]
        public void CheckNegativeNumbers(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_DifferentValues, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Thousands sep: space, thin space, no-break space
        /// Decimal sep: comma, period
        /// </summary>
        [Theory]
        [InlineData("1 554.5 some word 1 234,5 another word −1 222,3", "1 554,5 test 1 234.5 another test word -1 222,3")]
        public void ThousandsSeparatorsAllTypesOfSpaces(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.AllTypesOfSpacesChecked();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.SourceDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Thousands sep: comma, period
        /// Decimal sep: period
        /// </summary>
        [Theory]
        [InlineData("1.554,5 negative number -1.554,5", "1,554,5 negative -1.554,5")]
        public void ThousandsSeparatorsCommaPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Collection(errorMessage,
		        error => Assert.Equal(error.ErrorMessage, PluginResources.Error_DifferentSequences),
		        error =>
			        Assert.Equal(error.ErrorMessage,
				        PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator));

        }

        /// <summary>
        /// Thousands sep: no separator, comma, period
        /// Decimal sep: comma, period
        /// </summary>
        [Theory]
        [InlineData("1,554.5 some word 1.234,5 another word 1.222,3", "1,554,5 test 1,234.5 another test word 1.222,3")]
        public void ThousandsSeparatorsNoSeparatorCommaPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.TargetNoSeparator).Returns(true);
            numberVerifierSettings.Setup(d => d.SourceDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetThousandsComma).Returns(true);
            numberVerifierSettings.Setup(d => d.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				error => Assert.Equal(error.ErrorMessage, PluginResources.Error_DifferentSequences),
				error =>
					Assert.Equal(error.ErrorMessage,
						PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator));
		}

        /// <summary>
        /// Thousands sep: comma, period, space
        /// Decimal: comma
        /// </summary>
        [Theory]
        [InlineData("1 554,5 some word 1.234,5 another word -1 222,3", "1.554,5 test 1,234,5 another test word −1.222,3")]
        public void ThousandsSeparatorsSpaceCommaPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Collection(errorMessage,
		        error => Assert.Equal(error.ErrorMessage, PluginResources.Error_DifferentSequences),
		        error =>
			        Assert.Equal(error.ErrorMessage,
				        PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator));
        }

        /// <summary>
        /// Check decimal numbers, both numbers should have comma as separator
        /// </summary>
        [Theory]
        [InlineData("1,55", "1.55")]
        public void DecimalSeparatorsComma(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Check decimal numbers, numbers can have period or comma as separator
        /// Error message: Number modified
        /// </summary>
        [Theory]
        [InlineData("1,55", "1.55")]
        public void DecimalSeparatorsComma_WithErrors(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_DifferentSequences, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Check decimal numbers, numbers can have period or comma as separator
        /// No error message
        /// </summary>
        [Theory]
        [InlineData("2,55", "2.55")]
        public void DecimalSeparatorsCommaAndPeriod(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Check decimal numbers and added numbers error.
        /// Error: Number added.
        /// </summary>
        [Theory]
        [InlineData("2,55", "2.55 1,25")]
        public void CheckForAddedNumbers(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);
            Assert.Equal(PluginResources.Error_NumberAdded, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Check decimal numbers and removed numbers error.
        /// Error: Number removed.
        /// </summary>
        [Theory]
        [InlineData("2,55 1,25", "2.55")]
        public void CheckForRemovedNumbers(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_NumbersRemoved, errorMessage[0].ErrorMessage);
        }
    }
}