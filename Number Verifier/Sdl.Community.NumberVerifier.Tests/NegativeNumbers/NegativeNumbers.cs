using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NegativeNumbers
{
    public class NegativeNumbers
    {
        [Theory]
        [InlineData("-230", ",.", ",.")]
        public string ReturnNegativeNumbersWithNormalMinusSign(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var iMockSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iMockSettings);
            var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);

            var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

            Assert.True(negativeNumberNormalized != string.Empty);
            return negativeNumberNormalized;
        }

        //http://www.fileformat.info/info/unicode/char/2212/browsertest.htm
        [Theory]
        [InlineData("−45", ",.", ",.")]
        public string ReturnNegativeNumbersWithSpecialMinusSign(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var iMockSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();

            var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);

            var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

            Assert.True(negativeNumberNormalized != string.Empty);
            return negativeNumberNormalized;
        }

        [Theory]
        [InlineData("-0.05", ",.", ",.")]
        public void CheckNormalizedNegativeNumberWithNormalMinusSign(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var negativeNumberNormalized = ReturnNegativeNumbersWithNormalMinusSign(text, decimalSeparators,
                thousandSeparators);

            Assert.Equal(negativeNumberNormalized, "m0.05");
        }

        [Theory]
        [InlineData("-.55", ",.", ",.")]
        private void CheckNormalizedShortNegativeNumberWithNormalMinusSign(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var negativeNumberNormalized = ReturnNegativeNumbersWithNormalMinusSign(text, decimalSeparators,
                thousandSeparators);

            Assert.Equal(negativeNumberNormalized, "m.55");
        }

        [Theory]
        [InlineData("−0.78", ",.", ",.")]
        public void CheckNormalizedNegativeNumberWithSpecialMinusSign(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var negativeNumberNormalized = ReturnNegativeNumbersWithSpecialMinusSign(text, decimalSeparators,
                thousandSeparators);

            Assert.Equal(negativeNumberNormalized, "m0.78");
        }


        [Theory]
        [InlineData("-.60", ",.", ",.")]
        public void CheckNormalizedShortNegativeNumberWithSpecialMinusSign(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var negativeNumberNormalized = ReturnNegativeNumbersWithSpecialMinusSign(text, decimalSeparators,
                thousandSeparators);

            Assert.Equal(negativeNumberNormalized, "m.60");
        }

       
        #region Check for negative numbers if there is a space between minus and dash
        
        /// <summary>
        /// if there is a space between number and minus sign that number is not a negative number
        /// and the method return the text unmodified
        /// </summary>

        [Theory]
        [InlineData("- 60", ",.", ",.")]
        public string CheckForNegativeNumbersWithNormalSpace(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var iMockSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();

            var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
            var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

            Assert.True(negativeNumberNormalized != string.Empty);

            return negativeNumberNormalized;
        }


        [Theory]
        [InlineData("- 70", ",.", ",.")]
        public string CheckForNegativeNumbersWithNoBreakSpace(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var iMockSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();

            var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
            var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

            Assert.True(negativeNumberNormalized != string.Empty);

            return negativeNumberNormalized;
        }

        [Theory]
        [InlineData("- 80", ",.", ",.")]
        public string CheckForNegativeNumbersWithThinSpace(string text, string decimalSeparators,
           string thousandSeparators)
        {
            var iMockSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();

            var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
            var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

            Assert.True(negativeNumberNormalized != string.Empty);

            return negativeNumberNormalized;
        }

        [Theory]
        [InlineData("- 90", ",.", ",.")]
        public string CheckForNegativeNumbersWithNarrowNoBreakSpace(string text, string decimalSeparators,
           string thousandSeparators)
        {
            var iMockSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();

            var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
            var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

            Assert.True(negativeNumberNormalized != string.Empty);

            return negativeNumberNormalized;
        }

        [Theory]
        [InlineData("- 60", ",.", ",.")]
        public void CheckForNegativeNumbersWithNormalSpaceAndMinusSign(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var negativeNumberNormalized = CheckForNegativeNumbersWithNormalSpace(text, decimalSeparators,
                thousandSeparators);

            Assert.Equal(negativeNumberNormalized, "- 60");
        }

        [Theory]
        [InlineData("- 60", ",.", ",.")]
        public void CheckForNegativeNumbersWithNormalSpaceAndSpecialMinusSign(string text, string decimalSeparators,
          string thousandSeparators)
        {
            var negativeNumberNormalized = CheckForNegativeNumbersWithNormalSpace(text, decimalSeparators,
                thousandSeparators);

            Assert.Equal(negativeNumberNormalized, "- 60");
        }

        #endregion
    }
}
