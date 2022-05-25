using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NegativeNumbers
{
	public class NegativeNumbers
	{
		[Theory]
		[InlineData("-0.05", ",.", ",.")]
		public void CheckNormalizedNegativeNumberWithNormalMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var negativeNumberNormalized = ReturnNegativeNumbersWithNormalMinusSign(text, decimalSeparators, thousandSeparators);

			Assert.Equal("m0.05", negativeNumberNormalized);
		}

		[Theory]
		[InlineData("−0.78", ",.", ",.")]
		public void CheckNormalizedNegativeNumberWithSpecialMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var negativeNumberNormalized = ReturnNegativeNumbersWithSpecialMinusSign(text, decimalSeparators, thousandSeparators);

			Assert.Equal("m0.78", negativeNumberNormalized);
		}

		[Theory]
		[InlineData("-.60", ",.", ",.")]
		public void CheckNormalizedShortNegativeNumberWithSpecialMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var negativeNumberNormalized = ReturnNegativeNumbersWithSpecialMinusSign(text, decimalSeparators, thousandSeparators);

			Assert.Equal("m.60", negativeNumberNormalized);
		}

		[Theory]
		[InlineData("-230", ",.", ",.")]
		public string ReturnNegativeNumbersWithNormalMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			NumberVerifierLocalizationsSettings.InitSeparators(iMockSettings);
			var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);

			var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

			Assert.True(!string.IsNullOrEmpty(negativeNumberNormalized));
			return negativeNumberNormalized;
		}

		//http://www.fileformat.info/info/unicode/char/2212/browsertest.htm
		[Theory]
		[InlineData("−45", ",.", ",.")]
		public string ReturnNegativeNumbersWithSpecialMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();

			var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);

			var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

			Assert.True(!string.IsNullOrEmpty(negativeNumberNormalized));
			return negativeNumberNormalized;
		}

		[Theory]
		[InlineData(" −5", ",.", ",.")]
		public void ReturnNormalizedNegativeNumber_WithSpecialMinusSignAndSpace(string text, string decimalSeparators, string thousandSeparators)
		{
			var negativeNumberNormalized = ReturnNegativeNumbersWithNormalMinusSign(text, decimalSeparators, thousandSeparators);

			Assert.Equal(" m5", negativeNumberNormalized);
		}

		[Theory]
		[InlineData("-.55", ",.", ",.")]
		private void CheckNormalizedShortNegativeNumberWithNormalMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var negativeNumberNormalized = ReturnNegativeNumbersWithNormalMinusSign(text, decimalSeparators, thousandSeparators);

			Assert.Equal("m.55", negativeNumberNormalized);
		}

		/// <summary>
		/// if there is a space between number and minus sign that number is not a negative number
		/// and the method return the text unmodified
		/// </summary>

		[Theory]
		[InlineData("- 90", ",.", ",.")]
		public string CheckForNegativeNumbersWithNarrowNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();

			var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
			var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

			Assert.True(!string.IsNullOrEmpty(negativeNumberNormalized));

			return negativeNumberNormalized;
		}

		[Theory]
		[InlineData("- 70", ",.", ",.")]
		public string CheckForNegativeNumbersWithNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();

			var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
			var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

			Assert.True(negativeNumberNormalized != string.Empty);

			return negativeNumberNormalized;
		}

		[Theory]
		[InlineData("- 60", ",.", ",.")]
		public string CheckForNegativeNumbersWithNormalSpace(string text, string decimalSeparators, string thousandSeparators)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();

			var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
			var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

			Assert.True(negativeNumberNormalized != string.Empty);

			return negativeNumberNormalized;
		}

		[Theory]
		[InlineData("- 60", ",.", ",.")]
		public void CheckForNegativeNumbersWithNormalSpaceAndMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var negativeNumberNormalized = CheckForNegativeNumbersWithNormalSpace(text, decimalSeparators, thousandSeparators);

			Assert.Equal("- 60", negativeNumberNormalized);
		}

		[Theory]
		[InlineData("- 60", ",.", ",.")]
		public void CheckForNegativeNumbersWithNormalSpaceAndSpecialMinusSign(string text, string decimalSeparators, string thousandSeparators)
		{
			var negativeNumberNormalized = CheckForNegativeNumbersWithNormalSpace(text, decimalSeparators, thousandSeparators);

			Assert.Equal("- 60", negativeNumberNormalized);
		}

		[Theory]
		[InlineData("- 80", ",.", ",.")]
		public string CheckForNegativeNumbersWithThinSpace(string text, string decimalSeparators, string thousandSeparators)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();

			var numberVerifierMain = new NumberVerifierMain(iMockSettings.Object);
			var negativeNumberNormalized = numberVerifierMain.NormalizeNumberWithMinusSign(text);

			Assert.True(!string.IsNullOrEmpty(negativeNumberNormalized));

			return negativeNumberNormalized;
		}
	}
}