using Moq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.OmitZero
{
	public class OmitLeadingZero
	{
		private readonly NumberNormalizer _numberNormalizer;

		public OmitLeadingZero()
		{
			_numberNormalizer = new NumberNormalizer();
		}

		public NumberList OmitZeroChecked(string number)
		{
			var numberVerifierSettings = OmitZeroSettings.OmitZeroCheckedAndPreventLocalization();
			numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			_numberNormalizer.GetNormalizedNumbers(number, null, numberVerifierSettings.Object, out var normalizedNumbers, out _);

			return normalizedNumbers;
		}

		[Theory]
		[InlineData(".55")]
		public void CheckNumberIfOmitZeroIsChecked(string number)
		{
			var numberList = OmitZeroChecked(number);

			Assert.Equal("0m55", numberList.NormalizedPartsList[0]);
			Assert.Equal("0.55", numberList.InitialPartsList[0]);
		}

		public NumberList GetNormalizedNumberWhenLeadingZeroOmittedAndNotAllowed(string text)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			iMockSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(iMockSettings);

			_numberNormalizer.GetNormalizedNumbers(text, null, iMockSettings.Object, out var normalizedNumber, out _);

			return normalizedNumber;
		}

		[Theory]
		[InlineData(".55")]
		public string OmitZeroShortForm(string text)
		{
			var normalizedNumber = GetNormalizedNumberWhenLeadingZeroOmittedAndNotAllowed(text);

			Assert.Equal("m55", normalizedNumber.NormalizedPartsList[0]);

			return normalizedNumber.NormalizedPartsList[0];
		}

		[Theory]
		[InlineData("0.55")]
		public void OmitZeroLongForm(string text)
		{
			var normalizedNumber = GetNormalizedNumberWhenLeadingZeroOmittedAndNotAllowed(text);

			Assert.Equal("0m55", normalizedNumber.NormalizedPartsList[0]);
		}

		[Theory]
		[InlineData("-.55")]
		public string OmitZeroShortFormNegativeNumbers(string text)
		{
			var mockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			mockSettings.Setup(s => s.TargetOmitLeadingZero).Returns(true);
			mockSettings.Setup(s => s.TargetDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(mockSettings);

			_numberNormalizer.GetNormalizedNumbers(null, text, mockSettings.Object, out _, out var normalizedNumber);

			Assert.Equal("m0m55", normalizedNumber.NormalizedPartsList[0]);

			return normalizedNumber.NormalizedPartsList[0];
		}

		[Theory]
		[InlineData("−.55")]
		public void OmitZeroShortFormNegativeNumbersSpecialMinusSign(string text)
		{
			var normalizedNumber = OmitZeroShortFormNegativeNumbers(text);
			Assert.Equal("m0m55", normalizedNumber);
		}

		[Theory]
		[InlineData("-0.55", "-0,55")]
		public void OmitZeroLongFormNegativeNumbersMinusSign(string numberWithPeriod, string numberWithComma)
		{
			var mockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			mockSettings.Setup(s => s.TargetOmitLeadingZero).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(mockSettings);

			mockSettings.Setup(s => s.TargetDecimalPeriod).Returns(true);
			_numberNormalizer.GetNormalizedNumbers(null, numberWithPeriod, mockSettings.Object, out _, out var normalizedNumberWithPeriod);

			mockSettings.Setup(s => s.TargetDecimalComma).Returns(true);
			_numberNormalizer.GetNormalizedNumbers(null, numberWithComma, mockSettings.Object, out _, out var normalizedNumberWithComma);

			Assert.Equal("m0m55", normalizedNumberWithPeriod.NormalizedPartsList[0]);
			Assert.Equal("m0m55", normalizedNumberWithComma.NormalizedPartsList[0]);
		}

		#region Omit leading zero option is unchecked


		public string OmitZeroUnchecked(string number)
		{
			var numberVerifierSettings = OmitZeroSettings.OmitZeroUncheckedAndAllowLocalization();
			numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);
			var methodsMock = new Mock<INumberVerifierMethods>(MockBehavior.Strict);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

			_numberNormalizer.GetNormalizedNumbers(number, null, numberVerifierSettings.Object, out var normalizedNumbers, out _);

			methodsMock.Verify(m => m.OmitZero(number), Times.Never);
			return normalizedNumbers.InitialPartsList[0];
		}

		/// <summary>
		/// If the option is unchecked the method shouldn't be called
		/// </summary>
		/// <param name="number"></param>
		[Theory]
		[InlineData(".55")]
		public void CheckNumberIfOmitZeroIsUnchecked(string number)
		{
			var normalizedNumber = OmitZeroUnchecked(number);

			Assert.True(normalizedNumber != "0.55");
		}

		#endregion
	}
}