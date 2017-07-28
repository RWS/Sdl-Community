using Moq;
using Sdl.Community.NumberVerifier.Tests.SourceSettings;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class CheckFullWidthJapaneseNumbers
	{
		/// <summary>
		/// Validate the full-width Japanese numbers: ２０１６ -> 2016
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[Theory]
		[InlineData("２０１６", "2016")]
		public void ValidateFullWidthJapaneseNumbers(string source, string target)
		{
			//target settings
			var numberVerifierSettings = SourceSettingsAndPreventLocalization.SpaceCommaPeriod();

			NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

			//run initialize method in order to set chosen separators
			var docPropMock = new Mock<IDocumentProperties>();
			numberVerifierMain.Initialize(docPropMock.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}
	}
}