using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests
{
	public interface ISettingsBuilder
    {
        ISettingsBuilder AllowLocalization();

        Mock<INumberVerifierSettings> Build();

        ISettingsBuilder OmitLeadingZeroInSource();

        ISettingsBuilder OmitLeadingZeroInTarget();

        ISettingsBuilder PreventLocalization();

        ISettingsBuilder RequireLocalization();

        ISettingsBuilder WithSourceDecimalSeparators(bool comma, bool period, string custom = null);

        ISettingsBuilder WithSourceThousandSeparators(bool comma, bool period, string custom = null);

        ISettingsBuilder WithTargetDecimalSeparators(bool comma, bool period, string custom = null);

        ISettingsBuilder WithTargetThousandSeparators(bool comma, bool period, string custom = null);
    }
}