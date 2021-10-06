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
			var normalizedNumbers = _numberNormalizer.GetNormalizedNumbers(number, numberVerifierSettings.Object, false);

			return normalizedNumbers;
		}

		[Theory]
		[InlineData(".55")]
		public void CheckNumberIfOmitZeroIsChecked(string number)
		{
			var numberList = OmitZeroChecked(number);

			Assert.Equal("m55", numberList.NormalizedPartsList[0]);
			Assert.Equal(".55", numberList.InitialPartsList[0]);
		}

		public NumberList GetNormalizedNumberWhenLeadingZeroOmittedAndNotAllowed(string text)
		{
			var iMockSettings = NumberVerifierLocalizationsSettings.AllowLocalization();
			iMockSettings.Setup(s => s.SourceDecimalPeriod).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(iMockSettings);

			var normalizedNumber = _numberNormalizer.GetNormalizedNumbers(text, iMockSettings.Object, false);

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

			var normalizedNumber = _numberNormalizer.GetNormalizedNumbers(text, mockSettings.Object, false);

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
			var normalizedNumberWithPeriod = _numberNormalizer.GetNormalizedNumbers(numberWithPeriod, mockSettings.Object, false);

			mockSettings.Setup(s => s.TargetDecimalComma).Returns(true);
			var normalizedNumberWithComma = _numberNormalizer.GetNormalizedNumbers(numberWithComma, mockSettings.Object, false);

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

			var normalizedNumbers = _numberNormalizer.GetNormalizedNumbers(number, numberVerifierSettings.Object, false);

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