using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests
{
	public interface ISettingsBuilder
	{
		Mock<INumberVerifierSettings> Build();

		ISettingsBuilder AllowLocalization();
		ISettingsBuilder PreventLocalization();
		ISettingsBuilder RequireLocalization();

		ISettingsBuilder OmitLeadingZeroInSource();
		ISettingsBuilder OmitLeadingZeroInTarget();

		ISettingsBuilder WithSourceThousandSeparators(bool comma, bool period, string custom = null);
		ISettingsBuilder WithSourceDecimalSeparators(bool comma, bool period, string custom = null);
		ISettingsBuilder WithTargetThousandSeparators(bool comma, bool period, string custom = null);
		ISettingsBuilder WithTargetDecimalSeparators(bool comma, bool period, string custom = null);
	}
}
