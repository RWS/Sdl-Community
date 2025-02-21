using System.Collections.Generic;
using System.Linq;
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
        private readonly Mock<IDocumentProperties> _documentProperties;

        public SourceAndTargetNormalizationRequire()
        {
            _documentProperties = new Mock<IDocumentProperties>();
        }

        /// <summary>
        /// Check for negative numbers
        /// Error : number modified/unlocalized
        /// </summary>
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

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_DifferentValues, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Target sep -thousands : space, thin space, no-break space -decimal : comma, period
        /// Source sep - thousands: space , - decimal: period, comma
        /// </summary>
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


            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: decimal - comma , thousands - comma, period, space
        /// Target sep: decimal - comma, thousands - comma, period
        /// </summary>
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
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Collection(errorMessage,
		        error => Assert.Equal(PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator, error.ErrorMessage),
		        error => Assert.Equal(PluginResources.Error_DifferentSequences, error.ErrorMessage),
		        error => Assert.Equal(PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator, error.ErrorMessage),
		        error => Assert.Equal(PluginResources.Error_DifferentSequences, error.ErrorMessage));

        }

        /// <summary>
        /// Source sep: decimal - comma , thousands - comma
        /// Target sep: decimal - period, thousands - comma, period, space
        /// </summary>
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
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Collection(errorMessage,
		        error =>
			        Assert.Equal(error.ErrorMessage,
				        PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator),
		        error =>
			        Assert.Equal(error.ErrorMessage,
				        PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator),
		        error => Assert.Equal(error.ErrorMessage, PluginResources.Error_DifferentSequences),
		        error => Assert.Equal(error.ErrorMessage, PluginResources.Error_DifferentSequences));
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("1000", "1,000")]
        public void ValidateSource_NoSeparatorIsChecked_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("Simple test with comma after number 1000,for all", "1,000")]
        public void ValidateSource_ThousandAfterComma_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.ReportModifiedAlphanumerics).Returns(false);
            numberVerifierSettings.Setup(s => s.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: modified number error should be returned
        /// </summary>
        [Theory]
        [InlineData("2000,", "1,000")]
        public void ValidateSource_ThousandAfterComma_WithErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_DifferentValues);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("Simple test with 1000 and a comma before second number,1000", "Simple test with 1000 and second number 1,000")]
        public void ValidateSource_ThousandBeforeComma_CombinedNumbers_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("Simple test with comma before number,1000", "1,000")]
        public void ValidateSource_ThousandBeforeComma_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: modified number error should be returned
        /// </summary>
        [Theory]
        [InlineData(",1000", "2,000")]
        public void ValidateSource_ThousandBeforeComma_WithErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_DifferentValues);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("This is a simple test with 3000 and a comma before second number...3000", "This is a simple test with 3000 and second number 3,000")]
        [InlineData("This is a simple test with sum of 10 +12000 and a comma before second number...3000", "This is a simple test with sum of 10 +12.000 and second number 3,000")]
        public void ValidateSource_ThousandBeforeComma_WithMultiplePunctuationMarks_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsPeriod).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("This is a simple test with 3000 and a comma before second number, 3000", "This is a simple test with 3000 and second number 3,000")]
        public void ValidateSource_ThousandBeforeComma_WithSpaces_CombinedNumbers_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: Thousand comma sep, 'No separator' checked
        /// Target sep: 'No separator' checked
        /// No validation error should be displayed
        /// </summary>
        [Theory]
        [InlineData("1,000", "1000")]
        public void ValidateSource_ThousandsSeparatorsComma_NoSeparatorIsChecked(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(d => d.TargetNoSeparator).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'Comma' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("This is 12,000 and 3000 and a comma before second number, 3000", "This is 12000 and 3000 and second number 3,000")]
        public void ValidateSource_WithSpaces_CombinedNumbers_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.CustomsSeparatorsAlphanumerics).Returns(false);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: 'No separator' checked
        /// Verification error: Number(s) modified/unlocalised.
        /// </summary>
        [Theory]
        [InlineData("1000", "2,000")]
        public void ValidateTarget_NoSeparatorIsChecked_Errors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.AllTypesOfSpacesChecked();
            numberVerifierSettings.Setup(d => d.TargetNoSeparator).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_DifferentValues, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Source sep: 'Comma' checked
        /// Target sep: 'No separator' checked
        /// Verification error: no errors should be returned.
        /// </summary>
        [Theory]
        [InlineData("1,000", "1000")]
        public void ValidateTarget_NoSeparatorIsChecked_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetNoSeparator).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source sep: 'No separator' checked
        /// Target sep: thousands - comma, 'No separator' checked
        /// </summary>
        [Theory]
        [InlineData("1000", "1,000")]
        public void ValidateTarget_ThousandsSeparatorsComma_NoSeparatorIsChecked(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(d => d.TargetThousandsComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source decimal: comma
        /// Target decimal: period
        /// No error
        /// </summary>
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
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source decimal : comma
        /// Target decimal: comma, period
        /// No errors
        /// </summary>
        [Theory]
        [InlineData("1,55 another number 1,34", "1.55 number 1,34")]
        public void DecimalSeparatorsCommaPeriod(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Source decimal: comma and Source NoSeparator option
        /// Target decimal: period and Target NoSeparator option
        /// No error
        /// </summary>
        [Theory]
        [InlineData("1,55", "1.55")]
        public void Validate_DecimalSeparatorsComma_WhenNoSeparatorOption_IsChecked(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetNoSeparator).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            numberVerifierSettings.Setup(d => d.SourceNoSeparator).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Error number added
        /// </summary>
        [Theory]
        [InlineData("1 554.5", "1 554.5 some word 1.25")]
        public void CheckForAddedNumbers(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.ThousandsSeparatorsSpaceAndNoBreak();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_NumberAdded, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Error number removed
        /// </summary>
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
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_NumbersRemoved, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Validate text for source with no separator option and target with no-break space option.
        /// No error message returned
        /// </summary>
        [Theory]
        [InlineData("2300", "2 300")]
        public void CheckNoSeparatorNumbers(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.AllTypesOfSpacesChecked();

            numberVerifierSettings.Setup(s => s.TargetNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.TargetThousandsSpace).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Validate text for source with comma thousands option and target with no separator option.
        /// No error message returned
        /// </summary>
        [Theory]
        [InlineData("2,300", "2300")]
        public void CheckThousandCommaNoSeparatorNumbers(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetNoSeparator).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Error: number modified
        /// </summary>
        [Theory]
        [InlineData("1,55", "1,55")]
        public void DecimalSeparatorsCommaInsteadOfPeriodErrorMessage(string source, string target)
        {
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.SpaceCommaPeriod();
            numberVerifierSettings.Setup(a => a.CustomsSeparatorsAlphanumerics).Returns(false);

            // target settings
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(false);
            numberVerifierSettings.Setup(d => d.TargetThousandsComma).Returns(false);
            numberVerifierSettings.Setup(d => d.TargetThousandsSpace).Returns(false);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);
            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        var expectedErrors = new List<string>
	        {
		        PluginResources.Error_DifferentValues,
		        PluginResources.Error_NumberAdded,
	        };

			Assert.Equal(expectedErrors, errorMessage.Select(em => em.ErrorMessage));
        }

        /// <summary>
        /// Validate decimal text where an empty space is added within the target text
        /// Validation error message returned, because the space validation are made for thousand numbers, and not decimals
        /// </summary>
        [Theory]
        [InlineData("600", "6 00")]
        public void ValidateDecimalWithNoSpace_NoSeparator_Errors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetNoSeparator).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsSpace).Returns(false);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_DifferentValues, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Validate groups of numbers without any words between them
        /// Validation errors should be reported
        /// </summary>
        [Theory]
        [InlineData("It was October 13, 1978.", "Es war der 13. Oktober 2000.")]
        public void ValidateMultipleNumbers_WithErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_DifferentValues, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Validate groups of numbers without any words between them
        /// No validation errors should be reported
        /// </summary>
        [Theory]
        [InlineData("It was October 13, 1978.", "Es war der 13. Oktober 1978.")]
        [InlineData("I will buy 14, 15 chocolates and it will cost 1.200 dollars", "I will buy 14, 15 chocolates and it will cost 1,200 dollars")]
        public void ValidateMultipleNumbers_WithNoErrors(string source, string target)
        {
            // target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);

            // source settings
            numberVerifierSettings.Setup(t => t.SourceThousandsPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            // run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Validate text for source with No separator option and target with no separator option + Sace option enabled.
        /// No error message returned
        /// </summary>
        [Theory]
        [InlineData("2300", "2 300")]
        public void ValidateTargetSpace_NoSeparator(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetNoSeparator).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsSpace).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Validate text for source with No separator option and target with no separator option + Space option disabled.
        /// Validation error message returned
        /// </summary>
        [Theory]
        [InlineData("1200", "1 201")]
        public void ValidateTargetSpace_NoSeparator_Errors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetNoSeparator).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsSpace).Returns(false);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Collection(errorMessage, 
		        em => Assert.Equal(PluginResources.Error_DifferentValues, em.ErrorMessage), 
		        em => Assert.Equal(PluginResources.Error_NumberAdded, em.ErrorMessage));
        }

        /// <summary>
        /// Validate big number containing thousand with comma and period separators
        /// No validation error should be returned
        /// </summary>
        [Theory]
        [InlineData("234,463.345", "234,463.345")]
        public void ValidateThousandNumbers_NoErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetDecimalPeriod).Returns(true);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Validate big number containing thousand with comma and period separators
        /// Validation errors should be reported
        /// </summary>
        [Theory]
        [InlineData("234,463.345", "234,463,345")]
        public void ValidateThousandNumbers_WithErrors(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetDecimalComma).Returns(false);

            // source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_DifferentValues, errorMessage[0].ErrorMessage);
        }
    }
}