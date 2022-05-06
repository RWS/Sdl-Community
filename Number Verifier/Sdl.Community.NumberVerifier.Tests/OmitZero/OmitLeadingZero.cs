using Moq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.Community.NumberVerifier.Validator;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.OmitZero
{
	public class OmitLeadingZero
	{
		private readonly NumberValidator _numberValidator;

		public OmitLeadingZero()
		{
			_numberValidator = new NumberValidator();
		}

		[Theory]
		[InlineData(".55")]
		public void CheckNumberIfOmitZeroIsChecked(string number)
		{
			var numberList = OmitZeroChecked(number);

			Assert.Equal("0d55", numberList.Texts[0].Normalized);
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

		public NumberTexts GetNormalizedNumberWhenLeadingZeroOmittedAndNotAllowed(string text)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			iMockSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(iMockSettings);

			_numberValidator.Verify(text, null, iMockSettings.Object, out var normalizedNumber, out _);

			return normalizedNumber;
		}

		public NumberTexts OmitZeroChecked(string number)
		{
			var numberVerifierSettings = OmitZeroSettings.OmitZeroCheckedAndPreventLocalization();
			numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			_numberValidator.Verify(number, null, numberVerifierSettings.Object, out var normalizedNumbers, out _);

			return normalizedNumbers;
		}

		[Theory]
		[InlineData("0.55")]
		public void OmitZeroLongForm(string text)
		{
			var normalizedNumber = GetNormalizedNumberWhenLeadingZeroOmittedAndNotAllowed(text);

			Assert.Equal("0d55", normalizedNumber.Texts[0].Normalized);
		}

		[Theory]
		[InlineData("-0.55", "-0,55")]
		public void OmitZeroLongFormNegativeNumbersMinusSign(string numberWithPeriod, string numberWithComma)
		{
			var mockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			mockSettings.Setup(s => s.TargetOmitLeadingZero).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(mockSettings);

			mockSettings.Setup(s => s.TargetDecimalPeriod).Returns(true);
			_numberValidator.Verify(null, numberWithPeriod, mockSettings.Object, out _, out var normalizedNumberWithPeriod);

			mockSettings.Setup(s => s.TargetDecimalComma).Returns(true);
			_numberValidator.Verify(null, numberWithComma, mockSettings.Object, out _, out var normalizedNumberWithComma);

			Assert.Equal("n0d55", normalizedNumberWithPeriod.Texts[0].Normalized);
			Assert.Equal("n0d55", normalizedNumberWithComma.Texts[0].Normalized);
		}

		[Theory]
		[InlineData(".55")]
		public string OmitZeroShortForm(string text)
		{
			var normalizedNumber = GetNormalizedNumberWhenLeadingZeroOmittedAndNotAllowed(text);

			Assert.Null(normalizedNumber.Texts[0].Normalized);
			Assert.False(normalizedNumber.Texts[0].IsValidNumber);

			return normalizedNumber.Texts[0].Normalized;
		}

		[Theory]
		[InlineData("-.55")]
		public string OmitZeroShortFormNegativeNumbers(string text)
		{
			var mockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			mockSettings.Setup(s => s.TargetOmitLeadingZero).Returns(true);
			mockSettings.Setup(s => s.TargetDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(mockSettings);

			_numberValidator.Verify(null, text, mockSettings.Object, out _, out var normalizedNumber);

			Assert.Equal("n0d55", normalizedNumber.Texts[0].Normalized);

			return normalizedNumber.Texts[0].Normalized;
		}

		[Theory]
		[InlineData("−.55")]
		public void OmitZeroShortFormNegativeNumbersSpecialMinusSign(string text)
		{
			var normalizedNumber = OmitZeroShortFormNegativeNumbers(text);
			Assert.Equal("n0d55", normalizedNumber);
		}

		//#region Omit leading zero option is unchecked

		public string OmitZeroUnchecked(string number)
		{
			var numberVerifierSettings = OmitZeroSettings.OmitZeroUncheckedAndAllowLocalization();
			numberVerifierSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);
			var methodsMock = new Mock<INumberVerifierMethods>(MockBehavior.Strict);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

			_numberValidator.Verify(number, null, numberVerifierSettings.Object, out var normalizedNumbers, out _);

			methodsMock.Verify(m => m.OmitZero(number), Times.Never);
			return normalizedNumbers.Texts[0].Normalized;
		}

		//#endregion
	}
}