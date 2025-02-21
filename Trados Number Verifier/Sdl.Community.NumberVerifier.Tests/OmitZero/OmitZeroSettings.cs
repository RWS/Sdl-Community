using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests.OmitZero
{
	public class OmitZeroSettings
	{
		public static Mock<INumberVerifierSettings> OmitZeroCheckedAndPreventLocalization()
		{
			var numberVerifierSettings = Utilities.NumberVerifierLocalizationsSettings.PreventLocalization();
			numberVerifierSettings.Setup(z => z.SourceOmitLeadingZero).Returns(true);
			numberVerifierSettings.Setup(z => z.TargetDecimalPeriod).Returns(true);

			return numberVerifierSettings;
		}

		public static Mock<INumberVerifierSettings> OmitZeroUncheckedAndAllowLocalization()
		{
			var numberVerifierSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();
			numberVerifierSettings.Setup(z => z.SourceOmitLeadingZero).Returns(false);

			return numberVerifierSettings;
		}
	}
}