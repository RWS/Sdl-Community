using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class HindiNumbers
	{
		/// <summary>
		/// Validate correct numbers from Hindi to Arabic
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("١٢٣٤,٨٩", "1.234,89")]
		public void CheckFromHindiToArabicValid(string source, string target)
		{
			// settings
			var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
			numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Validate wrong numbers from Hindi to Arabic  
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("١٢٣٤,٨٩", "12.34,89")]
		public void CheckFromHindiToArabicInvalid(string source, string target)
		{
			// settings
			var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
			numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count != 0);
		}

		/// <summary>
		/// Validate correct numbers from Arabic to Hindi
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("1.234,89", "١٢٣٤,٨٩")]
		public void CheckFromArabicToHindiValid(string source, string target)
		{
			// settings
			var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
			numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

		/// <summary>
		/// Validate wrong numbers from Arabic to Hindi
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("12.34,89", "١٢٣٤,٨٩")]
		public void CheckFromArabicToHindiInValid(string source, string target)
		{
			// settings
			var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.HindiNumbers();
			numberVerifierSettings.Setup(s => s.HindiNumberVerification).Returns(true);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count != 0);
		}
	}
}