using Moq;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
	public class AlphanumericCustomSeparators
	{
		[Theory]
		[InlineData("BS2-3", "-")]
		public void FindAlphanumericWithCustomSeparators(string text, string customSeparators)
		{
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CustomSeparators();
			numberVerifierSettings.Setup(c=>c.CustomsSeparatorsAlphanumerics).Returns(true);
			numberVerifierSettings.Setup(c => c.GetAlphanumericsCustomSeparator).Returns(customSeparators);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var textAlphanumericsList = numberVerifierMain.GetAlphanumericList(text);

			Assert.True(textAlphanumericsList.Count != 0);
		}

		[Theory]
		[InlineData("12S\\A*-3", "\\*-")]
		public void FindAlphanumericWithMultipleCustomSeparators(string text, string customSeparators)
		{
			var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CustomSeparators();
			numberVerifierSettings.Setup(c => c.CustomsSeparatorsAlphanumerics).Returns(true);
			numberVerifierSettings.Setup(c => c.GetAlphanumericsCustomSeparator).Returns(customSeparators);

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);

			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var textAlphanumericsList = numberVerifierMain.GetAlphanumericList(text);

			Assert.True(textAlphanumericsList.Count != 0);
		}
	}
}