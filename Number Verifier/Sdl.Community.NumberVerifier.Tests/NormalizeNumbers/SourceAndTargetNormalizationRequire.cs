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
        /// Source sep: 'No separator' checked
        /// Target sep: thousands - comma, 'No separator' checked
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("1000", "1,000")]
        public void ValidateTarget_ThousandsSeparatorsComma_NoSeparatorIsChecked(string source, string target)
        {
			//target settings
			var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
			numberVerifierSettings.Setup(d => d.TargetNoSeparator).Returns(true);
			numberVerifierSettings.Setup(d => d.TargetThousandsComma).Returns(true);

			//source settings
			numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Source sep: 'No separator' checked
		/// Target sep: 'No separator' checked
		/// Verification error: Number(s) modified/unlocalised.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
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
	        var docPropMock = new Mock<IDocumentProperties>();
	        numberVerifierMain.Initialize(docPropMock.Object);

	        var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0].ErrorMessage);
        }


		/// <summary>
		/// Source sep: Thousand comma sep, 'No separator' checked
		/// Target sep: 'No separator' checked
		/// No validation error should be displayed
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
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
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Source sep: 'No separator' checked
		/// Target sep: 'No separator' checked
		/// Verification error: no errors should be returned.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("1,000", "1000")]
		public void ValidateSource_NoSeparatorIsChecked_NoErrors(string source, string target)
		{
			//target settings
			var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.AllTypesOfSpacesChecked();
			numberVerifierSettings.Setup(d => d.TargetNoSeparator).Returns(true);

			//source settings
			numberVerifierSettings.Setup(s => s.SourceNoSeparator).Returns(true);
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
        /// Error : number modified/unlocalized
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

            Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0].ErrorMessage);
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
        /// Source decimal: comma and Source NoSeparator option
        /// Target decimal: period and Target NoSeparator option
        /// No error
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0].ErrorMessage);
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

            Assert.Equal(PluginResources.Error_NumbersRemoved, errorMessage[0].ErrorMessage);
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
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.ThousandsSeparatorsSpaceAndNoBreak();
            numberVerifierSettings.Setup(d => d.TargetDecimalPeriod).Returns(true);

			//source settings
			numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
			numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

            Assert.Equal(PluginResources.Error_NumbersAdded, errorMessage[0].ErrorMessage);
        }

        /// <summary>
        /// Validate text for source with no separator option and target with no-break space option.
        /// No error message returned
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
	        var docPropMock = new Mock<IDocumentProperties>();
	        numberVerifierMain.Initialize(docPropMock.Object);

	        var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Validate text for source with comma thousands option and target with no separator option.
        /// No error message returned
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
	        var docPropMock = new Mock<IDocumentProperties>();
	        numberVerifierMain.Initialize(docPropMock.Object);

	        var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Validate text for source with No separator option and target with no separator option + Sace option enabled.
        /// No error message returned
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
	        var docPropMock = new Mock<IDocumentProperties>();
	        numberVerifierMain.Initialize(docPropMock.Object);

	        var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.True(errorMessage.Count == 0);
        }

        /// <summary>
        /// Validate text for source with No separator option and target with no separator option + Space option disabled.
        /// Validation error message returned
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
	        var docPropMock = new Mock<IDocumentProperties>();
	        numberVerifierMain.Initialize(docPropMock.Object);

	        var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0].ErrorMessage);
        }


        /// <summary>
        /// Validate decimal text where an empty space is added within the target text
        /// Validation error message returned, because the space validation are made for thousand numbers, and not decimals
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
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
	        var docPropMock = new Mock<IDocumentProperties>();
	        numberVerifierMain.Initialize(docPropMock.Object);

	        var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.Equal(PluginResources.Error_NumbersNotIdentical, errorMessage[0].ErrorMessage);
        }


        /// <summary>
        /// Validate big number containing thousand and decimals
        /// No validation error should be returned
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("234,463.345", "234,463,345")]
        public void ValidateThousandDecimalsNumbers_NoErrors(string source, string target)
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
	        var docPropMock = new Mock<IDocumentProperties>();
	        numberVerifierMain.Initialize(docPropMock.Object);

	        var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

	        Assert.True(errorMessage.Count == 0);
        }
		#endregion
	}
}